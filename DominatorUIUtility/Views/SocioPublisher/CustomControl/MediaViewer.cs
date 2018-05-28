using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    ///     <MyNamespace:MediaViewer/>
    ///
    /// </summary>

    [TemplatePart(Name = Border, Type = typeof(Border))]
    [TemplatePart(Name = ButtonNavigatePreviousImage, Type = typeof(Button))]
    [TemplatePart(Name = Image, Type = typeof(Image))]
    [TemplatePart(Name = ButtonNavigateNextImage, Type = typeof(Button))]
    [TemplatePart(Name = TextBlockCurrentPointerMediaId, Type = typeof(TextBlock))]
    [TemplatePart(Name = TextBlockTotalMediacount, Type = typeof(TextBlock))]
    public class MediaViewer : Control
    {
        #region Constructor

        static MediaViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MediaViewer), new FrameworkPropertyMetadata(typeof(MediaViewer)));
        }

        #endregion


        #region Properties

        public const string Border = "PART_Border";
        public const string ButtonNavigatePreviousImage = "PART_NavigatePreviousImage";
        public const string Image = "PART_Image";
        public const string ButtonNavigateNextImage = "PART_NavigateNextImage";
        public const string TextBlockCurrentPointerMediaId = "PART_CurrentPointerMediaId";
        public const string TextBlockTotalMediacount = "PART_TotalMediacount";

        /// <summary>
        /// To store all media items 
        /// </summary>
        public ObservableCollection<string> MediaList
        {
            get
            {
                return (ObservableCollection<string>)GetValue(MediaListProperty);
            }
            set
            {
                SetValue(MediaListProperty, value);

                if (MediaList.Count > 0)
                {
                    TotalMediaCount = MediaList.Count;
                    CurrentMediaPointer = 1;
                    CurrentMediaUrl = MediaList[CurrentMediaPointer - 1];
                    IsEnablePreviousPointer = false;
                    IsEnableNextPointer = MediaList.Count > 1;
                }
            }
        }

        // Using a DependencyProperty as the backing store for MediaList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaListProperty =
            DependencyProperty.Register("MediaList", typeof(ObservableCollection<string>), typeof(MediaViewer), new PropertyMetadata(new ObservableCollection<string>()));

        /// <summary>
        /// To specify the current media url
        /// </summary>
        public string CurrentMediaUrl
        {
            get { return (string)GetValue(CurrentMediaUrlProperty); }
            set { SetValue(CurrentMediaUrlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentMediaUrl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentMediaUrlProperty =
            DependencyProperty.Register("CurrentMediaUrl", typeof(string), typeof(MediaViewer), new PropertyMetadata(string.Empty));


        /// <summary>
        /// To specify the total media count of media list
        /// </summary>
        public int TotalMediaCount
        {
            get
            { return (int)GetValue(TotalMediaCountProperty); }
            set
            { SetValue(TotalMediaCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TotalMediaCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalMediaCountProperty =
            DependencyProperty.Register("TotalMediaCount", typeof(int), typeof(MediaViewer), new PropertyMetadata(0));



        /// <summary>
        /// To hold the current pointer/index of media list
        /// </summary>
        public int CurrentMediaPointer
        {
            get { return (int)GetValue(CurrentMediaPointerProperty); }
            set { SetValue(CurrentMediaPointerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentMediaPointer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentMediaPointerProperty =
            DependencyProperty.Register("CurrentMediaPointer", typeof(int), typeof(MediaViewer), new PropertyMetadata(0));


        /// <summary>
        /// To specify whether GoNext button enable or not
        /// </summary>
        public bool IsEnableNextPointer
        {
            get { return (bool)GetValue(IsEnableNextPointerProperty); }
            set { SetValue(IsEnableNextPointerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEnableNextPointer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEnableNextPointerProperty =
            DependencyProperty.Register("IsEnableNextPointer", typeof(bool), typeof(MediaViewer), new PropertyMetadata(false));


        /// <summary>
        /// To specify whether GoPrevious button enable or not
        /// </summary>
        public bool IsEnablePreviousPointer
        {
            get { return (bool)GetValue(IsEnablePreviousPointerProperty); }
            set { SetValue(IsEnablePreviousPointerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEnablePreviousPointer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEnablePreviousPointerProperty =
            DependencyProperty.Register("IsEnablePreviousPointer", typeof(bool), typeof(MediaViewer), new PropertyMetadata(false));


        Border _border;
        Button _buttonPreviousImage = new Button();
        Image _mediaImage;
        Button _buttonNextImage = new Button();
        TextBlock _textBlockCurrentPointer;
        TextBlock _textTotalMediaCount;


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Breakpoint here to see if the new value is being set
            var newValue = e.NewValue;
        }


        #endregion


        #region Apply Template

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template == null)
                return;

            _border = Template.FindName(Border, this) as Border;

            #region Media Image

            CurrentMediaUrl = "C:\\Users\\Public\\Pictures\\Sample Pictures\\2.jpg";

            _mediaImage = Template.FindName(Image, this) as Image;
            var mediaImage = new BitmapImage();
            mediaImage.BeginInit();
            mediaImage.UriSource = new Uri(CurrentMediaUrl, UriKind.Relative);           
            mediaImage.EndInit();

            if (_mediaImage != null)
                _mediaImage.Source = mediaImage;

            #endregion



            _textBlockCurrentPointer = Template.FindName(TextBlockCurrentPointerMediaId, this) as TextBlock;

            if (_textBlockCurrentPointer != null)
                _textBlockCurrentPointer.Text = "10";

            _textTotalMediaCount = Template.FindName(TextBlockTotalMediacount, this) as TextBlock;

            //Previous image button event register
            #region Previous image button event register

            var buttonPreviousImage = Template.FindName(ButtonNavigatePreviousImage, this) as Button;
            if (!_buttonPreviousImage.Equals(buttonPreviousImage))
            {
                //Unhook existing events
                if (buttonPreviousImage != null)
                    buttonPreviousImage.Click -= PreviousImageClick;

                _buttonPreviousImage = buttonPreviousImage;

                //Add new events
                if (buttonPreviousImage != null)
                    buttonPreviousImage.Click += PreviousImageClick;
            }

            #endregion

            //Next image button event register
            #region Next image button event register

            var buttonNextImage = Template.FindName(ButtonNavigateNextImage, this) as Button;
            if (!_buttonNextImage.Equals(buttonNextImage))
            {
                //Unhook existing events
                if (buttonNextImage != null)
                    buttonNextImage.Click -= NextImageClick;

                _buttonNextImage = buttonNextImage;

                //Add new events
                if (buttonNextImage != null)
                    buttonNextImage.Click += NextImageClick;
            }

            #endregion

        }

        #endregion

        #region Events Handler

        public void NextImageClick(object sender, RoutedEventArgs args)
        {




            if (MediaList.Count <= 0)
                return;
            TotalMediaCount = MediaList.Count;
            ++CurrentMediaPointer;
            CurrentMediaUrl = MediaList[CurrentMediaPointer - 1];

            _mediaImage = Template.FindName(Image, this) as Image;
            var mediaImage = new BitmapImage();
            mediaImage.BeginInit();
            mediaImage.UriSource = new Uri(CurrentMediaUrl, UriKind.Relative);
            mediaImage.EndInit();

            if (_mediaImage != null)
                _mediaImage.Source = mediaImage;

            IsEnablePreviousPointer = (MediaList.Count - CurrentMediaPointer) > 0;
            IsEnableNextPointer = MediaList.Count > 1;
        }

        public void PreviousImageClick(object sender, RoutedEventArgs args)
        {
            if (MediaList.Count <= 0)
                return;

            TotalMediaCount = MediaList.Count;
            CurrentMediaPointer = CurrentMediaPointer--;
            CurrentMediaUrl = MediaList[CurrentMediaPointer - 1];
            IsEnablePreviousPointer = (MediaList.Count - CurrentMediaPointer) > 0;
            IsEnableNextPointer = MediaList.Count > 1;
        }

        #endregion

    }
}
