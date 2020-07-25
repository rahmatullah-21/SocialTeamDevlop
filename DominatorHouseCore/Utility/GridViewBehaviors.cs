using System.Windows;
using System.Windows.Controls;

namespace DominatorHouseCore.Utility
{
    public class GridViewBehaviors
    {

        public static readonly DependencyProperty CollapseableColumnProperty =
            DependencyProperty.RegisterAttached("CollapseableColumn", typeof(bool), typeof(GridViewBehaviors),
                new UIPropertyMetadata(false, OnCollapseableColumnChanged));

        private static void OnCollapseableColumnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var header = sender as GridViewColumnHeader;
            if (header == null)
                return;

            header.IsVisibleChanged += AdjustWidth;
        }

        private static void AdjustWidth(object sender, DependencyPropertyChangedEventArgs e)
        {
            var header = sender as GridViewColumnHeader;
            if (header == null)
                return;
            header.Column.Width = header.Visibility == Visibility.Collapsed ? 0 : double.NaN;
        }

    }
}