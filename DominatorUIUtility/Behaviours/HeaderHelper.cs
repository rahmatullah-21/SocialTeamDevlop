using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LegionUIUtility.Behaviours
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
                expander.IsExpanded = IsExpanded;

        }
        public static void ExpandCollapseAllExpanderForActivity(object sender, bool IsExpanded)
        {
            var currentcontrol = (FrameworkElement)sender;

            foreach (Expander expander in FindVisualChildren<Expander>(currentcontrol as UserControl))
                expander.IsExpanded = IsExpanded;

        }
        public static bool IsAllExpanderCollapseOrNot(object sender)
        {
            var allExpander = FindVisualChildren<Expander>(sender as UserControl);
            if (allExpander.Count() != 0 && allExpander.All(x => !x.IsExpanded)) return true;
            return false;
        }
        public static Action UpdateToggleButtonInCampaignMode;
        public static Action UpdateToggleButtonInAccountActivityMode;
        public static Action UpdateToggleForQuery;
        public static Action UpdateToggleForNonQuery;
    }
}