using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DominatorUIUtility.Behaviours
{
    public class HeaderHelper
    {
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject usercontrol) where T : DependencyObject
        {
            if (usercontrol != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(usercontrol); i++)
                {
                    DependencyObject userControlChild = VisualTreeHelper.GetChild(usercontrol, i);
                    if (userControlChild is T)
                    {
                        yield return (T)userControlChild;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(userControlChild))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static void ExpandCollapseAllExpander(object sender, bool IsExpanded)
        {
            var currentcontrol = ((FrameworkElement)((FrameworkElement)sender).DataContext).DataContext;

            foreach (Expander expander in FindVisualChildren<Expander>(currentcontrol as UserControl))
            {
                expander.IsExpanded = IsExpanded;
            }
        }
    }
}