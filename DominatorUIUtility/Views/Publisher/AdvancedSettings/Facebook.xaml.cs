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
    /// Interaction logic for Facebook.xaml
    /// </summary>
    public partial class Facebook : UserControl
    {
        private Facebook()
        {
            InitializeComponent();
        }

        static Facebook ObjFacebook = null;
        public static Facebook GetSingeltonFacebookObject()
        {
            if (ObjFacebook == null)
                ObjFacebook = new Facebook();
            return ObjFacebook;
        }
    }
}
