using BlApi;
using BO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace PL.Volunteer
{
    public class CallHistoryViewModel : INotifyPropertyChanged
    {
        private readonly IBl bl = Factory.Get();
        private int volunteerId;

        public CallHistoryViewModel(int volunteerId)
        {
            this.volunteerId = volunteerId;

            CallTypes = new ObservableCollection<CallType?>(
                new CallType?[] { null }.Concat(((CallType[])Enum.GetValues(typeof(CallType))).Cast<CallType?>())
            );

            ClosedCalls = new ObservableCollection<ClosedCallInList>(
                bl.Call.GetClosedCallsByVolunteer(volunteerId)
            );

            FilteredClosedCalls = CollectionViewSource.GetDefaultView(ClosedCalls);
            FilteredClosedCalls.Filter = FilterCalls;
            FilteredClosedCalls.SortDescriptions.Add(new SortDescription(nameof(ClosedCallInList.EndCallTime), ListSortDirection.Descending));

            SelectedCallType = null;
        }

        public ObservableCollection<CallType?> CallTypes { get; }

        private CallType? selectedCallType;
        public CallType? SelectedCallType
        {
            get => selectedCallType;
            set
            {
                selectedCallType = value;
                OnPropertyChanged();
                FilteredClosedCalls.Refresh();
            }
        }

        private bool isDateDescending = true;
        public bool IsDateDescending
        {
            get => isDateDescending;
            set
            {
                isDateDescending = value;
                OnPropertyChanged();
                ApplySorting();
            }
        }

        public ObservableCollection<ClosedCallInList> ClosedCalls { get; }

        public ICollectionView FilteredClosedCalls { get; }

        private void ApplySorting()
        {
            FilteredClosedCalls.SortDescriptions.Clear();
            var direction = isDateDescending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            FilteredClosedCalls.SortDescriptions.Add(new SortDescription(nameof(ClosedCallInList.EndCallTime), direction));
        }

        private bool FilterCalls(object obj)
        {
            if (obj is not ClosedCallInList call)
                return false;

            return SelectedCallType == null || call.Type == SelectedCallType;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
