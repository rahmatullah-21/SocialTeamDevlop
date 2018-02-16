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
    /// Interaction logic for ErrorHandling.xaml
    /// </summary>
    public partial class ErrorHandling : UserControl
    {
        private ErrorHandling()
        {
            InitializeComponent();
        }
        private static ErrorHandling ObjErrorHandling = null;
        public static ErrorHandling GetSingeltonErrorHandlingObject()
        {
            if (ObjErrorHandling == null)
                ObjErrorHandling = new ErrorHandling();
            return ObjErrorHandling;
        }
    }
}
