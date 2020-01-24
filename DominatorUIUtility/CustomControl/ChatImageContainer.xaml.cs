using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LegionUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for ChatImageContainer.xaml
    /// </summary>
    public partial class ChatImageContainer : UserControl
    {
        public ChatImageContainer()
        {
            InitializeComponent();
            ImageGrid.DataContext = this;
        }

        public ObservableCollection<string> List_SelectedImages
        {
            get
            {
                return (ObservableCollection<string>)GetValue(list_SelectedImagesProperty);
            }
            set
            {
                SetValue(list_SelectedImagesProperty, value);
            }
        }


        public static readonly DependencyProperty list_SelectedImagesProperty =
            DependencyProperty.Register("List_SelectedImages", typeof(ObservableCollection<string>), typeof(ChatImageContainer), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        public int ImageWidth
        {
            get
            {
                return (int)GetValue(ImageWidthProperty);
            }
            set
            {
                SetValue(ImageWidthProperty, value);
            }
        }


        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(int), typeof(ChatImageContainer), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        public int ImageHeight
        {
            get
            {
                return (int)GetValue(ImageHeightProperty);
            }
            set
            {
                SetValue(ImageHeightProperty, value);
            }
        }


        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(int), typeof(ChatImageContainer), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });


        public Brush ContolBackground
        {
            get
            {
                return (Brush)GetValue(ContolBackgroundProperty);
            }
            set
            {
                SetValue(ContolBackgroundProperty, value);
            }
        }


        public static readonly DependencyProperty ContolBackgroundProperty =
            DependencyProperty.Register("ContolBackground", typeof(Brush), typeof(ChatImageContainer), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        public int RadiusX
        {
            get
            {
                return (int)GetValue(RadiusXProperty);
            }
            set
            {
                SetValue(RadiusXProperty, value);
            }
        }


        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register("RadiusX", typeof(int), typeof(ChatImageContainer), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        public int RadiusY
        {
            get
            {
                return (int)GetValue(RadiusYProperty);
            }
            set
            {
                SetValue(RadiusYProperty, value);
            }
        }


        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register("RadiusY", typeof(int), typeof(ChatImageContainer), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });


      

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Breakpoint here to see if the new value is being set
            var newValue = e.NewValue;
        }

        private void RemoveImageFromList(object sender, RoutedEventArgs e)
        {
            var currentImage = ((Button)sender).DataContext;
            List_SelectedImages.Remove(currentImage.ToString());
        }
    }
}
