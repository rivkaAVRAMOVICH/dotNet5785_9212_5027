using System.Windows;

namespace PL.Volunteer
{
    public partial class ChooseCallView : Window
    {
        public ChooseCallView(BO.Volunteer volunteer)
        {
            InitializeComponent();
            var viewModel = new ChooseCallViewModel(BlApi.Factory.Get(), volunteer);
            this.DataContext = viewModel;
        }
    }
}
