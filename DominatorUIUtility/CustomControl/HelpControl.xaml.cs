using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using DominatorHouseCore;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for HelpControl.xaml
    /// </summary>
    public partial class HelpControl : UserControl
    {

        public HelpControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        /// <summary>
        /// ModuleDescription is used for display the description about the modules 
        /// </summary>
        public string ModuleDescription
        {
            get
            {
                return (string)GetValue(ModuleDescriptionProperty);
            }
            set
            {
                SetValue(ModuleDescriptionProperty, value);
            }
        }

        public static readonly DependencyProperty ModuleDescriptionProperty =
             DependencyProperty.Register("ModuleDescription", typeof(string), typeof(HelpControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
             {
                 BindsTwoWayByDefault = true
             });



        /// <summary>
        /// WatchVideoUrl is used for navigate the video url for this modules 
        /// </summary>
        public string VideoTutorial
        {
            get
            {
                return (string)GetValue(VideoTutorialProperty);
            }
            set
            {
                SetValue(VideoTutorialProperty, value);
            }
        }
        public static readonly DependencyProperty VideoTutorialProperty =
              DependencyProperty.Register("VideoTutorial", typeof(string), typeof(HelpControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
              {
                  BindsTwoWayByDefault = true
              });



        /// <summary>
        /// TakeHelpFromKnowledgebaseUrl is used for navigate the knowledge base url for this modules 
        /// </summary>
        public string KnowledgeBaseLink
        {
            get
            {
                return (string)GetValue(KnowledgeBaseLinkProperty);
            }
            set
            {
                SetValue(KnowledgeBaseLinkProperty, value);
            }
        }
        public static readonly DependencyProperty KnowledgeBaseLinkProperty =
               DependencyProperty.Register("KnowledgeBaseLink", typeof(string), typeof(HelpControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
               {
                   BindsTwoWayByDefault = true
               });


        /// <summary>
        /// ContactSupportUrl is used for navigate the contact support url for this modules 
        /// </summary>
        public string ContactSupportLink
        {
            get
            {
                return (string)GetValue(ContactSupportLinkProperty);
            }
            set
            {
                SetValue(ContactSupportLinkProperty, value);
            }
        }
        public static readonly DependencyProperty ContactSupportLinkProperty =
              DependencyProperty.Register("ContactSupportLink", typeof(string), typeof(HelpControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
              {
                  BindsTwoWayByDefault = true
              });


        /// <summary>
        /// OnAvailableItemsChanged is used to update the value of the particular dependency property
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnAvailableItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {          
            var newValue = e.NewValue;
        }


        /// <summary>
        /// Hyperlink_RequestNavigate event is used to navigate the absolute url to default browser of the system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

}
