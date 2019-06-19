using DominatorHouseCore;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Utility;
using DominatorHouseCore.Models.FacebookModels;

namespace DominatorUIUtility.CustomControl.FacebookCustomControl
{
    /// <summary>
    /// Interaction logic for FbMultiMediaControl.xaml
    /// </summary>
    public partial class FbMultiMediaControl : UserControl
    {
        public FbMultiMediaControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        public FbMultiMediaModel FbMultiMediaModel
        {
            get { return (FbMultiMediaModel)GetValue(FbMultiMediaModelProperty); }
            set { SetValue(FbMultiMediaModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FbMultiMediaModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FbMultiMediaModelProperty =
            DependencyProperty.Register("FbMultiMediaModel", typeof(FbMultiMediaModel),
                typeof(FbMultiMediaControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });


        public string ActivityType
        {
            get { return (string)GetValue(ActivityTypeProperty); }
            set { SetValue(ActivityTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActivityType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActivityTypeProperty =
            DependencyProperty.Register("ActivityType", typeof(string),
                typeof(FbMultiMediaControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public double MediaHeight
        {
            get { return (double)GetValue(MediaHeightProperty); }
            set { SetValue(MediaHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MediaHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaHeightProperty =
            DependencyProperty.Register("MediaHeight", typeof(double),
                typeof(FbMultiMediaControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });


        public double CloseButtonwidth
        {
            get { return (double)GetValue(CloseButtonwidthProperty); }
            set { SetValue(CloseButtonwidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MediaHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseButtonwidthProperty =
            DependencyProperty.Register("CloseButtonwidth", typeof(double),
                typeof(FbMultiMediaControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var imageValue = ((FrameworkElement)sender).DataContext as MultiMediaValueModel;
            FbMultiMediaModel.MediaPaths.Remove(imageValue);
            FbMultiMediaModel.IsAddImageVisibile = true;
        }

        private void DeleteMedia_Click(object sender, RoutedEventArgs e)
        {
            var imageValue = ((FrameworkElement)sender).DataContext as MultiMediaValueModel;
            FbMultiMediaModel.MediaPaths.Remove(imageValue);
            FbMultiMediaModel.IsAddImageVisibile = true;
        }

        private void BtnPhotos_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = FbMultiMediaModel.IsMultiselect;
                openFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF | Video Files(*.dat; *.wmv; *.mp4;)|*.dat; *.wmv; *.mp4";
                if (openFileDialog.ShowDialog().Value)
                {
                    foreach (var fileName in openFileDialog.FileNames)
                    {
                        FbMultiMediaModel.MediaPaths.Add(new MultiMediaValueModel()
                        {
                            MediaHeight = MediaHeight,
                            MediaPath = fileName
                        });
                    }
                }

                if (FbMultiMediaModel.MediaPaths.Count != 0
                    && ActivityType == "LangKeyEventCreater".FromResourceDictionary())
                    FbMultiMediaModel.IsAddImageVisibile = false;

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

    }
}
