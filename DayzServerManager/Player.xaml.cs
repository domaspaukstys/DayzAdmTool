using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using DayzServerManager.Annotations;

namespace DayzServerManager
{
    /// <summary>
    /// Interaction logic for Vehicle.xaml
    /// </summary>
    public partial class Player : INotifyPropertyChanged
    {
        public static readonly DependencyProperty ShowDetailsProperty = DependencyProperty.Register("ShowDetails", typeof(Visibility), typeof(Player));
        public static readonly DependencyProperty HitpointsProperty = DependencyProperty.Register("Hitpoints", typeof(ObservableCollection<string>), typeof(Player));
        public static readonly DependencyProperty InventoryProperty = DependencyProperty.Register("Inventory", typeof(ObservableCollection<string>), typeof(Player));
        public static readonly DependencyProperty DamageProperty = DependencyProperty.Register("Damage", typeof(string), typeof(Player));
        public static readonly DependencyProperty FuelProperty = DependencyProperty.Register("Fuel", typeof(string), typeof(Player));
        public static readonly DependencyProperty ClassnameProperty = DependencyProperty.Register("Classname", typeof(string), typeof(Player));


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


        public Player(string className)
        {
            InitializeComponent();
            DataContext = this;

            Hitpoints = new ObservableCollection<string>();
            Inventory = new ObservableCollection<string>();
            Classname = className;
            ShowDetails = Visibility.Collapsed;
        }

        public void UpdateDetails(string inventory)
        {
            Inventory.Clear();
            ParseInventory(inventory).ForEach(x => Inventory.Add(x));
            OnPropertyChanged();
        }

        private readonly Regex _inventoryRegex = new Regex(@"""([^"",\]]+)");

        private List<string> ParseInventory(string inventory)
        {
            List<string> result = new List<string>();
            MatchCollection matches = _inventoryRegex.Matches(inventory);
            foreach (Match match in matches)
            {
                result.Add(match.Groups[1].Value);
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
