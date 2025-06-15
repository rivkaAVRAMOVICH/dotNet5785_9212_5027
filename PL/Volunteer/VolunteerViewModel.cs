//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Windows.Input;
//using BO;
//using BlApi;

//namespace PL
//{
//    public class VolunteerViewModel : INotifyPropertyChanged
//    {
//        public event PropertyChangedEventHandler PropertyChanged;

//        private BO.Volunteer _volunteer;
//        public BO.Volunteer Volunteer
//        {
//            get => _volunteer;
//            set
//            {
//                _volunteer = value;
//                OnPropertyChanged(nameof(Volunteer));
//                UpdateCommandsCanExecute();
//            }
//        }

//        private BO.Call _currentCall;
//        public BO.Call CurrentCall
//        {
//            get => _currentCall;
//            set
//            {
//                _currentCall = value;
//                OnPropertyChanged(nameof(CurrentCall));
//                UpdateCommandsCanExecute();
//            }
//        }

//        public ObservableCollection<string> DistanceTypes { get; set; } = new ObservableCollection<string> { "km", "miles" };

//        public bool CanChangeActiveStatus => CurrentCall == null;
//        public bool HasActiveCall => CurrentCall != null;
//        public bool CanChooseCall => !HasActiveCall && Volunteer?.IsActive == true;

//        // פקודות
//        public ICommand UpdateVolunteerCommand { get; }
//        public ICommand ShowCallHistoryCommand { get; }
//        public ICommand ChooseCallCommand { get; }
//        public ICommand FinishCallCommand { get; }
//        public ICommand CancelCallCommand { get; }

//        // שימוש ב־ICall מ־BL
//        private readonly ICall _callLogic;

//        public VolunteerViewModel()
//        {
//            _callLogic = Factory.Get().Call;

//            UpdateVolunteerCommand = new RelayCommand(UpdateVolunteer, () => true);
//            ShowCallHistoryCommand = new RelayCommand(ShowCallHistory, () => true);
//            ChooseCallCommand = new RelayCommand(ChooseCall, () => CanChooseCall);
//            FinishCallCommand = new RelayCommand(FinishCall, () => HasActiveCall);
//            CancelCallCommand = new RelayCommand(CancelCall, () => HasActiveCall);
//        }

//        private void UpdateCommandsCanExecute()
//        {
//            (ChooseCallCommand as RelayCommand)?.RaiseCanExecuteChanged();
//            (FinishCallCommand as RelayCommand)?.RaiseCanExecuteChanged();
//            (CancelCallCommand as RelayCommand)?.RaiseCanExecuteChanged();
//            OnPropertyChanged(nameof(CanChangeActiveStatus));
//            OnPropertyChanged(nameof(CanChooseCall));
//            OnPropertyChanged(nameof(HasActiveCall));
//        }

//        private void UpdateVolunteer()
//        {
//            // בינתיים לא נשתמש בלוגיקת מתנדבים - אפשר להשאיר ריק או להוסיף הודעה
//        }

//        private void ShowCallHistory()
//        {
//            // פתח מסך היסטוריית קריאות למתנדב
//        }

//        private void ChooseCall()
//        {
//            // פתח מסך בחירת קריאה לטיפול
//        }

//        private void FinishCall()
//        {
//            if (CurrentCall != null)
//            {
//                _callLogic.ReportCallCompletion(Volunteer.Id, CurrentCall.Id); // שימוש ב־ICall
//                CurrentCall.Status = BO.Status.closed;
//                CurrentCall = null;
//                UpdateCommandsCanExecute();
//            }
//        }

//        private void CancelCall()
//        {
//            if (CurrentCall != null)
//            {
//                _callLogic.CancelCallHandling(Volunteer.Id, CurrentCall.Id);
//                CurrentCall = null;
//                UpdateCommandsCanExecute();
//            }
//        }

//        protected void OnPropertyChanged(string propertyName) =>
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//    }
//}
