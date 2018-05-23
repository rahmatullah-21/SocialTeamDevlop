using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;

namespace DominatorUIUtility.Behaviours
{
   
    public class SelectionChangedBehaviour
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand),
            typeof(SelectionChangedBehaviour), new PropertyMetadata(PropertyChangedCallback));

        public static void PropertyChangedCallback(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            Selector selector = (Selector)depObj;
            if (selector != null)
            {
                selector.SelectionChanged += new SelectionChangedEventHandler(SelectionChanged);
            }
        }

        public static ICommand GetCommand(UIElement element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }

        public static void SetCommand(UIElement element, ICommand command)
        {
            element.SetValue(CommandProperty, command);
        }

        private static void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selector selector = (Selector)sender;
            if (selector != null)
            {
                ICommand command = selector.GetValue(CommandProperty) as ICommand;
                if (command != null)
                {
                    command.Execute(selector.SelectedItem);
                }
            }
        }
    }

    public class ReportManager
    {
        //public static Func<string, string, ObservableCollection<QueryInfo>> GetSavedQuery { get; set; }

        //public static Func<Reports, List<KeyValuePair<string, string>>,DbOperations, CampaignDetails, int> GetReportDetail
        //{ get; set; }
        ////public static Func<CampaignAccountWiseReport, DataBaseConnection, string, int> GetAccountWiseReportDetail{ get; set; }
        //public static Func<CampaignAccountWiseReport, DbOperations, int> GetAccountWiseReportDetail { get; set; }


        //// public static Func<Reports, List<KeyValuePair<string, string>>, CampaignDetails, int>  GetReportDetail { get; set; }

        //// public static Func<CampaignAccountWiseReport, DataBaseConnection, string, int> GetAccountWiseReportDetail{ get; set; }

        public static Func<string> GetHeader { get; set; }
        //public static Action<string, string> ExportReports { get; set; }
        public static Func<string, ReportModel, bool> FilterByQueryType { get; set; }
        //public static Func<string, ReportModel, bool> FilterByAccount { get; set; }
    }

}