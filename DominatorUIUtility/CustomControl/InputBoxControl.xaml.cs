using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Utility;
using System.Windows.Input;
using DominatorHouseCore;
using System;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.LogHelper;

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
            InputCollection = new List<string>();
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



        public List<string> InputCollection
        {
            get { return (List<string>)GetValue(InputCollectionProperty); }
            set { SetValue(InputCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputCollectionProperty =
            DependencyProperty.Register("InputCollection", typeof(List<string>), typeof(InputBoxControl), new PropertyMetadata(new List<string>()));



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
            try
            {
                var list = FileUtilities.FileBrowseAndReader();
                if (list.Count == 0)
                    return;
                foreach (var text in list)
                {
                    if (!InputCollection.Contains(text))
                        InputCollection.Add(text);

                }

                InputText = string.Empty;
                //InputCollection.ForEach(x =>
                //{
                //    InputText = string.IsNullOrEmpty(InputText) ? x : InputText + "\r\n" + x;
                //});

                List<string> tmpLstInputs = InputCollection;

                GlobusLogHelper.log.Info("Text uploading process has been started...");

                ThreadFactory.Instance.Start(() =>
                {

                    for (int counter = 0; counter < tmpLstInputs.Count; counter++)
                    {
                        string input = tmpLstInputs[counter];

                        if (!Application.Current.Dispatcher.CheckAccess())
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                InputText = string.IsNullOrEmpty(InputText) ? input : InputText + "\r\n" + input;
                            });
                        }
                        else
                        {
                            InputText = string.IsNullOrEmpty(InputText) ? input : InputText + "\r\n" + input;
                        }

                        System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(20));
                    }

                    GlobusLogHelper.log.Info("Text uploading process has been completed");
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void BtnSaveBlacklistsText_OnClick(object sender, RoutedEventArgs e)
        {
            GetInputClickEventHandler();
            InputCollection.Add(InputText);
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



        public ICommand SaveCommand
        {
            get { return (ICommand)GetValue(SaveCommandProperty); }
            set { SetValue(SaveCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SaveCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaveCommandProperty =
            DependencyProperty.Register("SaveCommand", typeof(ICommand), typeof(InputBoxControl));

        public ICommand ClearCommand
        {
            get { return (ICommand)GetValue(ClearCommandProperty); }
            set { SetValue(ClearCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SaveCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClearCommandProperty =
            DependencyProperty.Register("ClearCommand", typeof(ICommand), typeof(InputBoxControl));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(InputBoxControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged));


    }
}
