using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using DominatorUIUtility.ConfigControl;

namespace DominatorHouse.Social.DashBoards
{
    /// <summary>
    /// Interaction logic for DashBoardTab.xaml
    /// </summary>
    public partial class DashBoardTab : UserControl
    {
        private DashBoardTab()
        {
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyRevisionHistory").ToString(),
                    Content = new Lazy<UserControl>(RevisionHistory.GetSingeltonObjectRevisionHistory)
                },
            };
            DashBoardTabs.ItemsSource = tabItems;
        }
        private static DashBoardTab ObjDashBoardTab;

        public static DashBoardTab GetSingeltonObjectDashBoardTab()
        {
            return ObjDashBoardTab ?? (ObjDashBoardTab = new DashBoardTab());
        }
    }
}
