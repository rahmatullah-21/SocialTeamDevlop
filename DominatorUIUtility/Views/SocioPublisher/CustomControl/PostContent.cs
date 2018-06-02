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
    [TemplatePart(Type = typeof(Button), Name = ButtonMacros)]
    [TemplatePart(Type = typeof(Button), Name = ButtonSettings)]
    [TemplatePart(Type = typeof(MediaViewer), Name = MediaViewerControl)]
    public class PostContent : MediaViewer
    {
        static PostContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PostContent), new FrameworkPropertyMetadata(typeof(PostContent)));
        }

        #region Properties


        public const string ButtonImportImage = "PART_ImportImage";
        public const string ButtonMacros = "PART_Macros";
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


         Button _selectMedia = new Button();

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

        #endregion

        #region Events

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
                files.ForEach(x =>
                {
                    MediaList.Add(x);
                });
                mediaViewer.Initialize();
            }

            // Raise your event
            SelectMedia();
        }


        #endregion
    }
}
