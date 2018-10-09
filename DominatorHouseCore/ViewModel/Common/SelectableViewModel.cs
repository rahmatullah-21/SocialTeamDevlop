using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace DominatorHouseCore.ViewModel.Common
{
    public interface ISelectableViewModel<T> : INotifyPropertyChanged
    {
        T Selected { get; }
        INotifyCollectionChanged ItemsCollection { get; }
        void Add(T item);
        void Renew(IEnumerable<T> items);
        void Remove(T item);
        bool Contains(T socialNetworks);
        void SelectByIndex(int index);
        void SetSelected(T selected);

        event EventHandler<T> ItemSelected;
    }

    public class SelectableViewModel<T> : BindableBase, ISelectableViewModel<T>
    {
        private readonly object _syncContext = new object();
        private readonly ObservableCollection<T> _itemsCollection;
        private T _selected;

        public event EventHandler<T> ItemSelected;

        public T Selected
        {
            get { return _selected; }
            set
            {
                SetProperty(ref _selected, value, nameof(Selected));
                OnItemSelected(Selected);
            }
        }

        public INotifyCollectionChanged ItemsCollection
        {
            get
            {
                lock (_syncContext)
                    return _itemsCollection;
            }
        }

        public IReadOnlyCollection<T> Items
        {
            get
            {
                lock (_syncContext)
                    return _itemsCollection;
            }
        }

        public SelectableViewModel(IEnumerable<T> collection)
        {
            this._itemsCollection = new ObservableCollection<T>(collection);
            BindingOperations.EnableCollectionSynchronization(_itemsCollection, _syncContext);
        }

        public SelectableViewModel(IEnumerable<T> collection, T value) : this(collection)
        {
            this.Selected = value;
        }

        public void Add(T item)
        {
            lock (this._syncContext)
            {
                _itemsCollection.Add(item);
            }
        }

        public void Renew(IEnumerable<T> items)
        {
            lock (_syncContext)
            {
                _itemsCollection.Clear();
                _itemsCollection.AddRange(items);
            }
        }

        public void Remove(T item)
        {
            lock (this._syncContext)
            {
                _itemsCollection.Remove(item);
                if (item.Equals(Selected))
                {
                    Selected = default(T);
                }
            }
        }

        public void Remove(IEnumerable<T> items)
        {
            lock (this._syncContext)
            {
                foreach (var item in items)
                {
                    _itemsCollection.Remove(item);
                    if (item.Equals(Selected))
                    {
                        Selected = default(T);
                    }
                }
            }
        }

        protected virtual void OnItemSelected(T e)
        {
            // TODO: yes, it's ugly, but i was forced to use Dispatcher here. We get rid of that when we transite from using code behind & creating controls  to MVVM pattern & using templates
            Application.Current.Dispatcher.Invoke(() => { ItemSelected?.Invoke(this, e); });
        }

        public bool Contains(T socialNetworks)
        {
            lock (this._syncContext)
                return _itemsCollection.Contains(socialNetworks);
        }

        public void SelectByIndex(int index)
        {
            lock (_syncContext)
            {
                if (_itemsCollection.Count > index)
                {
                    Selected = _itemsCollection[index];
                }
            }
        }

        public void SetSelected(T selected)
        {
            this.Selected = selected;
        }
    }
}
