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

namespace PL.Assignment
{
    /// <summary>
    /// Interaction logic for AssignmentListWindow.xaml
    /// </summary>
    public partial class AssignmentListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.CallType SelectedAssignment { get; set; } = BO.CallType.none;
        public AssignmentListWindow()
        {
            InitializeComponent();
            this.DataContext = new
            {
                AssignmentCollection = new PL.AssignmentCollection()
            };
        }
        //private IEnumerable<BO.Assignment> _assignmentList;
        //public IEnumerable<BO.Assignment> AssignmentList
        //{
        //    get => _assignmentList;
        //    set
        //    {
        //        _assignmentList = value;
        //        OnPropertyChanged(nameof(AssignmentList));
        //    }
        //}

        private void AssignmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //var selectedAssignment = AssignmentComboBox.SelectedItem;
             var AssignmentList = (SelectedAssignment == BO.CallType.none)
       ? s_bl?.Call.GetCallsList()!
       : s_bl?.Call.GetCallsList(null, BO.CallType.cooking, SelectedAssignment)!;

        }

    }
}
