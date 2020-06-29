using DominatorHouse.Window;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using System.Windows;

namespace DominatorHouseCore.Models
{
    public class WindowModel : BindableBase
    {
        #region Private Member

        /// <summary>
        /// The window this view model controls
        /// </summary>
        public Window mWindow { get; set; }

        /// <summary>
        /// The window resizer helper that keeps the window size correct in various states
        /// </summary>
        private WindowResizer _windowResizer;

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        private int mOuterMarginSize = 10;

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        private int mWindowRadius = 10;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition mDockPosition = WindowDockPosition.Undocked;


        private bool _isIconVisible = false;

        /// <summary>
        /// True if we should have a dimmed overlay on the window
        /// such as when a popup is visible or the window is not focused
        /// </summary>

        private int _resizeBorder;

        private bool _borderless;

        private int _titleHeight = 55;

        private WindowState _windowState;

        private bool _dimmableOverlayVisible;

        private double _windowMinimumWidth = 800;

        private Thickness _resizeBorderThickness;

        private double _windowMinimumHeight = 600;

        private GridLength _titleHeightGridLength;

        private CornerRadius _windowCornerRadius;

        private Thickness _outerMarginSizeThickness = new Thickness(10);

        #endregion

        #region Public Properties


        public WindowDockPosition MdockPosition
        {
            get
            {
                return mDockPosition;
            }
            set
            {
                SetProperty(ref mDockPosition, value);
                Borderless = mWindow.WindowState == WindowState.Maximized || MdockPosition != WindowDockPosition.Undocked;
            }
        }

        public WindowState WindowState
        {
            get
            {
                return _windowState;
            }
            set
            {
                SetProperty(ref _windowState, value);
                mWindow.WindowState = value;
                Borderless = mWindow.WindowState == WindowState.Maximized || MdockPosition != WindowDockPosition.Undocked;
            }
        }


        /// <summary>
        /// The smallest width the window can go to
        /// </summary>
        public double WindowMinimumWidth
        {
            get
            {
                return _windowMinimumWidth;
            }
            set
            {
                SetProperty(ref _windowMinimumWidth, value);
            }
        }

        /// <summary>
        /// The smallest height the window can go to
        /// </summary>
        public double WindowMinimumHeight
        {
            get
            {
                return _windowMinimumHeight;
            }
            set
            {
                SetProperty(ref _windowMinimumHeight, value);
            }
        }

        /// <summary>
        /// True if the window should be borderless because it is docked or maximized
        /// </summary>
        public bool Borderless
        {
            get
            {
                return _borderless;
            }
            set
            {
                SetProperty(ref _borderless, value);
                ResizeBorder = Borderless ? 0 : 6;
                WindowRadius = Borderless ? 0 : mWindowRadius;
                OuterMarginSize = Borderless ? 0 : mOuterMarginSize;
            }
        }

        /// <summary>
        /// The size of the resize border around the window
        /// </summary>
        public int ResizeBorder
        {
            get
            {
                return _resizeBorder;
            }
            set
            {
                SetProperty(ref _resizeBorder, value);
            }
        }

        public WindowResizer WindowResizer
        {
            get
            {
                // If it is maximized or docked, no border
                return _windowResizer;
            }
            set
            {
                SetProperty(ref _windowResizer, value);
            }
        }


        public bool IsIconVisible
        {
            get
            {
                // If it is maximized or docked, no border
                return _isIconVisible;
            }
            set
            {
                SetProperty(ref _isIconVisible, value);
            }
        }



        private SelectedMenuItem _selectedMenuItem = SelectedMenuItem.DashBoard;

        public SelectedMenuItem SelectedMenuItem
        {
            get
            {
                return _selectedMenuItem;
            }
            set
            {
                SetProperty(ref _selectedMenuItem, value);
            }
        }

        /// <summary>
        /// The size of the resize border around the window, taking into account the outer margin
        /// </summary>
        public Thickness ResizeBorderThickness
        {
            get
            {
                return _resizeBorderThickness;
            }
            set
            {
                SetProperty(ref _resizeBorderThickness, value);
            }
        }



        //=> new Thickness(ResizeBorder + OuterMarginSize);

        /// <summary>
        /// The padding of the inner content of the main window
        /// </summary>
        public Thickness InnerContentPadding { get; set; } = new Thickness(0);

        /// <summary>
        /// True if we should have a dimmed overlay on the window
        /// such as when a popup is visible or the window is not focused
        /// </summary>
        public bool DimmableOverlayVisible
        {
            get
            {
                return _dimmableOverlayVisible;
            }
            set
            {
                SetProperty(ref _dimmableOverlayVisible, value);
            }
        }

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public int OuterMarginSize
        {
            get
            {
                return mOuterMarginSize;
            }
            set
            {
                SetProperty(ref mOuterMarginSize, value);
                OuterMarginSizeThickness = new Thickness(value);
            }
            // If it is maximized or docked, no border           
        }

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public Thickness OuterMarginSizeThickness
        {
            get
            {
                return _outerMarginSizeThickness;
            }
            set
            {
                SetProperty(ref _outerMarginSizeThickness, value);
            }
            // If it is maximized or docked, no border           
        }

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        public int WindowRadius
        {
            // If it is maximized or docked, no border
            get
            {
                return mWindowRadius;
            }
            set
            {
                SetProperty(ref mWindowRadius, value);
                WindowCornerRadius = new CornerRadius(WindowRadius);
            }
        }

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        public CornerRadius WindowCornerRadius
        {
            get
            {
                return _windowCornerRadius;
            }
            set
            {
                SetProperty(ref _windowCornerRadius, value);
            }
        }



        //new CornerRadius(WindowRadius);

        /// <summary>
        /// The height of the title bar / caption of the window
        /// </summary>
        public int TitleHeight
        {
            get
            {
                return _titleHeight;
            }
            set
            {
                SetProperty(ref _titleHeight, value);
                TitleHeightGridLength = new GridLength(TitleHeight + ResizeBorder);
            }
        }

        /// <summary>
        /// The height of the title bar / caption of the window
        /// </summary>
        public GridLength TitleHeightGridLength
        {
            get
            {
                return _titleHeightGridLength;
            }
            set
            {
                SetProperty(ref _titleHeightGridLength, value);
            }
        }


        #endregion
    }
}
//=> new GridLength(TitleHeight + ResizeBorder);