using PL.Volunteer;
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

namespace PL
{
    /// <summary>
    /// Interaction logic for VolunteersManagementWindow.xaml
    /// </summary>
    public partial class VolunteersManagementWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile bool _volunteerListObserverWorking = false; //stage 7
        public BO.CallType SelectedVolunteer { get; set; } = BO.CallType.none;
        public BO.VolunteerInList? SelectedVolunteerInList { get; set; }

        public VolunteersManagementWindow()
        {
            InitializeComponent();
        }
        private void lsvVolunteersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteerInList != null)
            {
                new VolunteerManagementWindow(SelectedVolunteerInList.Id).Show();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int volunteerId)
            {
                // 1. בדיקת אישור מחיקה
                var result = MessageBox.Show($"Are you sure you want to delete volunteer with ID {volunteerId}?",
                                             "Confirm Delete",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // 2. קריאה למחיקה בשכבת BL
                        s_bl.Volunteer.DeletingVolunteer(volunteerId);

                        // 3. הרענון מתבצע אוטומטית בזכות המנגנון של ה-Observer שכבר קיים
                        MessageBox.Show("Volunteer deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        // 4. טיפול בחריגה והצגת הודעת שגיאה
                        MessageBox.Show($"Failed to delete volunteer:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public static readonly DependencyProperty VolunteerListProperty =
    DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteersManagementWindow), new PropertyMetadata(null));
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
        private void volunteerListObserver()
        {
            if (!_volunteerListObserverWorking)
            {
                _volunteerListObserverWorking = true;
                _ = Dispatcher.BeginInvoke(() =>
                {
                    queryVolunteerList();
                    _volunteerListObserverWorking = false;
                });
            }
        }
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
            new AddVolunteerWindow().Show();
        }
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
