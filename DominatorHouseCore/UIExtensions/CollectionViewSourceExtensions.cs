using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace DominatorHouseCore.UIExtensions
{
    public class CollectionViewSourceExtensions
    {

        public static ObservableCollection<string> GetGroupBy(DependencyObject obj)
        {
            return (ObservableCollection<string>)obj.GetValue(GroupByProperty);
        }
        public static void SetGroupBy(DependencyObject obj, ObservableCollection<string> value)
        {
            obj.SetValue(GroupByProperty, value);
        }

        public static readonly DependencyProperty GroupByProperty =
            DependencyProperty.RegisterAttached("GroupBy", typeof(ObservableCollection<string>), typeof(CollectionViewSourceExtensions), new PropertyMetadata(null, GroupByPropertyChangedCallback));

        private static void GroupByPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {

            var collectionSource = (CollectionViewSource)dependencyObject;
            var newValue = (ObservableCollection<string>)args.NewValue;
            if (newValue != null)
            {
                newValue.CollectionChanged += (sender, eventArgs) =>
                {
                    collectionSource.GroupDescriptions.Clear();
                    collectionSource.GroupDescriptions.AddRange(newValue.Select(a => new PropertyGroupDescription(a)));
                };
            }
        }
    }
}
