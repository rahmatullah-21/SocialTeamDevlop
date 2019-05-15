using System;
using System.Windows.Input;
using CommonServiceLocator;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.NetworkActivitySetting;
using Prism.Commands;
using Prism.Regions;
using System.Windows;
using DominatorHouseCore;
using DominatorUIUtility.CustomControl;
using System.Linq;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ViewModel.Startup
{

    public interface IQueryViewModel
    {
    }
    public class QueryViewModel : StartupBaseViewModel, IQueryViewModel
    {

        public QueryViewModel(IRegionManager region) : base(region)
        {
            NextCommand = new DelegateCommand(OnNextClick);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
          

            SaveSettingModel.ListQueryType.Clear();
            SaveSettingModel.ListQueryType.Add("Query1");
            SaveSettingModel.ListQueryType.Add("Query2");
            SaveSettingModel.ListQueryType.Add("Query3");

            AddQueryCommand = new DelegateCommand<SearchQueryControl>(AddQuery);
        }

        public ICommand AddQueryCommand { get; set; }

        private SaveSettingModel _saveSettingModel = new SaveSettingModel();

        public SaveSettingModel SaveSettingModel
        {
            get { return _saveSettingModel; }
            set { SetProperty(ref _saveSettingModel, value); }
        }

        private void OnNextClick()
        {
            try
            {
                var genericFileManager = ServiceLocator.Current.GetInstance<IGenericFileManager>();
                // genericFileManager.Save(SaveSettingModel, ConstantVariable.GetModuleConfigPath(SelectedNetwork.ToString()));
                Application.Current.MainWindow.Close();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void AddQuery(SearchQueryControl queryControl)
        {
            try
            {
                if (string.IsNullOrEmpty(queryControl.CurrentQuery.QueryValue) && queryControl.QueryCollection.Count != 0)
                {
                    queryControl.QueryCollection.ForEach(query =>
                    {
                        var currentQuery = queryControl.CurrentQuery.Clone() as QueryInfo;

                        if (currentQuery == null) return;

                        currentQuery.QueryValue = query;
                        currentQuery.QueryTypeDisplayName = currentQuery.QueryType;
                        currentQuery.QueryPriority = SaveSettingModel.ListQueryInfo.Count + 1;

                        if (SaveSettingModel.ListQueryInfo.Any(x => x.QueryType == currentQuery.QueryType && x.QueryValue == currentQuery.QueryValue))
                        {
                            Dialog.ShowDialog("Warning", "Query already exist.");
                            return;
                        }
                        SaveSettingModel.ListQueryInfo.Add(currentQuery);
                    });
                }
                else
                {
                    queryControl.CurrentQuery.QueryTypeDisplayName = queryControl.CurrentQuery.QueryType;
                    var currentQuery = queryControl.CurrentQuery.Clone() as QueryInfo;

                    if (currentQuery == null) return;

                    currentQuery.QueryPriority = SaveSettingModel.ListQueryInfo.Count + 1;
                    if (SaveSettingModel.ListQueryInfo.Any(x => x.QueryType == currentQuery.QueryType && x.QueryValue == currentQuery.QueryValue))
                    {
                        Dialog.ShowDialog("Warning", "Query already exist.");
                        return;
                    }
                    currentQuery.Index = SaveSettingModel.ListQueryInfo.Count + 1;
                    SaveSettingModel.ListQueryInfo.Add(currentQuery);
                    queryControl.CurrentQuery = new QueryInfo();
                   
                }
                queryControl.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}