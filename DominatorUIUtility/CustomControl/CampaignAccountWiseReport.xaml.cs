using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DominatorHouseCore.Models;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for CampaignAccountWiseReport.xaml
    /// </summary>
    public partial class CampaignAccountWiseReport : UserControl
    {
        public CampaignAccountWiseReport()
        {
            InitializeComponent();
        }

        public CampaignAccountWiseReport(AccountWiseReportModel accountWiseReport)
        {
            this.AccountWiseReport = accountWiseReport;
            MainGrid.DataContext = this;

        }
        public AccountWiseReportModel AccountWiseReport
        {
            get { return (AccountWiseReportModel)GetValue(AccountWiseReportProperty); }
            set { SetValue(AccountWiseReportProperty, value); }
        }
        
        public static readonly DependencyProperty AccountWiseReportProperty =
            DependencyProperty.Register("AccountWiseReport", typeof(AccountWiseReportModel), typeof(CampaignAccountWiseReport), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }
        private void CmbAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReportGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = Regex.Replace(e.Column.Header.ToString(), "(\\B[A-Z])", " $1");
        }
    }
}
