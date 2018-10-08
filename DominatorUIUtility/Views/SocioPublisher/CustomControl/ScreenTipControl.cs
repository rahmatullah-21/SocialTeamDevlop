
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl
{
    public sealed class ScreenTipControl : Control
    {
        static ScreenTipControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScreenTipControl), new FrameworkPropertyMetadata(typeof(ScreenTipControl)));

            AddOwner(
                BackgroundProperty,
                BorderBrushProperty,
                BorderThicknessProperty,
                DataContextProperty,
                FontFamilyProperty,
                FontStretchProperty,
                FontStyleProperty,
                FontSizeProperty,
                FontWeightProperty);
        }

        private static void AddOwner(params DependencyProperty[] depdencyProperties)
        {
            foreach (var dp in depdencyProperties)
                dp.AddOwner(typeof(ScreenTipControl), new FrameworkPropertyMetadata { Inherits = false });
        }

        /// <summary>
        /// Gets or sets the content that is shown in the content area of the popup.
        /// Use the <see cref="Step.Content"/> to define the content to show. The default template shows the string representation of the content as text.
        /// Use the <see cref="Step.ContentDataTemplateKey"/> to define a data template for displaying the content.
        /// </summary>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ScreenTipControl), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the header that is shown in the header area of the popup.
        /// Use <see cref="Step.Header"/> to define the header to show. The default template shows the string representation of the header as text.
        /// Use <see cref="Step.HeaderDataTemplateKey"/> to define a data template for displaying the header.
        /// </summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(ScreenTipControl), new PropertyMetadata(string.Empty));

        //public Placement Placement
        //{
        //    get { return (Placement)GetValue(PlacementProperty); }
        //    set { SetValue(PlacementProperty, value); }
        //}

        // Using a DependencyProperty as the backing store for Placement.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty PlacementProperty =
        //    DependencyProperty.Register("Placement", typeof(Placement), typeof(ScreenTipControl), new PropertyMetadata(Placement.LeftBottom));

        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(double), typeof(ScreenTipControl), new PropertyMetadata(3.0));
    }
}
