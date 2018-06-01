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
    [TemplatePart(Name = ImageDeleteMenu,Type = typeof(MenuItem))]
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
        public const string ButtonNavigateNextImage = "PART_NavigateNextImage";
        public const string TextBlockCurrentPointerMediaId = "PART_CurrentPointerMediaId";
        public const string TextBlockTotalMediacount = "PART_TotalMediacount";
        public const string Image = "PART_Image";
        public const string ImageDeleteMenu = "PART_MenuItemImageDelete";
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
            }
        }

        // Using a DependencyProperty as the backing store for MediaList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaListProperty =
            DependencyProperty.Register("MediaList",
                typeof(ObservableCollection<string>),
                typeof(MediaViewer),
                new PropertyMetadata(new ObservableCollection<string>()));

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





        public bool IsPostDataPresent
        {
            get { return (bool)GetValue(IsPostDataPresentProperty); }
            set { SetValue(IsPostDataPresentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPostDataPresent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPostDataPresentProperty =
            DependencyProperty.Register("IsPostDataPresent", typeof(bool), typeof(MediaViewer), new PropertyMetadata(false));





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


        public Visibility DeleteMenuVisibility
        {
            get { return (Visibility)GetValue(DeleteMenuVisibilityProperty); }
            set { SetValue(DeleteMenuVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeleteMenuVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteMenuVisibilityProperty =
            DependencyProperty.Register("DeleteMenuVisibility", typeof(Visibility), typeof(MediaViewer), new PropertyMetadata(System.Windows.Visibility.Collapsed));


        Button _buttonPreviousImage = new Button();
        Button _buttonNextImage = new Button();
        MenuItem _imageDelete = new MenuItem();
        #endregion

        #region Apply Template

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template == null)
                return;

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

            //Previous image button event register
            #region Previous image button event register

            var buttonImageDelete = Template.FindName(ImageDeleteMenu, this) as MenuItem;
            if (!_imageDelete.Equals(buttonImageDelete))
            {
                //Unhook existing events
                if (buttonImageDelete != null)
                    buttonImageDelete.Click -= DeleteImageClick;

                _imageDelete = buttonImageDelete;

                //Add new events
                if (buttonImageDelete != null)
                    buttonImageDelete.Click += DeleteImageClick;
            }

            #endregion


        }

        #endregion

        #region Events Handler


        public static readonly RoutedEvent NextImageEvent = EventManager.RegisterRoutedEvent("NextImage",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MediaViewer));

        public event RoutedEventHandler NextImage
        {
            add { AddHandler(NextImageEvent, value); }
            remove { RemoveHandler(NextImageEvent, value); }
        }

        private void OnNextImage()
        {
            RoutedEventArgs args = new RoutedEventArgs(NextImageEvent);
            RaiseEvent(args);
        }

        public void NextImageClick(object sender, RoutedEventArgs args)
        {
            //Raise your event
            OnNextImage();
        }

        public static readonly RoutedEvent PreviousImageEvent = EventManager.RegisterRoutedEvent("PreviousImage",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MediaViewer));

        public event RoutedEventHandler PreviousImage
        {
            add { AddHandler(PreviousImageEvent, value); }
            remove { RemoveHandler(PreviousImageEvent, value); }
        }

        private void OnPreviousImage()
        {
            RoutedEventArgs args = new RoutedEventArgs(PreviousImageEvent);
            RaiseEvent(args);
        }


        public void PreviousImageClick(object sender, RoutedEventArgs args)
        {
            //Raise your event
            OnPreviousImage();
        }



        public static readonly RoutedEvent DeleteImageEvent = EventManager.RegisterRoutedEvent("DeleteImage",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MediaViewer));

        public event RoutedEventHandler DeleteImage
        {
            add { AddHandler(DeleteImageEvent, value); }
            remove { RemoveHandler(DeleteImageEvent, value); }
        }

        private void OnDeleteImage()
        {
            RoutedEventArgs args = new RoutedEventArgs(DeleteImageEvent);
            RaiseEvent(args);
        }

        public void DeleteImageClick(object sender, RoutedEventArgs args)
        {
            //Raise your event
            OnDeleteImage();
        }

        #endregion

    }
}
