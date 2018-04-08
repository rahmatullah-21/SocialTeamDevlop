using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for InputBoxControl.xaml
    /// </summary>
    public partial class InputBoxControl : UserControl
    {
        public InputBoxControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            SaveVisiblity = Visibility.Visible;
            ImportVisiblity = Visibility.Visible;
            RefreshVisiblity = Visibility.Visible;
            Height = 80;
        }

        public string InputText
        {
            get
            {
                return (string)GetValue(InputTextProperty);
            }
            set
            {
                SetValue(InputTextProperty, value);
            }
        }

        public static readonly DependencyProperty InputTextProperty =
            DependencyProperty.Register("InputText", typeof(string), typeof(InputBoxControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });


        public List<string> InputCollection = new List<string>();
      

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

            var newValue = e.NewValue;
        }
        private static readonly RoutedEvent GetInputClickEvent = EventManager.RegisterRoutedEvent("GetInputClick",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(InputBoxControl));

        /// <summary>
        /// Create a RoutedEventHandler for query clicks
        /// </summary>
        public event RoutedEventHandler GetInputClick
        {
            add { AddHandler(GetInputClickEvent, value); }
            remove { RemoveHandler(GetInputClickEvent, value); }
        }

        void GetInputClickEventHandler()
        {
            var routedEventArgs = new RoutedEventArgs(GetInputClickEvent);
            RaiseEvent(routedEventArgs);
        }
        private void BtnImportBlacklistsText_OnClick(object sender, RoutedEventArgs e)
        {
            InputCollection.AddRange(FileUtilities.FileBrowseAndReader());
        
            InputCollection.ForEach(x=>
                InputText = InputText + "\r\n" + x);
        }

        private void BtnSaveBlacklistsText_OnClick(object sender, RoutedEventArgs e)
        {
            GetInputClickEventHandler();
        }

        private void BtnRefereshBlacklistsText_OnClick(object sender, RoutedEventArgs e)
        {
            InputCollection.Clear();
            InputText = string.Empty;
        }
        public Visibility SaveVisiblity
        {
            get
            {
                return (Visibility)GetValue(SaveVisiblityProperty);
            }
            set
            {
                SetValue(SaveVisiblityProperty, value);
            }
        }

        public static readonly DependencyProperty SaveVisiblityProperty =
            DependencyProperty.Register("SaveVisiblity", typeof(Visibility), typeof(InputBoxControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });
        public Visibility ImportVisiblity
        {
            get
            {
                return (Visibility)GetValue(ImportVisiblityProperty);
            }
            set
            {
                SetValue(ImportVisiblityProperty, value);
            }
        }

        public static readonly DependencyProperty ImportVisiblityProperty =
            DependencyProperty.Register("ImportVisiblity", typeof(Visibility), typeof(InputBoxControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });
        public Visibility RefreshVisiblity
        {
            get
            {
                return (Visibility)GetValue(RefreshVisiblityProperty);
            }
            set
            {
                SetValue(RefreshVisiblityProperty, value);
            }
        }

        public static readonly DependencyProperty RefreshVisiblityProperty =
            DependencyProperty.Register("RefreshVisiblity", typeof(Visibility), typeof(InputBoxControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });
        public string WaterMarkText
        {
            get
            {
                return (string)GetValue(WaterMarkProperty);
            }
            set
            {
                SetValue(WaterMarkProperty, value);
            }
        }

        public static readonly DependencyProperty WaterMarkProperty =
            DependencyProperty.Register("WaterMarkText", typeof(string), typeof(InputBoxControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });
    }
}
