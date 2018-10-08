using DominatorHouse.PopUpStyle;
using DominatorUIUtility.Navigations;
using DominatorUIUtility.ScreenTip.ViewModel;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using DominatorUIUtility.ScreenTip.PopUpstyle;

namespace DominatorHouse.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : DominatorUIUtility.ScreenTip.ViewModel.ViewModelBase
    {
        private readonly PopupStyle myPopupStyle = new PopupStyle();
      
        private ICommand _cmdStartLoginAccounts;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            FeatureTour.SetViewModelFactoryMethod(tourRun => new CustomTourViewModel(tourRun));

            var navigator = FeatureTour.GetNavigator();
          
        }

    


       
        public static MainViewModel Instance { get; } = new MainViewModel();

      
    }
}