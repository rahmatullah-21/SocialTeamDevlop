using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherCustomDestinationViewModel : BindableBase
    {

        #region Constructor

        public PublisherCustomDestinationViewModel()
        {
            SaveDestinationCommand = new BaseCommand<object>(SaveCustomDestinationCanExecute, SaveCustomDestinationExecute);
            DeleteCommand = new BaseCommand<object>(DeleteDestinationCanExecute, DeleteDestinationExecute);
        }

        #endregion

        #region Properties

        public ICommand SaveDestinationCommand { get; set; }

        public ICommand DeleteCommand { get; set; }




        private PublisherCustomDestinationModel _inputDestination = new PublisherCustomDestinationModel();

        public PublisherCustomDestinationModel InputDestination
        {
            get { return _inputDestination; }
            set
            {
                if (_inputDestination == value)
                    return;
                SetProperty(ref _inputDestination, value);
            }
        }

        private ObservableCollection<PublisherCustomDestinationModel> _lstCustomDestination =
            new ObservableCollection<PublisherCustomDestinationModel>();

        public ObservableCollection<PublisherCustomDestinationModel> LstCustomDestination
        {
            get { return _lstCustomDestination; }
            set
            {
                if (value == _lstCustomDestination)
                    return;
                SetProperty(ref _lstCustomDestination, value);
            }
        }

        #endregion

        #region Methods

        public bool SaveCustomDestinationCanExecute(object sender) => true;

        public void SaveCustomDestinationExecute(object sender)
        {
            var publisherCustomDestinationModel =
                ((FrameworkElement)sender).DataContext as PublisherCustomDestinationViewModel;

            if (publisherCustomDestinationModel != null)
            {
                if (publisherCustomDestinationModel.InputDestination.DestinationValue.Contains("\r\n"))
                {
                    var splittedValue = Regex
                        .Split(publisherCustomDestinationModel.InputDestination.DestinationValue, "\r\n").ToList();

                    splittedValue.ForEach(x =>
                    {
                        if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(publisherCustomDestinationModel.InputDestination.DestinationType))
                            return;

                        if (LstCustomDestination.All(y =>
                            y.DestinationType != publisherCustomDestinationModel.InputDestination.DestinationType &&
                            y.DestinationValue != publisherCustomDestinationModel.InputDestination.DestinationValue))
                        {
                            LstCustomDestination.Add(new PublisherCustomDestinationModel()
                            {
                                DestinationType = publisherCustomDestinationModel.InputDestination.DestinationType,
                                DestinationValue = x
                            });
                        }
                    });
                }
                else
                {
                    if (!string.IsNullOrEmpty(publisherCustomDestinationModel.InputDestination.DestinationValue) &&
                        !string.IsNullOrEmpty(publisherCustomDestinationModel.InputDestination.DestinationType))
                    {
                        if (LstCustomDestination.All(x => x.DestinationType != publisherCustomDestinationModel.InputDestination.DestinationType && x.DestinationValue != publisherCustomDestinationModel.InputDestination.DestinationValue))
                            LstCustomDestination.Add(publisherCustomDestinationModel.InputDestination);
                    }
                }

                InputDestination = new PublisherCustomDestinationModel();
            }
        }

        public bool DeleteDestinationCanExecute(object sender) => true;

        public void DeleteDestinationExecute(object sender)
        {
            var publisherCustomDestinationModel =
                ((FrameworkElement)sender).DataContext as PublisherCustomDestinationModel;

            LstCustomDestination.Remove(publisherCustomDestinationModel);
        }


        #endregion

    }
}