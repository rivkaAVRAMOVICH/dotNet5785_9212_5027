using BlApi;
using BO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace PL.Volunteer
{
    public class VolunteerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly ICall _callLogic;

        public VolunteerViewModel()
        {
            _callLogic = Factory.Get().Call;

            UpdateVolunteerCommand = new RelayCommand(UpdateVolunteer, () => true);
            ShowCallHistoryCommand = new RelayCommand(ShowCallHistory, () => true);
            ChooseCallCommand = new RelayCommand(ChooseCall, () => CanChooseCall);
            FinishCallCommand = new RelayCommand(FinishCall, () => HasActiveCall);
            CancelCallCommand = new RelayCommand(CancelCall, () => HasActiveCall);
        }

        private BO.Volunteer? _volunteer;
        public BO.Volunteer? Volunteer
        {
            get => _volunteer;
            set
            {
                _volunteer = value;
                OnPropertyChanged(nameof(Volunteer));
                UpdateCommandsCanExecute();
            }
        }

        private BO.Call? _currentCall;
        public BO.Call? CurrentCall
        {
            get => _currentCall;
            set
            {
                _currentCall = value;
                OnPropertyChanged(nameof(CurrentCall));
                UpdateCommandsCanExecute();
            }
        }

        public ObservableCollection<string> DistanceTypes { get; set; } = new ObservableCollection<string> { "km", "miles" };

        public bool CanChangeActiveStatus => CurrentCall == null;
        public bool HasActiveCall => CurrentCall != null;
        public bool CanChooseCall => !HasActiveCall && Volunteer?.IsActive == true;

        public ICommand UpdateVolunteerCommand { get; }
        public ICommand ShowCallHistoryCommand { get; }
        public ICommand ChooseCallCommand { get; }
        public ICommand FinishCallCommand { get; }
        public ICommand CancelCallCommand { get; }

        private void UpdateCommandsCanExecute()
        {
            (ChooseCallCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (FinishCallCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (CancelCallCommand as RelayCommand)?.RaiseCanExecuteChanged();

            OnPropertyChanged(nameof(CanChangeActiveStatus));
            OnPropertyChanged(nameof(HasActiveCall));
            OnPropertyChanged(nameof(CanChooseCall));
        }

        private void UpdateVolunteer()
        {
        }

        private void ShowCallHistory()
        {
            new CallHistoryView(Volunteer.Id).Show();
        }

        private void ChooseCall()
        {
            System.Windows.MessageBox.Show("מסך בחירת קריאה עדיין לא ממומש.", "בקרוב", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void FinishCall()
        {
            if (CurrentCall != null)
            {
                _callLogic.ReportCallCompletion(Volunteer.Id, CurrentCall.Id);
                CurrentCall.Status = Status.closed;
                CurrentCall = null;
            }
        }

        private void CancelCall()
        {
            if (CurrentCall != null)
            {
                _callLogic.CancelCallHandling(Volunteer.Id, CurrentCall.Id);
                CurrentCall = null;
            }
        }

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
