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

        // Latitude של המתנדב
        public double Latitude
        {
            get => (double)GetValue(LatitudeProperty);
            set => SetValue(LatitudeProperty, value);
        }

        public static readonly DependencyProperty LatitudeProperty =
            DependencyProperty.Register("Latitude", typeof(double), typeof(MapControl), new PropertyMetadata(0.0));

        // Longitude של המתנדב
        public double Longitude
        {
            get => (double)GetValue(LongitudeProperty);
            set => SetValue(LongitudeProperty, value);
        }

        public static readonly DependencyProperty LongitudeProperty =
            DependencyProperty.Register("Longitude", typeof(double), typeof(MapControl), new PropertyMetadata(0.0));

        // Latitude של הקריאה
        public double CallLatitude
        {
            get => (double)GetValue(CallLatitudeProperty);
            set => SetValue(CallLatitudeProperty, value);
        }

        public static readonly DependencyProperty CallLatitudeProperty =
            DependencyProperty.Register("CallLatitude", typeof(double), typeof(MapControl), new PropertyMetadata(0.0));

        // Longitude של הקריאה
        public double CallLongitude
        {
            get => (double)GetValue(CallLongitudeProperty);
            set => SetValue(CallLongitudeProperty, value);
        }

        public static readonly DependencyProperty CallLongitudeProperty =
            DependencyProperty.Register("CallLongitude", typeof(double), typeof(MapControl), new PropertyMetadata(0.0));

        // האם לצייר קווים אוויריים בין נקודות
        public bool DrawLines
        {
            get => (bool)GetValue(DrawLinesProperty);
            set => SetValue(DrawLinesProperty, value);
        }

        public static readonly DependencyProperty DrawLinesProperty =
            DependencyProperty.Register("DrawLines", typeof(bool), typeof(MapControl), new PropertyMetadata(false));

        // האם להציג מסלול נסיעה
        public bool DrawRoute
        {
            get => (bool)GetValue(DrawRouteProperty);
            set => SetValue(DrawRouteProperty, value);
        }

        public static readonly DependencyProperty DrawRouteProperty =
            DependencyProperty.Register("DrawRoute", typeof(bool), typeof(MapControl), new PropertyMetadata(false));
    }
}
