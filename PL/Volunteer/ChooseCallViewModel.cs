using BlApi;
using BO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PL.Volunteer
{
    public class ChooseCallViewModel : INotifyPropertyChanged
    {
        private readonly IBl bl;

        public ChooseCallViewModel(IBl blInstance, BO.Volunteer volunteer)
        {
            bl = blInstance;
            Volunteer = volunteer;
            VolunteerLatitude = volunteer.Latitude;
            VolunteerLongitude = volunteer.Longitude;

            AllCalls = new ObservableCollection<OpenCallInList>(bl.Call.GetOpenCallsForVolunteer(volunteer.Id));
            CallTypes = new ObservableCollection<string>(AllCalls.Select(c => c.Type).Distinct());

            UpdateFilteredCalls();

            UpdateAddressCommand = new RelayCommand(UpdateAddress);
            ChooseCallCommand = new RelayCommand(ChooseCall, CanChooseCall);
        }

        public BO.Volunteer Volunteer { get; }

        public double? VolunteerLatitude { get; set; }
        public double? VolunteerLongitude { get; set; }

        public ObservableCollection<OpenCallInList> AllCalls { get; set; }

        private ObservableCollection<OpenCallInList> filteredCalls;
        public ObservableCollection<OpenCallInList> FilteredCalls
        {
            get => filteredCalls;
            set { filteredCalls = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> CallTypes { get; set; }

        private string selectedCallType;
        public string SelectedCallType
        {
            get => selectedCallType;
            set { selectedCallType = value; OnPropertyChanged(); UpdateFilteredCalls(); }
        }

        private bool isDistanceAscending;
        public bool IsDistanceAscending
        {
            get => isDistanceAscending;
            set { isDistanceAscending = value; OnPropertyChanged(); UpdateFilteredCalls(); }
        }

        private OpenCallInList selectedCall;
        public OpenCallInList SelectedCall
        {
            get => selectedCall;
            set { selectedCall = value; OnPropertyChanged(); }
        }

        public ICommand UpdateAddressCommand { get; }
        public ICommand ChooseCallCommand { get; }

        private void UpdateAddress()
        {
            AllCalls = new ObservableCollection<OpenCallInList>(bl.Call.GetOpenCallsForVolunteer(Volunteer.Id));
            UpdateFilteredCalls();
        }

        private bool CanChooseCall(object parameter)
        {
            return parameter is OpenCallInList && Volunteer.Id == null;
        }

        private void ChooseCall(object parameter)
        {
            if (parameter is OpenCallInList call)
            {
                bl.Call.AssignCallToVolunteer(Volunteer.Id, call.Id);
                AllCalls.Remove(call);
                UpdateFilteredCalls();
                SelectedCall = null;
            }
        }

        private void UpdateFilteredCalls()
        {
            var filtered = AllCalls.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(SelectedCallType))
                filtered = filtered.Where(c => c.Type == SelectedCallType);

            if (IsDistanceAscending)
                filtered = filtered.OrderBy(c => c.DistanceCallFromVolunteer);

            FilteredCalls = new ObservableCollection<OpenCallInList>(filtered);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
