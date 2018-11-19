using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DominatorHouseCore.UIExtensions
{
    public static class GridViewExtension
    {
        public static ObservableCollection<GridViewColumn> GetColumns(DependencyObject obj)
        {
            return (ObservableCollection<GridViewColumn>)obj.GetValue(ColumnsProperty);
        }

        public static void SetColumns(DependencyObject obj, ObservableCollection<GridViewColumn> value)
        {
            obj.SetValue(ColumnsProperty, value);
        }

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.RegisterAttached("Columns",
                typeof(ObservableCollection<GridViewColumn>),
                typeof(GridViewExtension),
                new UIPropertyMetadata(new ObservableCollection<GridViewColumn>(),
                    OnGridViewColumnsPropertyChanged));


        private static void OnGridViewColumnsPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            GridView myGrid = d as GridView;
            if (myGrid != null)
            {

                ObservableCollection<GridViewColumn> Columns =
                    (ObservableCollection<GridViewColumn>)e.NewValue;

                if (Columns != null)
                {

                    var lastAddedColumns = new List<GridViewColumn>();
                    foreach (GridViewColumn column
                        in Columns)
                    {
                        myGrid.Columns.Add(column);
                        lastAddedColumns.Add(column);
                    }

                    Columns.CollectionChanged += delegate (object sender,
                        NotifyCollectionChangedEventArgs args)
                    {
                        if (args.NewItems != null)
                        {
                            foreach (GridViewColumn column
                                in args.NewItems.Cast<GridViewColumn>())
                            {
                                myGrid.Columns.Add(column);
                                lastAddedColumns.Add(column);
                            }
                        }

                        if (args.Action == NotifyCollectionChangedAction.Reset)
                        {
                            foreach (var lastAddedColumn in lastAddedColumns)
                            {
                                myGrid.Columns.Remove(lastAddedColumn);
                            }
                        }

                        if (args.OldItems != null)
                        {
                            foreach (GridViewColumn column
                                in args.OldItems.Cast<GridViewColumn>())
                            {
                                myGrid.Columns.Remove(column);
                            }
                        }
                    };
                }
            }
        }

    }
}
