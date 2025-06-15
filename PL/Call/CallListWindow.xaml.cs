using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallListWindow.xaml
    /// </summary>
    public partial class CallListWindow : Window
    {
            static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

            public CallListWindow(string? statusName = null)
            {
                InitializeComponent();

                // שלב 1 - נביא את כל הקריאות
                IEnumerable<BO.CallInList> allCalls = s_bl.Call.GetCallsList();
       
            // שלב 2 - נסנן אם צריך
            IEnumerable<BO.CallInList> callsToDisplay = allCalls;

         
            if (!string.IsNullOrEmpty(statusName))
                {
                    callsToDisplay = allCalls.Where(call => call.Status.ToString() == statusName);
                }

                // שלב 3 - נציג במסך
                CallsListView.ItemsSource = callsToDisplay;
            }
        }

    }
