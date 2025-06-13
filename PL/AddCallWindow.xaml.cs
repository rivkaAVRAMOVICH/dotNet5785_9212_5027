using BO;
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

namespace PL
{
    /// <summary>
    /// Interaction logic for AddCallWindow.xaml
    /// </summary>
    public partial class AddCallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public AddCallWindow()
        {
            InitializeComponent();

            // מילוי ComboBoxes מה־Enums
            if (this.Resources["CallTypeCollectionKey"] is CollectionViewSource callTypeSource)
                callTypeSource.Source = Enum.GetValues(typeof(CallType));


            // יצירת אובייקט חדש
           Call= new BO.Call
            {
               CallDescription = string.Empty,
               CallAddress = string.Empty,
               MaxEndCallTime = null,
               CallType = CallType.none,
            };

            this.DataContext = this;
        }

        public BO.Call Call
        {
            get => (BO.Call)GetValue(CallProperty);
            set => SetValue(CallProperty, value);
        }

        public static readonly DependencyProperty CallProperty =
            DependencyProperty.Register("Call", typeof(BO.Call), typeof(AddCallWindow));

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Call.AddingCall(Call);
                MessageBox.Show("Call added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
