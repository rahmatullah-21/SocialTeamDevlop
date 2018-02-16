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

namespace DominatorUIUtility.Views.Publisher.AdvancedOptions
{
    /// <summary>
    /// Interaction logic for AdvancedOption.xaml
    /// </summary>
    public partial class AdvancedOption : UserControl
    {
        public AdvancedOption()
        {
            InitializeComponent();
            AddPosts ObjAddPosts = AddPosts.GetSingeltonAddPosts();
            MainGrid.DataContext = ObjAddPosts.AddPostViewModel.AddPostModel;
        }
    }
}
