using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace DominatorHouseCore.Converters
{
    public class TwoListSynchronizer : IWeakEventListener
    {

        private static readonly IListItemConverter DefaultConverter = new DoNothingListItemConverter();
        private readonly IList _masterList;
        private readonly IListItemConverter _masterTargetConverter;
        private readonly IList _targetList;

        public TwoListSynchronizer(IList masterList, IList targetList, IListItemConverter masterTargetConverter)
        {
            _masterList = masterList;
            _targetList = targetList;
            _masterTargetConverter = masterTargetConverter;
        }

        public TwoListSynchronizer(IList masterList, IList targetList)
            : this(masterList, targetList, DefaultConverter)
        {
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            HandleCollectionChanged(sender as IList, e as NotifyCollectionChangedEventArgs);
            return true;
        }

        public void StartSynchronizing()
        {
            ListenForChangeEvents(_masterList);
            ListenForChangeEvents(_targetList);
            SetListValuesFromSource(_masterList, _targetList, ConvertFromMasterToTarget);
            if (TargetAndMasterCollectionsAreEqual())
                return;
            SetListValuesFromSource(_targetList, _masterList, ConvertFromTargetToMaster);
        }

        public void StopSynchronizing()
        {
            StopListeningForChangeEvents(_masterList);
            StopListeningForChangeEvents(_targetList);
        }

        protected void ListenForChangeEvents(IList list)
        {
            if (!(list is INotifyCollectionChanged))
                return;
            CollectionChangedEventManager.AddListener(list as INotifyCollectionChanged, this);
        }

        protected void StopListeningForChangeEvents(IList list)
        {
            if (!(list is INotifyCollectionChanged))
                return;
            CollectionChangedEventManager.RemoveListener(list as INotifyCollectionChanged, this);
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
            if (_masterTargetConverter != null)
                return _masterTargetConverter.Convert(masterListItem);
            return masterListItem;
        }

        private object ConvertFromTargetToMaster(object targetListItem)
        {
            if (_masterTargetConverter != null)
                return _masterTargetConverter.ConvertBack(targetListItem);
            return targetListItem;
        }

        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IList sourceList = sender as IList;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    PerformActionOnAllLists(AddItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    PerformActionOnAllLists(RemoveItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    PerformActionOnAllLists(ReplaceItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Move:
                    PerformActionOnAllLists(MoveItems, sourceList, e);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    UpdateListsFromSource(sender as IList);
                    break;
            }
        }

        private void MoveItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            RemoveItems(list, e, converter);
            AddItems(list, e, converter);
        }

        private void PerformActionOnAllLists(ChangeListAction action, IList sourceList, NotifyCollectionChangedEventArgs collectionChangedArgs)
        {
            if (sourceList == _masterList)
                PerformActionOnList(_targetList, action, collectionChangedArgs, ConvertFromMasterToTarget);
            else
                PerformActionOnList(_masterList, action, collectionChangedArgs, ConvertFromTargetToMaster);
        }

        private void PerformActionOnList(IList list, ChangeListAction action, NotifyCollectionChangedEventArgs collectionChangedArgs, Converter<object, object> converter)
        {
            StopListeningForChangeEvents(list);
            action(list, collectionChangedArgs, converter);
            ListenForChangeEvents(list);
        }

        private void RemoveItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            int count = e.OldItems.Count;
            for (int index = 0; index < count; ++index)
                list.RemoveAt(e.OldStartingIndex);
        }

        private void ReplaceItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
        {
            RemoveItems(list, e, converter);
            AddItems(list, e, converter);
        }

        private void SetListValuesFromSource(IList sourceList, IList targetList, Converter<object, object> converter)
        {
            StopListeningForChangeEvents(targetList);
            targetList.Clear();
            foreach (object source in sourceList)
                targetList.Add(converter(source));
            ListenForChangeEvents(targetList);
        }

        private bool TargetAndMasterCollectionsAreEqual()
        {
            return _masterList.Cast<object>().SequenceEqual(_targetList.Cast<object>().Select(item => ConvertFromTargetToMaster(item)));
        }

        private void UpdateListsFromSource(IList sourceList)
        {
            if (sourceList == _masterList)
                SetListValuesFromSource(_masterList, _targetList, ConvertFromMasterToTarget);
            else
                SetListValuesFromSource(_targetList, _masterList, ConvertFromTargetToMaster);
        }

        private delegate void ChangeListAction(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter);

        public interface IListItemConverter
        {
            object Convert(object masterListItem);

            object ConvertBack(object targetListItem);
        }

        internal class DoNothingListItemConverter : IListItemConverter
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