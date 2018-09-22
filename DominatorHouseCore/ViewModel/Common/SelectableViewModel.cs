using DominatorHouseCore.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace DominatorHouseCore.ViewModel.Common
{
    public class SelectableViewModel<T> : BindableBase
    {
        private INotifyCollectionChanged _itemsCollection;
        private T _selected;

        public T Selected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value, nameof(Selected)); }
        }

        public INotifyCollectionChanged ItemsCollection
        {
            get { return _itemsCollection; }
            set { SetProperty(ref _itemsCollection, value, nameof(ItemsCollection)); }
        }

        public SelectableViewModel(IEnumerable<T> collection)
        {
            this.ItemsCollection = new ObservableCollection<T>(collection);
        }

        public SelectableViewModel(IEnumerable<T> collection, T value) : this(collection)
        {
            this.Selected = value;
        }

    }
}
