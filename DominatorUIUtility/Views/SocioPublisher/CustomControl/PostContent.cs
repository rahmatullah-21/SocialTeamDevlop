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
using DominatorHouseCore;
using DominatorHouseCore.Interfaces.SocioPublisher;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Models.SocioPublisher.Settings;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.Views.SocioPublisher.CustomControl.Settings;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:DominatorUIUtility.Views.SocioPublisher.CustomControl"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:DominatorUIUtility.Views.SocioPublisher.CustomControl;assembly=DominatorUIUtility.Views.SocioPublisher.CustomControl"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:PostContent/>
    ///
    /// </summary>
    [TemplatePart(Type = typeof(Button), Name = ButtonImportImage)]
    [TemplatePart(Type = typeof(Button), Name = ButtonSettings)]
    [TemplatePart(Type = typeof(MediaViewer), Name = MediaViewerControl)]
    public class PostContent : Control
    {
        static PostContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PostContent), new FrameworkPropertyMetadata(typeof(PostContent)));
        }

        #region Properties

        public const string ButtonImportImage = "PART_ImportImage";
        public const string ButtonSettings = "PART_Settings";
        public const string MediaViewerControl = "PART_MediaViewer";

        public string PostDescription
        {
            get { return (string)GetValue(PostDescriptionProperty); }
            set { SetValue(PostDescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PostDescription.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostDescriptionProperty =
            DependencyProperty.Register("PostDescription", typeof(string), typeof(PostContent), new PropertyMetadata(string.Empty));

        public string PublisherInstagramTitle
        {
            get { return (string)GetValue(PublisherInstagramTitleProperty); }
            set { SetValue(PublisherInstagramTitleProperty, value); }
        }

        public static readonly DependencyProperty PublisherInstagramTitleProperty =
             DependencyProperty.Register("PublisherInstagramTitle", typeof(string), typeof(PostContent), new PropertyMetadata(string.Empty));


        public string FdSellProductTitle
        {
            get { return (string)GetValue(FdSellProductTitleProperty); }
            set { SetValue(FdSellProductTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FdSellProductTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FdSellProductTitleProperty =
            DependencyProperty.Register("FdSellProductTitle", typeof(string), typeof(PostContent), new PropertyMetadata(string.Empty));


        public double FdSellPrice
        {
            get { return (double)GetValue(FdSellPriceProperty); }
            set { SetValue(FdSellPriceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FdSellPrice.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FdSellPriceProperty =
            DependencyProperty.Register("FdSellPrice", typeof(double), typeof(PostContent), new PropertyMetadata(double.MinValue));



        public string FdSellLocation
        {
            get { return (string)GetValue(FdSellLocationProperty); }
            set { SetValue(FdSellLocationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FdSellLocation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FdSellLocationProperty =
            DependencyProperty.Register("FdSellLocation", typeof(string), typeof(PostContent), new PropertyMetadata(string.Empty));



        public string PdSourceUrl
        {
            get { return (string)GetValue(PdSourceUrlProperty); }
            set { SetValue(PdSourceUrlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PdSourceUrl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PdSourceUrlProperty =
            DependencyProperty.Register("PdSourceUrl", typeof(string), typeof(PostContent), new PropertyMetadata(string.Empty));



        public Visibility IsImportOptionsVisibility
        {
            get { return (Visibility)GetValue(IsImportOptionsVisibilityProperty); }
            set { SetValue(IsImportOptionsVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsImportOptionsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsImportOptionsVisibilityProperty =
            DependencyProperty.Register("IsImportOptionsVisibility", typeof(Visibility), typeof(PostContent), new PropertyMetadata(Visibility.Visible));


        public bool IsFdSellPost
        {
            get { return (bool)GetValue(IsFdSellPostProperty); }
            set { SetValue(IsFdSellPostProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFdSellPost.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFdSellPostProperty =
            DependencyProperty.Register("IsFdSellPost", typeof(bool), typeof(PostContent), new PropertyMetadata(false));



        Button _selectMedia = new Button();
        private Button _buttonSettings = new Button();


        public PublisherPostSettings PublisherPostSettings
        {
            get { return (PublisherPostSettings)GetValue(PublisherPostSettingsProperty); }
            set { SetValue(PublisherPostSettingsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PublisherPostSettings.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PublisherPostSettingsProperty =
            DependencyProperty.Register("PublisherPostSettings", typeof(PublisherPostSettings), typeof(PostContent), new PropertyMetadata(new PublisherPostSettings()));

        public bool IsRandomlyPickTitleFromList
        {
            get { return (bool)GetValue(IsRandomlyPickTitleFromListProperty); }
            set { SetValue(IsRandomlyPickTitleFromListProperty, value); }
        }

        public static readonly DependencyProperty IsRandomlyPickTitleFromListProperty =
            DependencyProperty.Register("IsRandomlyPickTitleFromList", typeof(bool), typeof(PostContent), new PropertyMetadata(false));
        public bool IsRemoveTitleOnceUsed
        {
            get { return (bool)GetValue(IsRemoveTitleOnceUsedProperty); }
            set { SetValue(IsRemoveTitleOnceUsedProperty, value); }
        }

        public static readonly DependencyProperty IsRemoveTitleOnceUsedProperty =
            DependencyProperty.Register("IsRemoveTitleOnceUsed", typeof(bool), typeof(PostContent), new PropertyMetadata(false));

        public Visibility IsPostTitleOptionVisibility
        {
            get { return (Visibility)GetValue(IsPostTitleOptionVisibilityProperty); }
            set { SetValue(IsPostTitleOptionVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsImportOptionsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPostTitleOptionVisibilityProperty =
            DependencyProperty.Register("IsPostTitleOptionVisibility", typeof(Visibility), typeof(PostContent), new PropertyMetadata(Visibility.Collapsed));
        public double PostTitleHeight
        {
            get { return (double)GetValue(PostTitleHeightProperty); }
            set { SetValue(PostTitleHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsImportOptionsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostTitleHeightProperty =
            DependencyProperty.Register("PostTitleHeight", typeof(double), typeof(PostContent), new PropertyMetadata(30.0));

        #endregion

        #region Apply Template

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();



            #region Button Import Images

            var buttonMedia = Template.FindName(ButtonImportImage, this) as Button;

            if (!_selectMedia.Equals(buttonMedia))
            {
                if (buttonMedia != null)
                    buttonMedia.Click -= SelectMediaClick;

                _selectMedia = buttonMedia;

                if (buttonMedia != null)
                    buttonMedia.Click += SelectMediaClick;
            }

            #endregion

            #region Settings 

            var buttonSettingChanges = Template.FindName(ButtonSettings, this) as Button;

            if (!_buttonSettings.Equals(buttonSettingChanges))
            {
                if (buttonSettingChanges != null)
                {
                    buttonSettingChanges.Click -= PostSettingsChangeClick;
                }
                _buttonSettings = buttonSettingChanges;
                if (buttonSettingChanges != null)
                {
                    buttonSettingChanges.Click += PostSettingsChangeClick;
                }
            }

            #endregion



            this.Loaded += PostContentLoad;

        }

        private void PostContentLoad(object sender, RoutedEventArgs e)
        {
            SetMedia();
        }

        #endregion

        #region Routed Events

        public static readonly RoutedEvent SelectMediaEvent = EventManager.RegisterRoutedEvent(
            "SelectMediaHandler",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(PostContent));

        public event RoutedEventHandler SelectMediaHandler
        {
            add { AddHandler(SelectMediaEvent, value); }
            remove { RemoveHandler(SelectMediaEvent, value); }
        }

        private void SelectMedia()
        {
            RoutedEventArgs args = new RoutedEventArgs(SelectMediaEvent);
            RaiseEvent(args);
        }


        public static readonly RoutedEvent PostSettings = EventManager.RegisterRoutedEvent("PostSettingHandler",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PostContent));

        public event RoutedEventHandler PostSettingHandler
        {
            add { AddHandler(PostSettings, value); }
            remove { RemoveHandler(PostSettings, value); }
        }

        private void PostSettingEvent()
        {
            var args = new RoutedEventArgs(PostSettings);
            RaiseEvent(args);
        }

        #endregion

        #region Events

        public void PostSettingsChangeClick(object sender, RoutedEventArgs args)
        {
            // PublisherPostSettings PublisherPostSettings = new PublisherPostSettings();
            var objAdvancedSettings = new PostAdvancedSettings(PublisherPostSettings);
            var customDialog = new CustomDialog
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = objAdvancedSettings
            };
            var objDialog = new Dialog();
            var dialogWindow = objDialog.GetCustomDialog(customDialog, "Post Settings");
            dialogWindow.ShowDialog();

            PostSettingEvent();
        }

        public void SelectMediaClick(object sender, RoutedEventArgs args)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "Image Files |*.jpg;*.jpeg;*.png;*.gif|Videos Files |*.dat; *.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso; *.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; " +
                         " *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4; *.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm"
            };
            var openFileDialogResult = openFileDialog.ShowDialog();
            if (openFileDialogResult != true)
                return;

            var files = openFileDialog.FileNames.ToList();

            var mediaViewer = Template.FindName(MediaViewerControl, this) as MediaViewer;

            if (mediaViewer != null)
            {
                var mediaUtilites = new MediaUtilites();
                files.ForEach(x =>
                {
                    MediaViewerAssist.SetMediaList(this, mediaViewer.MediaList);
                    mediaViewer.MediaList.Add(mediaUtilites.GetThumbnail(x));
                    //MediaViewer.MediaList.Add(x);
                });
                mediaViewer.Initialize();
            }

            // Raise your event
            SelectMedia();
        }

        internal void SetMedia()
        {
            var mediaViewer = Template.FindName(MediaViewerControl, this) as MediaViewer;
            try
            {
                if (mediaViewer != null)
                {
                    mediaViewer.MediaList = (mediaViewer.DataContext as PublisherPostlistModel).MediaList;

                    mediaViewer.Initialize();
                }
            }
            catch (Exception ex)
            {
                if (mediaViewer != null)
                    mediaViewer.Initialize();
                ex.DebugLog();
            }

        }


        #endregion
    }
}
