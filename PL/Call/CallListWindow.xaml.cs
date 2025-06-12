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

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallListWindow.xaml
    /// </summary>
    public partial class CallListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
     
        public BO.CallType SelectedCall { get; set; } = BO.CallType.none;
        public CallListWindow(string? statusName = null)
        {
            InitializeComponent();
            this.DataContext = new
            {
                CallCollection = new PL.CallCollection()
            };
        }

        private void CallComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var selectedCall = CallComboBox.SelectedItem;
            var CallList = (SelectedCall == BO.CallType.none)
? s_bl?.Call.GetCallsList()!
: s_bl?.Call.GetCallsList(null, BO.CallType.cooking, SelectedCall)!;
        }

    }
}
