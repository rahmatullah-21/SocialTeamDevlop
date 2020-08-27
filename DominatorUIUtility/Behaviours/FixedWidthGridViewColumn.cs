using System.Windows;
using System.Windows.Controls;

namespace DominatorUIUtility.Behaviours
{
    public class FixedWidthGridViewColumn : GridViewColumn
    {
        public static readonly DependencyProperty FixedWidthProperty = DependencyProperty.Register("FixedWidth",
            typeof(double), typeof(FixedWidthGridViewColumn),
            new FrameworkPropertyMetadata(double.NaN, OnFixedWidthChanged));

        static FixedWidthGridViewColumn()
        {
            WidthProperty.OverrideMetadata(typeof(FixedWidthGridViewColumn),
                new FrameworkPropertyMetadata(null, OnCoerceWidth));
        }

        public double FixedWidth
        {
            get => (double) GetValue(FixedWidthProperty);

            set => SetValue(FixedWidthProperty, value);
        }

        private static object OnCoerceWidth(DependencyObject o, object baseValue)
        {
            var fixedWidthGridViewColumn = o as FixedWidthGridViewColumn;
            if (fixedWidthGridViewColumn != null)
                return fixedWidthGridViewColumn.FixedWidth;

            return baseValue;
        }

        private static void OnFixedWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var fixedWidthGridViewColumn = o as FixedWidthGridViewColumn;

            if (fixedWidthGridViewColumn != null)
                fixedWidthGridViewColumn.CoerceValue(WidthProperty);
        }
    }
}