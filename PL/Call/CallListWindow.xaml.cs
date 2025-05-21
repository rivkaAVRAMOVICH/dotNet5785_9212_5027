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
        public CallListWindow()
        {
            InitializeComponent();
            this.DataContext = new
            {
                CallCollection = new PL.Enums.CallCollection()
            };
        }

        private void CallComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCall = CallComboBox.SelectedItem;
        }

    }
}
