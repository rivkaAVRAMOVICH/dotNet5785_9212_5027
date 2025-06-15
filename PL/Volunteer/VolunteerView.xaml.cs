using System.Windows;

namespace PL.Volunteer
{
    public partial class VolunteerView : Window
    {
        public VolunteerView()
        {
            InitializeComponent();
        }
        public VolunteerViewModel VolunteerViewModel { get; set; } = new VolunteerViewModel();
    }
}
