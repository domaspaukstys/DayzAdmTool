using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DayzServerManager.Annotations;

namespace DayzServerManager
{
    /// <summary>
    /// Interaction logic for Vehicle.xaml
    /// </summary>
    public partial class Vehicle : INotifyPropertyChanged
    {
        public static readonly DependencyProperty ShowDetailsProperty = DependencyProperty.Register("ShowDetails", typeof(Visibility), typeof(Vehicle));
        public static readonly DependencyProperty HitpointsProperty = DependencyProperty.Register("Hitpoints", typeof(ObservableCollection<string>), typeof(Vehicle));
        public static readonly DependencyProperty InventoryProperty = DependencyProperty.Register("Inventory", typeof(ObservableCollection<string>), typeof(Vehicle));
        public static readonly DependencyProperty DamageProperty = DependencyProperty.Register("Damage", typeof(string), typeof(Vehicle));
        public static readonly DependencyProperty FuelProperty = DependencyProperty.Register("Fuel", typeof(string), typeof(Vehicle));
        public static readonly DependencyProperty ClassnameProperty = DependencyProperty.Register("Classname", typeof(string), typeof(Vehicle));


        public Visibility ShowDetails
        {
            get { return (Visibility)GetValue(ShowDetailsProperty); }
            set { SetValue(ShowDetailsProperty, value); }
        }
        public ObservableCollection<string> Hitpoints
        {
            get { return (ObservableCollection<string>)GetValue(HitpointsProperty); }
            set { SetValue(HitpointsProperty, value); }
        }
        public ObservableCollection<string> Inventory
        {
            get { return (ObservableCollection<string>)GetValue(InventoryProperty); }
            set { SetValue(InventoryProperty, value); }
        }
        public string Damage
        {
            get { return (string)GetValue(DamageProperty); }
            set { SetValue(DamageProperty, value); }
        }
        public string Fuel
        {
            get { return (string)GetValue(FuelProperty); }
            set { SetValue(FuelProperty, value); }
        }
        public string Classname
        {
            get { return (string)GetValue(ClassnameProperty); }
            set { SetValue(ClassnameProperty, value); }
        }


        public Vehicle(string className)
        {
            InitializeComponent();
            DataContext = this;

            Hitpoints = new ObservableCollection<string>();
            Inventory = new ObservableCollection<string>();
            Classname = className;
            ShowDetails = Visibility.Collapsed;
        }

        public void UpdateDetails(string damage, string fuel, string hitpoints, string inventory)
        {
            Damage = damage;
            Fuel = fuel;
            Hitpoints.Clear();
            Inventory.Clear();
            ParseHitpoints(hitpoints).ForEach(x => Hitpoints.Add(x));
            ParseInventory(inventory).ForEach(x => Inventory.Add(x));
            OnPropertyChanged();
        }

        private readonly Regex _hitpointsRegex = new Regex(@"\[""([^""]+)"",([^\]])");

        private List<string> ParseHitpoints(string hitpoints)
        {
            List<string> result = new List<string>();
            MatchCollection matchCollection = _hitpointsRegex.Matches(hitpoints);
            foreach (Match match in matchCollection)
            {
                result.Add(string.Format("{0} {1}", match.Groups[1].Value, match.Groups[2].Value));
            }
            return result;
        }

        private readonly Regex _inventoryRegex = new Regex(@"((\[\[\[)|(\[\[))([^\]]+)\],[ ]*\[([^\]]+)\]\]");

        private List<string> ParseInventory(string inventory)
        {
            List<string> result = new List<string>();
            MatchCollection matchCollection = _inventoryRegex.Matches(inventory);
            foreach (Match match in matchCollection)
            {
                string[] items = match.Groups[4].Value.Replace(@"""", "").Split(',');
                string[] counts = match.Groups[5].Value.Split(',');
                if (items.Length == counts.Length)
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        result.Add(string.Format("{0} {1}", items[i].Trim(), counts[i].Trim()));
                    }
                }
            }
            return result;
        } 

        private void FocusLost(object sender, RoutedEventArgs e)
        {
            ShowDetails = Visibility.Collapsed;
            OnPropertyChanged();
        }

        private void Click(object sender, MouseButtonEventArgs e)
        {
            ShowDetails = ShowDetails == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            OnPropertyChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
