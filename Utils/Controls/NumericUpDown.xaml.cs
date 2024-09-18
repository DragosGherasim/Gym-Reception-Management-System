using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gym_Reception_Management_System.Utils.Controls
{
    /// <summary>
    ///     Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        public NumericUpDown()
        {
            InitializeComponent();
        }

        public int? LowerBound
        {
            get => (int?)GetValue(LowerBoundProperty);
            set => SetValue(LowerBoundProperty, value);
        }

        public static readonly DependencyProperty LowerBoundProperty =
            DependencyProperty.Register(nameof(LowerBound), typeof(int?), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int? UpperBound
        {
            get => (int?)GetValue(UpperBoundProperty);
            set => SetValue(UpperBoundProperty, value);
        }

        public static readonly DependencyProperty UpperBoundProperty =
            DependencyProperty.Register(nameof(UpperBound), typeof(int?), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set
            {
                if (value < LowerBound)
                    value = UpperBound ?? LowerBound.Value;

                if (value > UpperBound)
                    value = LowerBound ?? UpperBound.Value;

                SetValue(ValueProperty, value);
            }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(int), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(default(int), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Hint
        {
            get => (string)GetValue(HintProperty);
            set => SetValue(HintProperty, value);
        }

        public static readonly DependencyProperty HintProperty = DependencyProperty.Register(nameof(Hint),
            typeof(string), typeof(NumericUpDown),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private void IncreaseButton_OnClick(object sender, RoutedEventArgs e) => Value++;

        private void DecreaseButton_OnClick(object sender, RoutedEventArgs e) => Value--;

        private void NumericUpDown_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!IsMouseOver) return;

            if (e.Delta > 0)
                Value++;
            else if (e.Delta < 0) Value--;

            e.Handled = true;
        }
    }
}