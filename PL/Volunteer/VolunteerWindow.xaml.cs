using BO;
using DO;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PL.Volunteer
{
    public partial class VolunteerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public VolunteerWindow(int id = 0)
        {
            InitializeComponent();

            // מילוי ה־CollectionViewSource מתוך Enums
            if (this.Resources["RolesCollectionKey"] is CollectionViewSource rolesSource)
                rolesSource.Source = Enum.GetValues(typeof(Role));

            if (this.Resources["DistanceTypesCollectionKey"] is CollectionViewSource distanceSource)
                distanceSource.Source = Enum.GetValues(typeof(TypeOfDistance));

            // קביעת מצב הטופס לפי ID
            ButtonText = id == 0 ? "Add" : "Update";

            Volunteer = id == 0
                ? new BO.Volunteer
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
                }
                : s_bl.Volunteer.GetVolunteerDetails(id);

            this.DataContext = this;

            // הוספת Observer רק אם מדובר בעדכון
            if (Volunteer.Id != 0)
            {
                this.Loaded += (s, e) =>
                {
                    s_bl.Volunteer.AddObserver(Volunteer.Id, VolunteerObserver);
                };

                this.Closed += (s, e) =>
                {
                    s_bl.Volunteer.RemoveObserver(Volunteer.Id, VolunteerObserver);
                };
            }
            IsUpdateMode = id != 0;
        }

        public BO.Volunteer Volunteer
        {
            get => (BO.Volunteer)GetValue(VolunteerProperty);
            set => SetValue(VolunteerProperty, value);
        }
        public static readonly DependencyProperty VolunteerProperty =
            DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(VolunteerWindow));

        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow));

        private void VolunteerObserver()
        {
            int id = Volunteer.Id;
            Volunteer = null;
            Volunteer = s_bl.Volunteer.GetVolunteerDetails(id);
        }
        public bool IsUpdateMode
        {
            get => (bool)GetValue(IsUpdateModeProperty);
            set => SetValue(IsUpdateModeProperty, value);
        }

        public static readonly DependencyProperty IsUpdateModeProperty =
            DependencyProperty.Register("IsUpdateMode", typeof(bool), typeof(VolunteerWindow));


        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                {
                    s_bl.Volunteer.AddingVolunteer(Volunteer);
                    MessageBox.Show("Volunteer added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (DataContext == null)
                    {
                        MessageBox.Show("DataContext is null!");
                        return;
                    }

                    if (s_bl.Volunteer == null)
                    {
                        MessageBox.Show("volunteer is null!");
                        return;
                    }
                    s_bl.Volunteer.UpdateVolunteerDetails(Volunteer.Id, Volunteer);
                    MessageBox.Show("Volunteer updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
