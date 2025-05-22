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
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.Volunteer Volunteer
        {
            get { return (BO.Volunteer)GetValue(VolunteerProperty); }
            set { SetValue(VolunteerProperty, value); }
        }

        public static readonly DependencyProperty VolunteerProperty =
            DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(VolunteerWindow));

        // תכונת תלות עבור טקסט הכפתור
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow));
        private void VolunteerObserver()
        {
            int id = Volunteer!.Id;
Volunteer = null;
            Volunteer = s_bl.Volunteer.GetVolunteerDetails(id);

        }
        public VolunteerWindow(int id=0)
        {
            InitializeComponent();
            ButtonText = id == 0 ? "Add" : "Update";

            Volunteer = ButtonText== "Update"
    ? s_bl.Volunteer.GetVolunteerDetails(id)
    : new BO.Volunteer
    {
        Id = 0,
        Name = string.Empty,
        Phone = string.Empty,
        Email = string.Empty,
        Password = string.Empty,
        Address = string.Empty,
        Latitude = null,
        Longitude = null,
        Role = BO.Role.None,
        IsActive = true,
        MaxDistance = 0,
        TypeOfDistance = BO.TypeOfDistance.None,
        SumHandledCalls = 0,
        SumCanceledCalls = 0,
        SumChosenExpiredCalls = 0,
        CallInVolunteerHandle = null
    };
            if (Volunteer!.Id != 0)
            {
                    this.Loaded += (s, e) =>
                {
                    //_refreshAction = RefreshVolunteer;
                    s_bl.Volunteer.AddObserver(Volunteer.Id, VolunteerObserver);
                };

                this.Closed += (s, e) =>
                {
                    //if (_refreshAction != null)
                        s_bl.Volunteer.RemoveObserver(Volunteer.Id, VolunteerObserver);
                };
            }



            // טיפול בלחיצה על כפתור Add/Update



        }
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
