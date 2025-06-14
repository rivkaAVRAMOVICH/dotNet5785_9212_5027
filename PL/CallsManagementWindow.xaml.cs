using BO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for CallManagementWindow.xaml
    /// </summary>
    public partial class CallsManagementWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public ObservableCollection<CallInList> Calls { get; set; } = new();

        public CallsManagementWindow()
        {
            InitializeComponent();
            DataContext = this;

            LoadCalls();
        }

        private void LoadCalls()
        {
            var calls = s_bl.Call.GetCallsList()
                .Select(c => new CallInList
                {
                    Id = c.Id,
                    CallId = c.CallId,
                    CallType = c.CallType,
                    StartCallTime = c.StartCallTime,
                    EndCallTimeSpan = c.EndCallTimeSpan,
                    LastVolunteerName = c.LastVolunteerName,
                    CompleteTreatmentTimeSpan = c.CompleteTreatmentTimeSpan,
                    Status = c.Status,
                    AssignSum = c.AssignSum
                   
                });

            Calls.Clear();
            foreach (var call in calls)
                Calls.Add(call);
        }

        private void AddCall_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddCallWindow();
            addWindow.ShowDialog();
            LoadCalls();
        }

        private void CallsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CallsDataGrid.SelectedItem is CallInList selectedCall)
            {
                var manageWindow = new ManageSingleCallWindow(selectedCall.CallId);
                manageWindow.ShowDialog();
                LoadCalls();
            }
        }

        private void DeleteCall_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is CallInList call)
            {
                if (call.Status == Status.open && call.AssignSum == 0)
                {
                    MessageBoxResult result = MessageBox.Show("האם אתה בטוח שברצונך למחוק את הקריאה?", "אישור מחיקה", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        s_bl.Call.DeletingCall(call.CallId);
                        LoadCalls();
                    }
                }
            }
        }

        private void UnassignCall_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is CallInList call)
            {
                if (call.Status == Status.inProgress || call.Status == Status.inProgressAtRisk)
                {
                    //BO.Call callDetail = s_bl.Call.GetCallsDetails(call.CallId);
            
                    //s_bl.Call.CancelCallHandling(call.CallId);
                    //MessageBox.Show("ההקצאה בוטלה ואימייל נשלח למתנדב.");
                    //LoadCalls();
                }
            }
        }
        private void CallsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // אם את לא צריכה אותה כרגע, אפשר להשאיר ריקה או להסיר לחלוטין מה־XAML
        }
    }
}
