using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.CallType SelectedVolunteer { get; set; } = BO.CallType.none;
        public BO.VolunteerInList? SelectedVolunteerInList { get; set; }

        public VolunteerListWindow()
        {
            InitializeComponent();
            this.DataContext = new
            {
                VolunteerCollection = new PL.VolunteerCollection()
            };
        }
        private void lsvVolunteersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteerInList != null)
            {
                new VolunteerWindow(SelectedVolunteerInList.Id).Show();
            }
        }

        public static readonly DependencyProperty VolunteerListProperty =
    DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));
        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }
        private void VolunteerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var selectedVolunteer = VolunteerComboBox.SelectedItem;
           VolunteerList = (SelectedVolunteer == BO.CallType.none)
      ? s_bl?.Volunteer.GetVolunteersList()!
      : s_bl?.Volunteer.GetVolunteersList(null, SelectedVolunteer)!;
        }
        private void queryVolunteerList()
        {
            VolunteerList = (SelectedVolunteer == BO.CallType.none)
  ? s_bl?.Volunteer.GetVolunteersList()!
  : s_bl?.Volunteer.GetVolunteersList(null, SelectedVolunteer)!;
        }
        private void volunteerListObserver() => queryVolunteerList();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(volunteerListObserver);
            queryVolunteerList();
        }
        private void Window_Closed(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(volunteerListObserver);
            queryVolunteerList();
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow(0).Show();
        }
    }
   
    }
