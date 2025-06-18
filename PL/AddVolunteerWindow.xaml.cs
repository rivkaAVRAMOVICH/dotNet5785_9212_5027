using BO;
using System;
using System.Windows;
using System.Windows.Data;

namespace PL
{
    public partial class AddVolunteerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public AddVolunteerWindow()
        {
            InitializeComponent();

            // מילוי ComboBoxes מה־Enums
            if (this.Resources["RolesCollectionKey"] is CollectionViewSource rolesSource)
                rolesSource.Source = Enum.GetValues(typeof(Role));

            if (this.Resources["DistanceTypesCollectionKey"] is CollectionViewSource distanceSource)
                distanceSource.Source = Enum.GetValues(typeof(TypeOfDistance));

            // יצירת אובייקט מתנדב חדש
            Volunteer = new BO.Volunteer
            {
                Id = 0,
                Name = string.Empty,
                Phone = string.Empty,
                Email = string.Empty,
                Password = string.Empty,
                Address = string.Empty,
                Latitude = null,
                Longitude = null,
                Role = Role.None,
                IsActive = true,
                MaxDistance = 0,
                TypeOfDistance = TypeOfDistance.None,
                SumHandledCalls = 0,
                SumCanceledCalls = 0,
                SumChosenExpiredCalls = 0,
                CallInVolunteerHandle = null
            };

            this.DataContext = this;
        }

        public BO.Volunteer Volunteer
        {
            get => (BO.Volunteer)GetValue(VolunteerProperty);
            set => SetValue(VolunteerProperty, value);
        }

        public static readonly DependencyProperty VolunteerProperty =
            DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(AddVolunteerWindow));

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Volunteer.AddingVolunteer(Volunteer);
                MessageBox.Show("Volunteer added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
