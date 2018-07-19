using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Command;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ConfigControl
{
    /// <summary>
    /// Interaction logic for SocinatorMacros.xaml
    /// </summary>
    public partial class SocinatorMacros : UserControl, INotifyPropertyChanged
    {
        private SocinatorMacros()
        {
            InitializeComponent();
            SocinatorsMacros.DataContext = SocinatorMacrosViewModel;
        }

        private static SocinatorMacros _objSocinatorMacros;

        public static SocinatorMacros GetSingeltonSocinatorMacros()
        {
            if (_objSocinatorMacros == null)
                _objSocinatorMacros = new SocinatorMacros();
            _objSocinatorMacros.SocinatorMacrosViewModel.InitializeMacros();
            return _objSocinatorMacros;
        }

        private SocinatorMacrosViewModel _socinatorMacrosViewModel = new SocinatorMacrosViewModel();

        public SocinatorMacrosViewModel SocinatorMacrosViewModel
        {
            get
            {
                return _socinatorMacrosViewModel;
            }
            set
            {
                _socinatorMacrosViewModel = value;
                OnPropertyChanged(nameof(SocinatorMacrosViewModel));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SocinatorMacrosViewModel : BindableBase
    {

        #region Constructor

        public SocinatorMacrosViewModel()
        {
            SaveMacrosCommand = new BaseCommand<object>(SaveMacrosCanExecute, SaveMacrosExecute);
            DeleteCommand = new BaseCommand<object>(DeleteMacrosCanExecute, DeleteMacrosExecute);
            ImportMacrosCammand = new BaseCommand<object>(ImportMacrosCanExecute, ImportMacrosExecute);
        }

        public void InitializeMacros()
        {
            MacrosCollection = new ObservableCollection<SocinatorIntellisenseModel>(GenericFileManager.GetModuleDetails<SocinatorIntellisenseModel>(ConstantVariable.GetMacroDetails));
        }

        #endregion

        #region Properties

        public ICommand SaveMacrosCommand { get; set; }

        public ICommand DeleteCommand { get; set; }

        public ICommand ImportMacrosCammand { get; set; }


        private SocinatorIntellisenseModel _inputMacro = new SocinatorIntellisenseModel();

        public SocinatorIntellisenseModel InputMacro
        {
            get { return _inputMacro; }
            set
            {
                if (_inputMacro == value)
                    return;
                SetProperty(ref _inputMacro, value);
            }
        }

        private ObservableCollection<SocinatorIntellisenseModel> _macrosCollection =
            new ObservableCollection<SocinatorIntellisenseModel>();

        public ObservableCollection<SocinatorIntellisenseModel> MacrosCollection
        {
            get { return _macrosCollection; }
            set
            {
                if (value == _macrosCollection)
                    return;
                SetProperty(ref _macrosCollection, value);
            }
        }

        #endregion

        #region Methods

        public bool SaveMacrosCanExecute(object sender) => true;

        public void SaveMacrosExecute(object sender)
        {
            var socinatorIntellisenseModel = ((FrameworkElement)sender).DataContext as SocinatorMacrosViewModel;

            if (socinatorIntellisenseModel == null)
                return;

            if (!string.IsNullOrEmpty(socinatorIntellisenseModel.InputMacro.Key))
            {
                MacrosCollection.Add(socinatorIntellisenseModel.InputMacro);
                GenericFileManager.AddModule(socinatorIntellisenseModel.InputMacro, ConstantVariable.GetMacroDetails);
            }

            InputMacro = new SocinatorIntellisenseModel();
        }

        public bool DeleteMacrosCanExecute(object sender) => true;

        public void DeleteMacrosExecute(object sender)
        {
            var socinatorIntellisenseModel =
                ((FrameworkElement)sender).DataContext as SocinatorIntellisenseModel;

            MacrosCollection.Remove(socinatorIntellisenseModel);

            GenericFileManager.Delete<SocinatorIntellisenseModel>(x=> socinatorIntellisenseModel != null && x.Key == socinatorIntellisenseModel.Key, ConstantVariable.GetMacroDetails);
        }


        public bool ImportMacrosCanExecute(object sender) => true;

        public void ImportMacrosExecute(object sender)
        {
            var loadedMacroslist = FileUtilities.FileBrowseAndReader();

            loadedMacroslist?.ForEach(macros =>
            {
                var inputMacro = macros.Replace(",", ":");
                var splitMacros = Regex.Split(inputMacro, ":");

                if (splitMacros.Length == 2)
                {
                    var isPresent = MacrosCollection.Any(x => x.Key == splitMacros[0]);
                    if(isPresent)                   
                        GlobusLogHelper.log.Info($"Macro key : {splitMacros[0]} already present!");                    
                    else                   
                        MacrosCollection.Add(new SocinatorIntellisenseModel{Key = splitMacros[0] ,Value= splitMacros[1] });                    
                }
            });

            GenericFileManager.UpdateModuleDetails(MacrosCollection.ToList(), ConstantVariable.GetMacroDetails);
        }

        #endregion

    }

}
