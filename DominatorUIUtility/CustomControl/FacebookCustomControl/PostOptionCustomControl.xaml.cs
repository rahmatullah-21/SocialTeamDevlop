using DominatorHouseCore.Models.FacebookModels;
using System.Windows;

namespace DominatorUIUtility.CustomControl.FacebookCustomControl
{
    /// <summary>
    /// Interaction logic for PostOptionCustomControl.xaml
    /// </summary>
    public partial class PostOptionCustomControl
    {
        public PostOptionCustomControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        public PostLikeCommentorModel PostLikeCommentorModel
        {
            get { return (PostLikeCommentorModel)GetValue(PostLikerCommentorProperty); }
            set { SetValue(PostLikerCommentorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostLikerCommentorProperty =
            DependencyProperty.Register("PostLikeCommentorModel", typeof(PostLikeCommentorModel), typeof(PostOptionCustomControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }


        // Using a DependencyProperty as the backing store for SaveCommand.  This enables animation, styling, binding, etc...
//        public static readonly DependencyProperty PostOptionCheckedChangedCommandProperty =
//            DependencyProperty.Register("PostOptionCheckedChangedCommand", typeof(ICommand), typeof(PostOptionCustomControl));
//        public object CommandParameterNew
//        {
//            get { return GetValue(CommandParameterNewProperty); }
//            set { SetValue(CommandParameterNewProperty, value); }
//        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
//        public static readonly DependencyProperty CommandParameterNewProperty =
//            DependencyProperty.Register("CommandParameterNew", typeof(object), typeof(PostOptionCustomControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged));


        public bool IsAlbumsNeeded
        {
            get { return (bool)GetValue(IsAlbumsNeededProperty); }
            set { SetValue(IsAlbumsNeededProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SaveCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAlbumsNeededProperty =
            DependencyProperty.Register("IsAlbumsNeeded", typeof(bool), typeof(PostOptionCustomControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });


    }
}
