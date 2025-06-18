using System.Windows;

namespace PL.Volunteer
{
    public partial class CallHistoryView : Window
    {
        public CallHistoryView(int volunteerId)
        {
            InitializeComponent();
            DataContext = new CallHistoryViewModel(volunteerId);
        }
        public CallHistoryView() : this(0) { }
    }
}