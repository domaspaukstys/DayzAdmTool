using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DayzServerManager
{
    /// <summary>
    /// Interaction logic for MapControl.xaml
    /// </summary>
    public partial class MapControl
    {
        private readonly Dictionary<string, Vehicle> _vehicles = new Dictionary<string, Vehicle>();
        private readonly Dictionary<string, Player> _players = new Dictionary<string, Player>();
        private readonly Dictionary<string, Player> _corpses = new Dictionary<string, Player>(); 
        private readonly Regex _worldSpaceRegex = new Regex(@"(\[[^\\[]+\[)([^\]]+)");
        private const double ScaleX = 0.4054;
        private const double ScaleY = 0.4054;
        private const double PositionX = -175;
        private const double PositionY = -590;
        private readonly Timer _timer = new Timer(5000);
        private readonly MySqlWrapper _wrapper = Context.Instance.DbWrapper;
        private readonly InnerElementsScaleConverter _innerElementsScaleConverter = new InnerElementsScaleConverter();


        public MapControl()
        {
            InitializeComponent();
            DataContext = this;
            Context.Instance.DBConnectionOpen += (sender, args) => StartTimer();
        }

        private void StartTimer()
        {
            _timer.AutoReset = true;
            _timer.Stop();
            _timer.Elapsed -= TimerOnElapsed;
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            LoadVehicleData();
            LoadUserData();
        }

        private void LoadUserData()
        {
            DataTable data = _wrapper.GetPlayers();
            Dispatcher.Invoke(() =>
            {
                foreach (DataRow row in data.Rows)
                {
                    double x, y;
                    if (TryParsePosition(row["Worldspace"].ToString(), out x, out y))
                    {
                        Player player;
                        if (row["Alive"].ToString() == "1")
                        {
                            string key = row["PlayerUID"].ToString();
                            if (!_players.TryGetValue(key, out player))
                            {
                                player = new Player(row["playerName"].ToString());
                                BindToScale(player);
                                _players.Add(key, player);
                                _canvasMap.Children.Add(player);
                            }
                        }
                        else
                        {
                            string key = row["Worldspace"].ToString();
                            if (!_corpses.TryGetValue(key, out player))
                            {
                                player = new Player(row["playerName"].ToString()) {Background = Brushes.LightSlateGray};
                                BindToScale(player);
                                _corpses.Add(key, player);
                                _canvasMap.Children.Add(player);
                            }
                        }
                        player.UpdateDetails(row["Inventory"].ToString());
                        SetPosition(player, x, y);
                    }
                }
                _canvasMap.UpdateLayout();
            });
        }

        private void LoadVehicleData()
        {
            DataTable data = _wrapper.GetVehicles();
            Dispatcher.Invoke(() =>
            {
                foreach (DataRow row in data.Rows)
                {
                    double x, y;
                    if (TryParsePosition(row["Worldspace"].ToString(), out x, out y))
                    {
                        Vehicle vehicle;
                        string key = row["ObjectUID"].ToString();
                        if (!_vehicles.TryGetValue(key, out vehicle))
                        {
                            vehicle = new Vehicle(row["Classname"].ToString());
                            BindToScale(vehicle);
                            _vehicles.Add(key, vehicle);
                            _canvasMap.Children.Add(vehicle);
                        }
                        vehicle.UpdateDetails(row["Damage"].ToString(), row["Fuel"].ToString(), row["Hitpoints"].ToString(),
                            row["Inventory"].ToString());
                        SetPosition(vehicle, x, y);
                    }
                }
                _canvasMap.UpdateLayout();
            });
        }
        private bool TryParsePosition(string worldspace, out double x, out double y)
        {
            bool result = false;
            x = 0;
            y = 0;
            MatchCollection matches = _worldSpaceRegex.Matches(worldspace);
            if (matches.Count > 0)
            {
                if (matches[0].Groups.Count > 2)
                {
                    string[] vals = matches[0].Groups[2].Value.Split(',');
                    if (vals.Length > 1)
                    {
                        if (double.TryParse(vals[0], out x) && double.TryParse(vals[1], out y))
                        {
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        private void BindToScale(FrameworkElement element)
        {
            ScaleTransform scale = element.LayoutTransform as ScaleTransform;
            if (scale == null)
            {
                scale = new ScaleTransform();
                Binding binding = new Binding
                {
                    Path = new PropertyPath("Zoom"),
                    ElementName = "_scrollViewer",
                    Converter = _innerElementsScaleConverter
                };
                BindingOperations.SetBinding(scale, ScaleTransform.ScaleXProperty, binding);
                BindingOperations.SetBinding(scale, ScaleTransform.ScaleYProperty, binding);
                element.LayoutTransform = scale;
            }
        }

        private void SetPosition(UserControl control, double x, double y)
        {
            x = x * ScaleX + PositionX;
            y = y * ScaleY + PositionY;
            Canvas.SetLeft(control, x);
            Canvas.SetTop(control, _canvasMap.Height - y);
        }
    }
}
