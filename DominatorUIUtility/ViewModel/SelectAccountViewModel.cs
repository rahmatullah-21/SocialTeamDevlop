using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore.Command;

namespace LegionUIUtility.ViewModel
{
    public class SelectAccountViewModel : BindableBase
    {
        public SelectAccountViewModel()
        {
            SelectSingle = new BaseCommand<object>((sender) => true, SingleAccountSelector);
        }
        public ICommand SelectSingle { get; set; }
        private bool _isUncheckFromList;


        private bool _isAllAccountSelected;
        public bool IsAllAccountSelected
        {
            get
            {
                return _isAllAccountSelected;
            }
            set
            {

                if (_isAllAccountSelected == value)
                    return;
                SetProperty(ref _isAllAccountSelected, value);
                MultiSelector(_isAllAccountSelected);
                _isUncheckFromList = false;
            }
        }
        private SelectAccountModel _selectAccountModel = new SelectAccountModel();

        public SelectAccountModel SelectAccountModel
        {
            get { return _selectAccountModel; }
            set { SetProperty(ref _selectAccountModel, value); }
        }

        private ICollectionView _accountCollectionView;

        public ICollectionView AccountCollectionView
        {
            get
            {
                return _accountCollectionView;
            }
            set
            {
                if (_accountCollectionView != null && _accountCollectionView == value)
                    return;
                SetProperty(ref _accountCollectionView, value);

            }
        }

        private ObservableCollection<SelectAccountModel> _lstSelectAccount = new ObservableCollection<SelectAccountModel>();
        public ObservableCollection<SelectAccountModel> LstSelectAccount
        {
            get { return _lstSelectAccount; }
            set { SetProperty(ref _lstSelectAccount, value); }
        }

        /// <summary>
        ///     SelectDeselectAccountByGroup method take a boolean value.
        ///     pass true if you want to select account by selected groups.
        ///     pass false if you want to deselect account by selected groups.
        /// </summary>
        /// <param name="isChecked"></param>
        public void SelectDeselectAccountByGroup(bool isChecked)
        {
            try
            {
                var checkedGroup = SelectAccountModel.Groups.Where(group => group.IsContentSelected == isChecked);
                LstSelectAccount.ForEach(account =>
                {
                    checkedGroup.ForEach(group =>
                    {
                        if (account.GroupName == group.Content)
                            account.IsAccountSelected = isChecked;
                    });
                });
                if (LstSelectAccount.All(account => account.IsAccountSelected == isChecked))
                    IsAllAccountSelected = isChecked;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public void AccountGroupSelected()
        {
            try
            {
                var selectedGroups = SelectAccountModel.Groups.Count(x => x.IsContentSelected);
                SelectAccountModel.GroupText = $"{selectedGroups} {"LangKeyGroupSSelected".FromResourceDictionary()?.ToString()}";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        /// <summary>
        ///     SelectDeselectAllAccount method take a boolean value.
        ///     pass true if you want to select all account.
        ///     pass false if you want to deselect all account.
        /// </summary>
        /// <param name="isChecked"></param>
        private void MultiSelector(bool isChecked)
        {
            try
            {
                if (_isUncheckFromList)
                    return;

                var list = AccountCollectionView.Cast<SelectAccountModel>();
                var selectFromlist = LstSelectAccount.Count == list.Count()
                                     ? LstSelectAccount.ToList() : LstSelectAccount.Intersect(list).ToList();


                selectFromlist.Select(x =>
                {
                    x.IsAccountSelected = isChecked;
                    return x;
                }).ToList();
                SelectAccountModel.Groups.Select(group =>
                {
                    group.IsContentSelected = isChecked;
                    return group;
                }).ToList();
                _isUncheckFromList = false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        void CompareModelAndSelectionList()
        {
            var collectionList = AccountCollectionView.Cast<SelectAccountModel>();
            if (collectionList.Count().Equals(0))
                return;
            var list = LstSelectAccount.Except(collectionList);
            list.Where(x => x.IsAccountSelected).ForEach(x => x.IsAccountSelected = false);
        }

        public void ChangeSelectionAfterFilter()
        {
            CompareModelAndSelectionList();
            var list = AccountCollectionView.Cast<SelectAccountModel>();
            if (list.Count().Equals(0) || !list.All(x => x.IsAccountSelected))
            {
                if (IsAllAccountSelected)
                {
                    _isUncheckFromList = true;
                    IsAllAccountSelected = false;
                }
            }
            else
            {
                if (!IsAllAccountSelected)
                {
                    _isUncheckFromList = true;
                    IsAllAccountSelected = true;
                }
            }
        }

        private void SingleAccountSelector(object value)
        {
            if (AccountCollectionView.Cast<SelectAccountModel>().All(x => x.IsAccountSelected))
            {
                IsAllAccountSelected = true;
                SelectAccountModel.Groups.Select(group =>
                {
                    group.IsContentSelected = true;
                    return group;
                }).ToList();
            }
            else
            {
                if (IsAllAccountSelected)
                    _isUncheckFromList = true;
                IsAllAccountSelected = false;
            }
        }
    }
}
