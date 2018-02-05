using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DominatorHouseCore.Utility
{
    public sealed class ObservableCollectionCustom<T> : IList, ICollection, IEnumerable, IList<T>, ICollection<T>, IEnumerable<T>, INotifyPropertyChanged, INotifyCollectionChanged, IReadOnlyCollection<T>
    {
        private readonly object listLock = new object();
        private readonly IList<T> collection;
        private object syncRoot;

        public ObservableCollectionCustom(IList<T> collection)
        {
            this.collection = collection;
        }

        public ObservableCollectionCustom()
        {
            this.collection = (IList<T>)new List<T>();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Count
        {
            get
            {
                lock (this.listLock)
                    return this.collection.Count;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                IList collection = this.collection as IList;
                if (collection == null)
                    return this.collection.IsReadOnly;
                return collection.IsFixedSize;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return this.collection.IsReadOnly;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return true;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (this.syncRoot != null)
                    return this.syncRoot;
                lock (this.listLock)
                {
                    ICollection collection = this.collection as ICollection;
                    if (collection != null)
                        this.syncRoot = collection.SyncRoot;
                    else
                        Interlocked.CompareExchange<object>(ref this.syncRoot, new object(), (object)null);
                }
                return this.syncRoot;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return this.collection.IsReadOnly;
            }
        }

        public T this[int index]
        {
            get
            {
                lock (this.listLock)
                    return this.collection[index];
            }
            set
            {
                lock (this.listLock)
                    this.collection[index] = value;
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
                this[index] = (T)value;
            }
        }

        public void Add(T item)
        {
            lock (this.listLock)
            {
                this.collection.Add(item);
                this.OnPropertyChanged("Count");
                this.OnPropertyChanged("Item[]");
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (object)item));
            }
        }

        public void AddRange(IList<T> objects)
        {
            lock (this.listLock)
            {
                ((List<T>)this.collection).AddRange((IEnumerable<T>)objects);
                this.OnPropertyChanged("Count");
                this.OnPropertyChanged("Item[]");
                this.OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (object)objects));
            }
        }

        public void Clear()
        {
            lock (this.listLock)
                this.collection.Clear();
        }

        public bool Contains(T item)
        {
            lock (this.listLock)
                return this.collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (this.listLock)
                this.collection.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T item)
        {
            lock (this.listLock)
                return this.collection.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            lock (this.listLock)
                this.collection.Insert(index, item);
        }

        public bool Remove(T item)
        {
            lock (this.listLock)
            {
                if (!this.collection.Remove(item))
                    return false;
                this.OnPropertyChanged("Count");
                this.OnPropertyChanged("Item[]");
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (object)item));
                return true;
            }
        }

        public void RemoveAll(Predicate<T> predicate)
        {
            lock (this.listLock)
            {
                List<T> collection = (List<T>)this.collection;
                IEnumerable<T> objs = collection.Where<T>((Func<T, bool>)(x => predicate(x)));
                collection.RemoveAll(predicate);
                this.OnPropertyChanged("Count");
                this.OnPropertyChanged("Item[]");
                this.OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (object)objs));
            }
        }

        public void RemoveAt(int index)
        {
            lock (this.listLock)
            {
                T obj = this.collection[index];
                this.collection.RemoveAt(index);
                this.OnPropertyChanged("Count");
                this.OnPropertyChanged("Item[]");
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (object)obj));
            }
        }

        public void RemoveRange(int begin, int end)
        {
            lock (this.listLock)
            {
                List<T> collection = (List<T>)this.collection;
                collection.RemoveRange(begin, end);
                List<T> range = collection.GetRange(begin, end);
                this.OnPropertyChanged("Count");
                this.OnPropertyChanged("Item[]");
                this.OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList)range));
            }
        }

        int IList.Add(object value)
        {
            this.Add((T)value);
            return this.Count - 1;
        }

        bool IList.Contains(object value)
        {
            return this.Contains((T)value);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            lock (this.listLock)
            {
                if (array.Rank != 1)
                    throw new ArgumentException("Multidimension arrays are not supported");
                if (array.GetLowerBound(0) != 0)
                    throw new ArgumentException("Non-zero lower bound arrays are not supported");
                if (index < 0)
                    throw new ArgumentOutOfRangeException();
                if (array.Length - index < this.collection.Count)
                    throw new ArgumentException("Array is too small");
                T[] array1 = array as T[];
                if (array1 == null)
                    throw new ArrayTypeMismatchException("Invalid array type");
                this.collection.CopyTo(array1, index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (this.listLock)
                return (IEnumerator)this.collection.GetEnumerator();
        }

        int IList.IndexOf(object value)
        {
            return this.IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            T obj = (T)value;
            this.Insert(index, obj);
        }

        void IList.Remove(object value)
        {
            this.Remove((T)value);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            lock (this.listLock)
                return this.collection.GetEnumerator();
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            // ISSUE: reference to a compiler-generated field
            NotifyCollectionChangedEventHandler handler = this.CollectionChanged;
            if (handler == null)
                return;
            if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
                Application.Current.Dispatcher.BeginInvoke(new Action(() => handler((object)this, args)));
            else
                handler((object)this, args);
        }

        private void OnCollectionChangedMultiItem(NotifyCollectionChangedEventArgs e)
        {
            // ISSUE: reference to a compiler-generated field
            NotifyCollectionChangedEventHandler collectionChanged = this.CollectionChanged;
            if (collectionChanged == null)
                return;
            foreach (Delegate invocation in collectionChanged.GetInvocationList())
            {
                CollectionView view = invocation.Target as CollectionView;
                if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
                    Application.Current.Dispatcher.BeginInvoke(view != null ? (new Action(() => view.Refresh())) : (new Action(() => collectionChanged((object)this, e))));
                else if (view != null)
                    view.Refresh();
                else
                    collectionChanged((object)this, e);
            }
        }

        private void OnPropertyChanged(string propertyName = null)
        {
            // ISSUE: reference to a compiler-generated field
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged == null)
                return;
            propertyChanged((object)this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
