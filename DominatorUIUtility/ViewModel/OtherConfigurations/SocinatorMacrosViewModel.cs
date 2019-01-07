using CommonServiceLocator;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace DominatorUIUtility.ViewModel.OtherConfigurations
{
    public class SocinatorMacrosViewModel : BaseTabViewModel, IOtherConfigurationViewModel
    {
        private readonly IGenericFileManager _genericFileManager;
        private readonly IConstantVariable _constantVariable;
        private ObservableCollection<SocinatorIntellisenseModel> _macrosCollection =
            new ObservableCollection<SocinatorIntellisenseModel>();

        private SocinatorIntellisenseModel _inputMacro = new SocinatorIntellisenseModel();

        public ICommand SaveMacrosCommand { get; set; }

        public ICommand DeleteCommand { get; set; }

        public ICommand ImportMacrosCammand { get; set; }


        public SocinatorIntellisenseModel InputMacro
        {
            get { return _inputMacro; }
            set { SetProperty(ref _inputMacro, value); }
        }

        public ObservableCollection<SocinatorIntellisenseModel> MacrosCollection
        {
            get { return _macrosCollection; }
            set { SetProperty(ref _macrosCollection, value); }
        }

        #region Constructor

        public SocinatorMacrosViewModel(IGenericFileManager genericFileManager) : base("LangKeyMacroS", "SocinatorMacrosControlTemplate")
        {
            _genericFileManager = genericFileManager;
            _constantVariable = ServiceLocator.Current.GetInstance<IConstantVariable>();
            SaveMacrosCommand = new DelegateCommand<SocinatorMacrosViewModel>(SaveMacrosExecute);
            DeleteCommand = new DelegateCommand<SocinatorIntellisenseModel>(DeleteMacrosExecute);
            ImportMacrosCammand = new DelegateCommand(ImportMacrosExecute);
            MacrosCollection = new ObservableCollection<SocinatorIntellisenseModel>(_genericFileManager.GetModuleDetails<SocinatorIntellisenseModel>(_constantVariable.GetMacroDetails));
        }
        #endregion

        #region Methods

        public void SaveMacrosExecute(SocinatorMacrosViewModel sender)
        {
            var socinatorIntellisenseModel = sender;

            if (socinatorIntellisenseModel == null)
                return;

            if (!string.IsNullOrEmpty(socinatorIntellisenseModel.InputMacro.Key))
            {
                MacrosCollection.Add(socinatorIntellisenseModel.InputMacro);
                _genericFileManager.AddModule(socinatorIntellisenseModel.InputMacro, _constantVariable.GetMacroDetails);
            }

            InputMacro = new SocinatorIntellisenseModel();
        }

        public void DeleteMacrosExecute(SocinatorIntellisenseModel sender)
        {
            var socinatorIntellisenseModel = sender;

            MacrosCollection.Remove(socinatorIntellisenseModel);

            _genericFileManager.Delete<SocinatorIntellisenseModel>(x => socinatorIntellisenseModel != null && x.Key == socinatorIntellisenseModel.Key, _constantVariable.GetMacroDetails);
        }

        public void ImportMacrosExecute()
        {
            var loadedMacroslist = FileUtilities.FileBrowseAndReader();

            loadedMacroslist?.ForEach(macros =>
            {
                //var inputMacro = macros.Replace(",", ":");
                // var splitMacros = Regex.Split(inputMacro, ":");

                var splitMacros = Regex.Split(macros, "\t");

                if (splitMacros.Length == 2)
                {
                    var isPresent = MacrosCollection.Any(x => x.Key == splitMacros[0]);
                    if (isPresent)
                        GlobusLogHelper.log.Info($"Macro key : {splitMacros[0]} already present!");
                    else
                        MacrosCollection.Add(new SocinatorIntellisenseModel { Key = splitMacros[0], Value = splitMacros[1] });
                }
            });

            _genericFileManager.UpdateModuleDetails(MacrosCollection.ToList(), _constantVariable.GetMacroDetails);
        }

        #endregion
    }
}
