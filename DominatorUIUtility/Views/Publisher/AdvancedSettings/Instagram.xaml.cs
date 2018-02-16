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

namespace DominatorUIUtility.Views.Publisher.AdvancedSettings
{
    /// <summary>
    /// Interaction logic for Instagram.xaml
    /// </summary>
    public partial class Instagram : UserControl
    {
        private Instagram()
        {
            InitializeComponent();
        }
        static Instagram ObjInstagram = null;
        public static Instagram GetSingeltonInstagramObject()
        {
            if (ObjInstagram == null)
                ObjInstagram = new Instagram();
            return ObjInstagram;
        }
    }
}
