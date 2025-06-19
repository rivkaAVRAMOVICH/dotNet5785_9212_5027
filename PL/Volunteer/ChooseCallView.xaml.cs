using System.Windows;

namespace PL.Volunteer
{
    public partial class ChooseCallView : Window
    {
        public ChooseCallView(BO.Volunteer volunteer)
        {
            InitializeComponent();
            var ChooseCallViewModel = new ChooseCallViewModel(BlApi.Factory.Get(), volunteer);
            this.DataContext = ChooseCallViewModel;
        }
    };
}
