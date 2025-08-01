﻿using PL.Assignment;
using PL.Call;
using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainAdminWindow.xaml
    /// </summary>
    public partial class MainAdminWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile bool _adminClockObserverWorking = false; //stage 7
        private volatile bool _adminConfigObserverWorking = false; //stage 7
        DispatcherTimer simulatorTimer = new DispatcherTimer();
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly DispatcherTimer clockTimer = new DispatcherTimer();
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public MainAdminWindow()
        {

            InitializeComponent();
            DataContext = this;
            //Start updating time
            //DispatcherTimer timer = new DispatcherTimer();
            clockTimer.Interval = TimeSpan.FromSeconds(1);
            clockTimer.Tick += (s, e) =>
            {
                CurrentTime = s_bl.Admin.GetClock().ToString("HH:mm:ss dd/MM/yyyy");
            };
            clockTimer.Start();
            this.Loaded += MainAdminWindow_Loaded;
            simulatorTimer.Interval = TimeSpan.FromMilliseconds(1000);
            simulatorTimer.Tick += SimulatorTimer_Tick;

            //Observers
            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(ConfigObserver);

            MaxYearRange = s_bl.Admin.GetRiskTimeRange();
        }
        private string currentTime;
        public string CurrentTime
        {
            get => currentTime;
            set
            {
                currentTime = value;
                OnPropertyChanged(nameof(CurrentTime));
            }
        }
        private void SimulatorTimer_Tick(object sender, EventArgs e)
        {
            // כאן לדוגמה, נקדם את השעון בדקה
            btnAddOneMinute_Click(null, null);
        }

        // לחצן הפעלה/כיבוי
        private void btnToggleSimulator_Click(object sender, RoutedEventArgs e)
        {
            if (IsSimulatorRunning)
            {
                simulatorTimer.Stop();
                IsSimulatorRunning = false;
                btnToggleSimulator.Content = "Start Simulator";
            }
            else
            {
                simulatorTimer.Start();
                IsSimulatorRunning = true;
                btnToggleSimulator.Content = "Stop Simulator";
            }
        }

        // לחצן לשינוי מהירות הסימולטור
        private void btnSetSimulatorSpeed_Click(object sender, RoutedEventArgs e)
        {
            if (Interval > 0)
            {
                simulatorTimer.Interval = TimeSpan.FromMilliseconds(Interval);
                MessageBox.Show($"Simulator speed updated to {Interval} ms", "Simulator", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please enter a valid positive number (in milliseconds).", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }



        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.MINUTE);
            CurrentTime = s_bl.Admin.GetClock().ToString("HH:mm:ss dd/MM/yyyy");
        }

        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.HOUR);
            CurrentTime = s_bl.Admin.GetClock().ToString("HH:mm:ss dd/MM/yyyy");
        }

        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.DAY);
            CurrentTime = s_bl.Admin.GetClock().ToString("HH:mm:ss dd/MM/yyyy");
        }

        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.MONTH);
            CurrentTime = s_bl.Admin.GetClock().ToString("HH:mm:ss dd/MM/yyyy");

        }

        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.YEAR);
            CurrentTime = s_bl.Admin.GetClock().ToString("HH:mm:ss dd/MM/yyyy");
        }

        public TimeSpan MaxYearRange
        {
            get { return (TimeSpan)GetValue(MaxYearRangeProperty); }
            set { SetValue(MaxYearRangeProperty, value); }
        }

        public static readonly DependencyProperty MaxYearRangeProperty =
            DependencyProperty.Register("MaxYearRange", typeof(TimeSpan), typeof(MainAdminWindow));

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
            if (!_adminClockObserverWorking)
            {
                _adminClockObserverWorking = true;
                _ = Dispatcher.BeginInvoke(() =>
                {
                    CurrentTime = CurrentTime = s_bl.Admin.GetClock().ToString("HH:mm:ss dd/MM/yyyy");
                    _adminClockObserverWorking = false;
                });
            }
          
        }

        private void ConfigObserver()
        {
            if (!_adminConfigObserverWorking)
            {
                _adminConfigObserverWorking = true;
                _ = Dispatcher.BeginInvoke(() =>
                {
                    MaxYearRange = s_bl.Admin.GetRiskTimeRange();
                    _adminConfigObserverWorking = false;
                });
            }
       
        }
        public ObservableCollection<StatusCount> CallsByStatus { get; set; }

        private void LoadStatusCounts()
        {
            CallsByStatus = new ObservableCollection<StatusCount>();
            OnPropertyChanged(nameof(CallsByStatus)); // <<< שורה חשובה!!

            string[] statusNames = Enum.GetNames(typeof(BO.Status));
            int[] counts = s_bl.Call.RequestCallsQuantities();

            for (int i = 0; i < statusNames.Length; i++)
            {
                CallsByStatus.Add(new StatusCount
                {
                    Status = statusNames[i],
                    Count = counts[i]
                });
            }
        }

        private void CallStatusButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is string statusName)
            {
                // נניח ש-CallListWindow מקבל enum או string עם הסטטוס
                var window = new CallsManagementWindow(statusName);
                window.Show();
            }
        }
        private void MainAdminWindow_Loaded(object sender, RoutedEventArgs e)
        {

           CurrentTime = CurrentTime = s_bl.Admin.GetClock().ToString("HH:mm:ss dd/MM/yyyy");
            MaxYearRange = s_bl.Admin.GetRiskTimeRange();
            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(ConfigObserver);
            LoadStatusCounts();
        }

        private void MainAdminWindow_Closed(object sender, EventArgs e)
        {
            if (IsSimulatorRunning)
            {
              simulatorTimer.Stop();
                IsSimulatorRunning = false;
            }

            s_bl.Admin.RemoveClockObserver(ClockObserver);
            s_bl.Admin.RemoveConfigObserver(ConfigObserver);
        }

        private void btnAssignment_Click(object sender, RoutedEventArgs e)
        {
            new VolunteersManagementWindow().Show();
        }

        private void btnCall_Click(object sender, RoutedEventArgs e)
        {
            new CallsManagementWindow().Show();
        }
        private void btnClockInitialization_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.GetClock();
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
        private void btnShowCallStats_Click(object sender, RoutedEventArgs e)
        {
            new CallsManagementWindow().Show();
           
        }

        public int Interval
        {
            get { return (int)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(MainAdminWindow), new PropertyMetadata(1));

        public bool IsSimulatorRunning
        {
            get { return (bool)GetValue(IsSimulatorRunningProperty); }
            set { SetValue(IsSimulatorRunningProperty, value); }
        }

        public static readonly DependencyProperty IsSimulatorRunningProperty =
            DependencyProperty.Register("IsSimulatorRunning", typeof(bool), typeof(MainAdminWindow), new PropertyMetadata(false));

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
        private void btnResetConfiguration_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to reset the all Configuration? This action will reset all data",
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
                    CurrentTime = CurrentTime = s_bl.Admin.GetClock().ToString("HH:mm:ss dd/MM/yyyy");
                    MaxYearRange = s_bl.Admin.GetRiskTimeRange();
                    MessageBox.Show("The Configuration was reset successfully", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
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
        public class StatusCount
        {
            public string Status { get; set; }
            public int Count { get; set; }
        }


    }
}
