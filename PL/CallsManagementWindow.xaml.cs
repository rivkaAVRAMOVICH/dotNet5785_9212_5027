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
        private readonly string? statusName;
        public ObservableCollection<CallInList> Calls { get; set; } = new();

        public CallsManagementWindow(string? _statusName = null)
        {
            InitializeComponent();
            DataContext = this;
            statusName= _statusName;
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
                if (!string.IsNullOrEmpty(statusName))
                {
                    if(call.Status.ToString() == statusName)
                        Calls.Add(call);
                }
                else
                {
                   Calls.Add(call);
                }
               
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
                if (call.Status != Status.open && call.AssignSum == 0)
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
            if (sender is Button btn && btn.DataContext is CallInList call)
            {
                var result = MessageBox.Show("Cancel the current assignment for this call?", "Cancel Assignment", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // call.Id represents the assignment ID
                        s_bl.Call.CancelCallHandling(call.CallId, call.Id ?? throw new InvalidOperationException("Assignment ID is required."));
                        LoadCalls();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error while canceling assignment:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

        }
     
    }
}
