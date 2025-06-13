using BO;
using System;
using System.Windows;
using System.Windows.Data;

namespace PL
{
    public partial class VolunteerManagementWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public VolunteerManagementWindow(int volunteerId)
        {
            InitializeComponent();

            // מילוי ComboBoxes עם ערכי ה־Enums שכבר קיימים
            if (Resources["RolesCollectionKey"] is CollectionViewSource rolesSource)
                rolesSource.Source = Enum.GetValues(typeof(Role));
            if (Resources["DistanceTypesCollectionKey"] is CollectionViewSource distanceSource)
                distanceSource.Source = Enum.GetValues(typeof(TypeOfDistance));

            // שליפת המתנדב לפי ID
            Volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);

            if (Volunteer == null)
            {
                MessageBox.Show("Volunteer not found");
                Close(); // או זרוק חריגה, או משהו אחר בהתאם להקשר
                return;
            }
            IsUpdateMode = true;
            ButtonText = "Update";

            DataContext = this;

            Loaded += (_, __) => s_bl.Volunteer.AddObserver(Volunteer.Id, VolunteerObserver);
            Closed += (_, __) => s_bl.Volunteer.RemoveObserver(Volunteer.Id, VolunteerObserver);
        }

        public BO.Volunteer Volunteer
        {
            get => (BO.Volunteer)GetValue(VolunteerProperty);
            set => SetValue(VolunteerProperty, value);
        }
        public static readonly DependencyProperty VolunteerProperty =
            DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(VolunteerManagementWindow));

        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerManagementWindow));

        public bool IsUpdateMode
        {
            get => (bool)GetValue(IsUpdateModeProperty);
            set => SetValue(IsUpdateModeProperty, value);
        }
        public static readonly DependencyProperty IsUpdateModeProperty =
            DependencyProperty.Register("IsUpdateMode", typeof(bool), typeof(VolunteerManagementWindow));

        private void VolunteerObserver()
        {
            int id = Volunteer.Id;
            Volunteer = s_bl.Volunteer.GetVolunteerDetails(id);
        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Volunteer.UpdateVolunteerDetails(Volunteer.Id, Volunteer);
                MessageBox.Show("Volunteer updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
