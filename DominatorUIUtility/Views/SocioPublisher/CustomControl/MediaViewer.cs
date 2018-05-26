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
        static MediaViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MediaViewer), new FrameworkPropertyMetadata(typeof(MediaViewer)));
        }

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
        public List<string> MediaList
        {
            get { return (List<string>)GetValue(MediaListProperty); }
            set { SetValue(MediaListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MediaList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaListProperty =
            DependencyProperty.Register("MediaList", typeof(List<string>), typeof(MediaViewer), new PropertyMetadata(new List<string>()));

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


        private Border _border;
        private Button _buttonPreviousImage;
        private Image _mediaImage;
        private Button _buttonNextImage;
        private TextBlock _textBlockCurrentPointer;
        private TextBlock _textTotalMediaCount;

        #endregion

        #region Apply Template

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template != null)
            {
                _border = Template.FindName(Border, this) as Border;
                _buttonPreviousImage = Template.FindName(ButtonNavigatePreviousImage, this) as Button;
                _mediaImage = Template.FindName(Image, this) as Image;
                _buttonNextImage = Template.FindName(ButtonNavigateNextImage, this) as Button;
                _textBlockCurrentPointer = Template.FindName(TextBlockCurrentPointerMediaId, this) as TextBlock;
                _textTotalMediaCount = Template.FindName(TextBlockTotalMediacount, this) as TextBlock;
            }

        }

        #endregion
    }
}
