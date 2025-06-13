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
    /// Interaction logic for ManageSingleCallWindow.xaml
    /// </summary>using BO;


        public partial class ManageSingleCallWindow : Window
        {
            static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

            public  BO.Call CurrentCall { get; private set; }

            public ObservableCollection<BO.CallAssignInList> Assignments { get; set; } = new();
        public ManageSingleCallWindow(int callId)
            {
                InitializeComponent();
            if (Resources["CallTypeCollectionKey"] is CollectionViewSource callTypeSource)
                callTypeSource.Source = Enum.GetValues(typeof(BO.CallType));
            this.DataContext = this;
            LoadCall(callId);
        }

            private void LoadCall(int callId)
            {
                try
                {
                    CurrentCall = s_bl.Call.GetCallsDetails(callId);

                    // עדכון שדות במסך לפי הקריאה
                    //CallTypeComboBox.ItemsSource = Enum.GetValues(typeof(CallType));
                    //CallTypeComboBox.SelectedItem = CurrentCall.CallType;

                    //DescriptionTextBox.Text = CurrentCall.CallDescription;
                    //AddressTextBox.Text = CurrentCall.CallAddress;
                //UrgencyTextBox.Text = CurrentCall.Urgency.ToString();
                EndTimePicker.Text = (CurrentCall.MaxEndCallTime ?? DateTime.Now.Date).ToString("HH:mm");


                //StatusTextBlock.Text = CurrentCall.Status.ToString();
                //    StartTimeTextBlock.Text = CurrentCall.StartCallTime.ToString("g");

                    // תצוגת הקצאות
                    Assignments.Clear();
                    foreach (var assign in CurrentCall.CallAssignList)
                        Assignments.Add(assign);

                    // ניהול זמינות עריכה לפי סטטוס
                    UpdateEditabilityByStatus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"שגיאה בטעינת הקריאה: {ex.Message}");
                    this.Close();
                }
            }

            private void UpdateEditabilityByStatus()
            {
                var editableAll = CurrentCall.Status == BO.Status.open || CurrentCall.Status ==BO.Status.openAtRisk;
                var editableEndTimeOnly = CurrentCall.Status == BO.Status.inProgress || CurrentCall.Status ==BO. Status.inProgressAtRisk;

                //DescriptionTextBox.IsReadOnly = !editableAll;
                //AddressTextBox.IsReadOnly = !editableAll;
                //UrgencyTextBox.IsReadOnly = !editableAll;
                //CallTypeComboBox.IsEnabled = editableAll;

                EndTimePicker.IsEnabled = editableAll || editableEndTimeOnly;

                UpdateButton.IsEnabled = editableAll || editableEndTimeOnly;
            }

            private void UpdateButton_Click(object sender, RoutedEventArgs e)
            {
                try
                {
                    // בדיקות בסיסיות
                    //if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
                    //{
                    //    MessageBox.Show("יש להזין תיאור.");
                    //    return;
                    //}

                    //if (string.IsNullOrWhiteSpace(AddressTextBox.Text))
                    //{
                    //    MessageBox.Show("יש להזין כתובת.");
                    //    return;
                    //}

                    //if (!int.TryParse(UrgencyTextBox.Text, out int urgency))
                    //{
                    //    MessageBox.Show("הדחיפות צריכה להיות מספר.");
                    //    return;
                    //}

                    if (EndTimePicker.Text is null)
                    {
                        MessageBox.Show("יש לבחור זמן לסיום.");
                        return;
                    }

                    // עדכון הקריאה
                    //CurrentCall.CallDescription = DescriptionTextBox.Text;
                    //CurrentCall.CallAddress = AddressTextBox.Text;
                    //CurrentCall.MaxEndCallTime = urgency;
                    //CurrentCall.CallType = (CallType)CallTypeComboBox.SelectedItem;
                CurrentCall.MaxEndCallTime = DateTime.Parse(EndTimePicker.Text);

                s_bl.Call.UpdateCallDetails(CurrentCall);
                    MessageBox.Show("הקריאה עודכנה בהצלחה.");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"שגיאה בעדכון הקריאה: {ex.Message}");
                }
            }

            private void CancelButton_Click(object sender, RoutedEventArgs e)
            {
                this.Close();
            }
        }
    }


