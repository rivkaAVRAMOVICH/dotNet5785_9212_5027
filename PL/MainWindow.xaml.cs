using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using PL.Assignment;
using PL.Call;
using PL.Volunteer;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            //Start updating time
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => CurrentTime = s_bl.Admin.GetClock();
            timer.Start();
            this.Loaded += MainWindow_Loaded;

            //Observers
            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(ConfigObserver);

            MaxYearRange = s_bl.Admin.GetRiskTimeRange();
        }
        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.MINUTE);
            CurrentTime = s_bl.Admin.GetClock();
        }

        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.HOUR);
            CurrentTime = s_bl.Admin.GetClock();
        }

        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.DAY);
            CurrentTime = s_bl.Admin.GetClock();
        }

        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.MONTH);
            CurrentTime = s_bl.Admin.GetClock();
        }

        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.YEAR);
            CurrentTime = s_bl.Admin.GetClock();
        }

        public TimeSpan MaxYearRange
        {
            get { return (TimeSpan)GetValue(MaxYearRangeProperty); }
            set { SetValue(MaxYearRangeProperty, value); }
        }

        public static readonly DependencyProperty MaxYearRangeProperty =
            DependencyProperty.Register("MaxYearRange", typeof(TimeSpan), typeof(MainWindow));

        private void btnUpdateMaxRange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.SetRiskTimeRange(MaxYearRange);
                MessageBox.Show("The value updated", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error was occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClockObserver()
        {
            CurrentTime = s_bl.Admin.GetClock();
        }

        private void ConfigObserver()
        {
            MaxYearRange = s_bl.Admin.GetRiskTimeRange();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetClock();
            MaxYearRange = s_bl.Admin.GetRiskTimeRange();
            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(ConfigObserver);
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(ClockObserver);
            s_bl.Admin.RemoveConfigObserver(ConfigObserver);
        }
        private void btnAssignment_Click(object sender, RoutedEventArgs e)
        {
            new AssignmentListWindow().Show();
        }

        private void btnCall_Click(object sender, RoutedEventArgs e)
        {
            new CallListWindow().Show();
        }


        private void btnVolunteer_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerListWindow().Show();
        }

        private void btnInitializeDB_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want initialize the database? This action will overwrite all data.",
                                                      "Confirm initialization", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    // לוודא שה-UI מעודכן לפני הפעולה הכבדה
                    Dispatcher.Invoke(() => { }, DispatcherPriority.Render);

                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window != this)
                            window.Close();
                    }

                    s_bl.Admin.InitializeDatabase();

                    MessageBox.Show("The database was update successfully", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error was occurred when initialize database:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }

        private void btnResetDB_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to reset the database? This action will reset all data",
                                                      "Confirm reset", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    Dispatcher.Invoke(() => { }, DispatcherPriority.Render);

                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window != this)
                            window.Close();
                    }

                    s_bl.Admin.ResetDatabase();

                    MessageBox.Show("The database was reset successfully", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error was occurred when reset data:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }

        //public IEnumerable<BO.CallInList> AssignmentList
        //{
        //    get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
        //    set { SetValue(CallListProperty, value); }
        //}

        //public static readonly DependencyProperty CallListProperty =
        //    DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>),
        //                                typeof(CallListWindow), new PropertyMetadata(null));

        //public IEnumerable<BO.VolunteerInList> VolunteerList
        //{
        //    get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
        //    set { SetValue(VolunteerListProperty, value); }
        //}

        //public static readonly DependencyProperty VolunteerListProperty =
        //    DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>),
        //                                typeof(VolunteerListWindow), new PropertyMetadata(null));
    }
}