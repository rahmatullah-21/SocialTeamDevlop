using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : UserControl
    {
        public Reports()
        {
            InitializeComponent();          
            MainGrid.DataContext = this;
        }

        public Reports(ReportModel ReportModel)
        {
            InitializeComponent();
            this.ReportModel = ReportModel;
            MainGrid.DataContext = this;
        }

        public ReportModel ReportModel
        {
            get { return (ReportModel)GetValue(ReportModelProperty); }
            set { SetValue(ReportModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReportModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReportModelProperty =
            DependencyProperty.Register("ReportModel", typeof(ReportModel), typeof(Reports), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });
       

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

    }
}
