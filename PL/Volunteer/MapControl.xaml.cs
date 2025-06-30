using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer
{
    public partial class MapControl : UserControl
    {
        public MapControl()
        {
            InitializeComponent();
        }

        public double Latitude
        {
            get => (double)GetValue(LatitudeProperty);
            set => SetValue(LatitudeProperty, value);
        }

        public static readonly DependencyProperty LatitudeProperty =
            DependencyProperty.Register("Latitude", typeof(double), typeof(MapControl), new PropertyMetadata(0.0));

        public double Longitude
        {
            get => (double)GetValue(LongitudeProperty);
            set => SetValue(LongitudeProperty, value);
        }

        public static readonly DependencyProperty LongitudeProperty =
            DependencyProperty.Register("Longitude", typeof(double), typeof(MapControl), new PropertyMetadata(0.0));
        public double CallLatitude
        {
            get => (double)GetValue(CallLatitudeProperty);
            set => SetValue(CallLatitudeProperty, value);
        }

        public static readonly DependencyProperty CallLatitudeProperty =
            DependencyProperty.Register("CallLatitude", typeof(double), typeof(MapControl), new PropertyMetadata(0.0));

        public double CallLongitude
        {
            get => (double)GetValue(CallLongitudeProperty);
            set => SetValue(CallLongitudeProperty, value);
        }

        public static readonly DependencyProperty CallLongitudeProperty =
            DependencyProperty.Register("CallLongitude", typeof(double), typeof(MapControl), new PropertyMetadata(0.0));

        public bool DrawLines
        {
            get => (bool)GetValue(DrawLinesProperty);
            set => SetValue(DrawLinesProperty, value);
        }

        public static readonly DependencyProperty DrawLinesProperty =
            DependencyProperty.Register("DrawLines", typeof(bool), typeof(MapControl), new PropertyMetadata(false));

        public bool DrawRoute
        {
            get => (bool)GetValue(DrawRouteProperty);
            set => SetValue(DrawRouteProperty, value);
        }

        public static readonly DependencyProperty DrawRouteProperty =
            DependencyProperty.Register("DrawRoute", typeof(bool), typeof(MapControl), new PropertyMetadata(false));
    }
}
