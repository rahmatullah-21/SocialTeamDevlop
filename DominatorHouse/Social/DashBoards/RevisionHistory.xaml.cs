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
using DominatorHouseCore.ViewModel;

namespace DominatorHouse.Social.DashBoards
{
    /// <summary>
    /// Interaction logic for RevisionHistory.xaml
    /// </summary>
    public partial class RevisionHistory : UserControl
    {
        private RevisionHistoryViewModel RevisionHistoryViewModel { get; set; }
        private RevisionHistory()
        {
            InitializeComponent();
            RevisionHistoryViewModel=new RevisionHistoryViewModel();
            MainGrid.DataContext = RevisionHistoryViewModel;
        }
        private static RevisionHistory ObjRevisionHistory;

        public static RevisionHistory GetSingeltonObjectRevisionHistory()
        {
            return ObjRevisionHistory ?? (ObjRevisionHistory = new RevisionHistory());
        }
    }
}
