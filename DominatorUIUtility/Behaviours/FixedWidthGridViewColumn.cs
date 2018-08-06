using System.Windows;
using System.Windows.Controls;

namespace DominatorUIUtility.Behaviours
{
    public class FixedWidthGridViewColumn : GridViewColumn
    {
        static FixedWidthGridViewColumn()
        {
            WidthProperty.OverrideMetadata(typeof(FixedWidthGridViewColumn), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceWidth)));
        }

        private static object OnCoerceWidth(DependencyObject o, object baseValue)
        {
            FixedWidthGridViewColumn fixedWidthGridViewColumn = o as FixedWidthGridViewColumn;
            if (fixedWidthGridViewColumn != null)
                return fixedWidthGridViewColumn.FixedWidth;

            return baseValue;
        }

        public double FixedWidth
        {
            get { return (double)GetValue(FixedWidthProperty); }

            set { SetValue(FixedWidthProperty, value); }
        }

        public static readonly DependencyProperty FixedWidthProperty = DependencyProperty.Register("FixedWidth", typeof(double), typeof(FixedWidthGridViewColumn),
            new FrameworkPropertyMetadata(double.NaN, new PropertyChangedCallback(OnFixedWidthChanged)));

        private static void OnFixedWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FixedWidthGridViewColumn fixedWidthGridViewColumn = o as FixedWidthGridViewColumn;

            if (fixedWidthGridViewColumn != null)
                fixedWidthGridViewColumn.CoerceValue(WidthProperty);
        }

    }
}