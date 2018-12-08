using System.Windows.Controls;

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
        private static ErrorHandling ObjErrorHandling;
        public static ErrorHandling GetSingeltonErrorHandlingObject()
        {
            if (ObjErrorHandling == null)
                ObjErrorHandling = new ErrorHandling();
            return ObjErrorHandling;
        }
    }
}
