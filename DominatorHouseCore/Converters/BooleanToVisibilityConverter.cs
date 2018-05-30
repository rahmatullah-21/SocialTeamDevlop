using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(bool),typeof(Visibility))]

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && bool.Parse(value.ToString()) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    public class StringLengthToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.Parse(value.ToString()) >0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    [ValueConversion(typeof(string), typeof(string))]

    public class StringToPasswordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "*****";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
 
    public class TwoListSynchronizer : IWeakEventListener
    {

        private static readonly TwoListSynchronizer.IListItemConverter DefaultConverter = (TwoListSynchronizer.IListItemConverter)new TwoListSynchronizer.DoNothingListItemConverter();
        private readonly IList _masterList;
        private readonly TwoListSynchronizer.IListItemConverter _masterTargetConverter;
        private readonly IList _targetList;

        public TwoListSynchronizer(IList masterList, IList targetList, TwoListSynchronizer.IListItemConverter masterTargetConverter)
        {
            this._masterList = masterList;
            this._targetList = targetList;
            this._masterTargetConverter = masterTargetConverter;
        }

        public TwoListSynchronizer(IList masterList, IList targetList)
          : this(masterList, targetList, TwoListSynchronizer.DefaultConverter)
        {
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            this.HandleCollectionChanged((object)(sender as IList), e as NotifyCollectionChangedEventArgs);
            return true;
        }

        public void StartSynchronizing()
        {
            this.ListenForChangeEvents(this._masterList);
            this.ListenForChangeEvents(this._targetList);
            this.SetListValuesFromSource(this._masterList, this._targetList, new Converter<object, object>(this.ConvertFromMasterToTarget));
            if (this.TargetAndMasterCollectionsAreEqual())
                return;
            this.SetListValuesFromSource(this._targetList, this._masterList, new Converter<object, object>(this.ConvertFromTargetToMaster));
        }

        public void StopSynchronizing()
        {
            this.StopListeningForChangeEvents(this._masterList);
            this.StopListeningForChangeEvents(this._targetList);
        }

        protected void ListenForChangeEvents(IList list)
        {
            if (!(list is INotifyCollectionChanged))
                return;
            CollectionChangedEventManager.AddListener(list as INotifyCollectionChanged, (IWeakEventListener)this);
        }

        protected void StopListeningForChangeEvents(IList list)
        {
            if (!(list is INotifyCollectionChanged))
                return;
            CollectionChangedEventManager.RemoveListener(list as INotifyCollectionChanged, (IWeakEventListener)this);
        }

        private void AddItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            int count = e.NewItems.Count;
            for (int index1 = 0; index1 < count; ++index1)
            {
                int index2 = e.NewStartingIndex + index1;
                if (index2 > list.Count)
                    list.Add(converter(e.NewItems[index1]));
                else
                    list.Insert(index2, converter(e.NewItems[index1]));
            }
        }

        private object ConvertFromMasterToTarget(object masterListItem)
        {
            if (this._masterTargetConverter != null)
                return this._masterTargetConverter.Convert(masterListItem);
            return masterListItem;
        }

        private object ConvertFromTargetToMaster(object targetListItem)
        {
            if (this._masterTargetConverter != null)
                return this._masterTargetConverter.ConvertBack(targetListItem);
            return targetListItem;
        }

        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IList sourceList = sender as IList;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.PerformActionOnAllLists(new TwoListSynchronizer.ChangeListAction(this.AddItems), sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.PerformActionOnAllLists(new TwoListSynchronizer.ChangeListAction(this.RemoveItems), sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.PerformActionOnAllLists(new TwoListSynchronizer.ChangeListAction(this.ReplaceItems), sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Move:
                    this.PerformActionOnAllLists(new TwoListSynchronizer.ChangeListAction(this.MoveItems), sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.UpdateListsFromSource(sender as IList);
                    break;
            }
        }

        private void MoveItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            this.RemoveItems(list, e, converter);
            this.AddItems(list, e, converter);
        }

        private void PerformActionOnAllLists(TwoListSynchronizer.ChangeListAction action, IList sourceList, NotifyCollectionChangedEventArgs collectionChangedArgs)
        {
            if (sourceList == this._masterList)
                this.PerformActionOnList(this._targetList, action, collectionChangedArgs, new Converter<object, object>(this.ConvertFromMasterToTarget));
            else
                this.PerformActionOnList(this._masterList, action, collectionChangedArgs, new Converter<object, object>(this.ConvertFromTargetToMaster));
        }

        private void PerformActionOnList(IList list, TwoListSynchronizer.ChangeListAction action, NotifyCollectionChangedEventArgs collectionChangedArgs, Converter<object, object> converter)
        {
            this.StopListeningForChangeEvents(list);
            action(list, collectionChangedArgs, converter);
            this.ListenForChangeEvents(list);
        }

        private void RemoveItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            int count = e.OldItems.Count;
            for (int index = 0; index < count; ++index)
                list.RemoveAt(e.OldStartingIndex);
        }

        private void ReplaceItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            this.RemoveItems(list, e, converter);
            this.AddItems(list, e, converter);
        }

        private void SetListValuesFromSource(IList sourceList, IList targetList, Converter<object, object> converter)
        {
            this.StopListeningForChangeEvents(targetList);
            targetList.Clear();
            foreach (object source in (IEnumerable)sourceList)
                targetList.Add(converter(source));
            this.ListenForChangeEvents(targetList);
        }

        private bool TargetAndMasterCollectionsAreEqual()
        {
            return this._masterList.Cast<object>().SequenceEqual<object>(this._targetList.Cast<object>().Select<object, object>((Func<object, object>)(item => this.ConvertFromTargetToMaster(item))));
        }

        private void UpdateListsFromSource(IList sourceList)
        {
            if (sourceList == this._masterList)
                this.SetListValuesFromSource(this._masterList, this._targetList, new Converter<object, object>(this.ConvertFromMasterToTarget));
            else
                this.SetListValuesFromSource(this._targetList, this._masterList, new Converter<object, object>(this.ConvertFromTargetToMaster));
        }

        private delegate void ChangeListAction(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter);

        public interface IListItemConverter
        {
            object Convert(object masterListItem);

            object ConvertBack(object targetListItem);
        }

        internal class DoNothingListItemConverter : TwoListSynchronizer.IListItemConverter
        {
            public object Convert(object masterListItem)
            {
                return masterListItem;
            }

            public object ConvertBack(object targetListItem)
            {
                return targetListItem;
            }
        }
    }
}
