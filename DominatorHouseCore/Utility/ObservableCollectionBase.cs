using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace DominatorHouseCore.Utility
{
    public class ObservableCollectionBase<TType> : IList, ICollection, IEnumerable, IList<TType>, ICollection<TType>, IEnumerable<TType>, INotifyPropertyChanged, INotifyCollectionChanged, IReadOnlyCollection<TType>
    {

        /// <summary>
        /// _listLocker is used to lock the current collection object
        /// </summary>
        private readonly object _listLocker = new object();

        /// <summary>
        /// _inputCollection is the source where the operations takes place
        /// </summary>
        private readonly IList<TType> _inputCollection;

        private object _syncRoot;

        public ObservableCollectionBase(IList<TType> inputCollection)
        {
            this._inputCollection = inputCollection;
        }

        public ObservableCollectionBase()
        {
            this._inputCollection = (IList<TType>)new List<TType>();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;


        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Count property is used get the count of the input collection items
        /// </summary>
        public int Count
        {
            get
            {
                lock (this._listLocker)
                    return this._inputCollection.Count;
            }
        }


        bool IList.IsFixedSize
        {
            get
            {
                var collection = this._inputCollection as IList;
                return collection?.IsFixedSize ?? this._inputCollection.IsReadOnly;
            }
        }

        bool IList.IsReadOnly => this._inputCollection.IsReadOnly;

        bool ICollection.IsSynchronized => true;

        object ICollection.SyncRoot
        {
            get
            {
                if (this._syncRoot != null)
                    return this._syncRoot;
                lock (this._listLocker)
                {
                    var collection = this._inputCollection as ICollection;
                    if (collection != null)
                        this._syncRoot = collection.SyncRoot;
                    else
                        Interlocked.CompareExchange<object>(ref this._syncRoot, new object(), (object)null);
                }
                return this._syncRoot;
            }
        }

        bool ICollection<TType>.IsReadOnly => this._inputCollection.IsReadOnly;

        public TType this[int index]
        {
            get
            {
                lock (this._listLocker)
                    return this._inputCollection[index];
            }
            set
            {
                lock (this._listLocker)
                    this._inputCollection[index] = value;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return (object)this[index];
            }
            set
            {
                this[index] = (TType)value;
            }
        }

        public void Add(TType item)
        {
            lock (this._listLocker)
            {
                this._inputCollection.Add(item);
                this.OnPropertyChanged("Count");
                this.OnPropertyChanged("Item[]");
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (object)item));
            }
        }

        public void AddRange(IList<TType> objects)
        {
            lock (this._listLocker)
            {
                ((List<TType>)this._inputCollection).AddRange((IEnumerable<TType>)objects);
                this.OnPropertyChanged("Count");
                this.OnPropertyChanged("Item[]");
                this.OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (object)objects));
            }
        }

        public void Clear()
        {
            lock (this._listLocker)
                this._inputCollection.Clear();
        }

        public bool Contains(TType item)
        {
            lock (this._listLocker)
                return this._inputCollection.Contains(item);
        }

        public void CopyTo(TType[] array, int arrayIndex)
        {
            lock (this._listLocker)
                this._inputCollection.CopyTo(array, arrayIndex);
        }

        public int IndexOf(TType item)
        {
            lock (this._listLocker)
                return this._inputCollection.IndexOf(item);
        }

        public void Insert(int index, TType item)
        {
            lock (this._listLocker)
                this._inputCollection.Insert(index, item);
        }

        public bool Remove(TType item)
        {
            lock (this._listLocker)
            {
                if (!this._inputCollection.Remove(item))
                    return false;
                this.OnPropertyChanged("Count");
                this.OnPropertyChanged("Item[]");
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (object)item));
                return true;
            }
        }

        public void RemoveAll(Predicate<TType> predicate)
        {
            lock (this._listLocker)
            {
                var collection = (List<TType>)this._inputCollection;
                var predicate1 = (Func<TType, bool>)(x => predicate(x));
                var objs = collection.Where<TType>(predicate1);
                var match = predicate;
                collection.RemoveAll(match);
                this.OnPropertyChanged("Count");
                this.OnPropertyChanged("Item[]");
                this.OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (object)objs));
            }
        }

        public void RemoveAt(int index)
        {
            lock (this._listLocker)
            {
                var obj = this._inputCollection[index];
                this._inputCollection.RemoveAt(index);
                this.OnPropertyChanged("Count");
                this.OnPropertyChanged("Item[]");
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (object)obj));
            }
        }

        public void RemoveRange(int begin, int end)
        {
            lock (this._listLocker)
            {
                var collection = (List<TType>)this._inputCollection;
                var index1 = begin;
                var count1 = end;
                collection.RemoveRange(index1, count1);
                var index2 = begin;
                var count2 = end;
                var range = collection.GetRange(index2, count2);
                this.OnPropertyChanged("Count");
                this.OnPropertyChanged("Item[]");
                this.OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList)range));
            }
        }

        int IList.Add(object value)
        {
            this.Add((TType)value);
            return this.Count - 1;
        }

        bool IList.Contains(object value)
        {
            return this.Contains((TType)value);
        }

        void ICollection.CopyTo(Array inputArray, int index)
        {
            lock (this._listLocker)
            {
                if (inputArray.Rank != 1)
                    throw new ArgumentException("Multidimension arrays are not supported");
                if (inputArray.GetLowerBound(0) != 0)
                    throw new ArgumentException("Non-zero lower bound arrays are not supported");
                if (index < 0)
                    throw new ArgumentOutOfRangeException();
                if (inputArray.Length - index < this._inputCollection.Count)
                    throw new ArgumentException("Array is too small");
                var typeArray = inputArray as TType[];
                if (typeArray == null)
                    throw new ArrayTypeMismatchException("Invalid inputArray type");
                this._inputCollection.CopyTo(typeArray, index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (this._listLocker)
                return (IEnumerator)this._inputCollection.GetEnumerator();
        }

        int IList.IndexOf(object value)
        {
            return this.IndexOf((TType)value);
        }

        void IList.Insert(int index, object value)
        {
            var obj = (TType)value;
            this.Insert(index, obj);
        }

        void IList.Remove(object value)
        {
            this.Remove((TType)value);
        }

        IEnumerator<TType> IEnumerable<TType>.GetEnumerator()
        {

            lock (this._listLocker)
                return this._inputCollection.GetEnumerator();

        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
           
            var handler = this.CollectionChanged;
            if (handler == null)
                return;

            if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate { handler((object) this, args); }));

               // Application.Current.Dispatcher.BeginInvoke((Delegate)(() => handler((object)this, args)));
            else
                handler((object)this, args);

        }

        private void OnCollectionChangedMultiItem(NotifyCollectionChangedEventArgs e)
        {        
                
            var collectionChanged = this.CollectionChanged;

            if (collectionChanged == null)
                return;

            foreach (var invocation in collectionChanged.GetInvocationList())
            {
                var view = invocation.Target as CollectionView;

                if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
                    Application.Current.Dispatcher.BeginInvoke
                    (view != null
                        ? new Action(delegate { view.Refresh(); })
                        : new Action(delegate
                        {
                            collectionChanged((object) this, e);
                        }));
                // Application.Current.Dispatcher.BeginInvoke(view != null ? (Delegate)(() => view.Refresh()) : (Delegate)(() => collectionChanged((object)this, e)));
                else if (view != null)
                    view.Refresh();
                else
                    collectionChanged((object) this, e);
            }
        }

        private void OnPropertyChanged(string propertyName = null)
        {          
            var propertyChanged = this.PropertyChanged;
            if (propertyChanged == null)
                return;
            var e = new PropertyChangedEventArgs(propertyName);
            propertyChanged((object)this, e);
        }
    }
}
