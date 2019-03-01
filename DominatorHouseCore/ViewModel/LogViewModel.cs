using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel.Common;
using NLog;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace DominatorHouseCore.ViewModel
{
    public interface ILogViewModel
    {
        void Add(string message, LogLevel logLevel);
    }

    public class LogViewModel : BindableBase, ILogViewModel
    {
        private const int MaxLogSize = 1000;
        private LoggerModel _selected;
        private SocialNetworks? _selectedNetwork;
        private ObservableCollection<LoggerModel> _logs;

        public object SyncObject { get; }
        public ObservableCollection<LoggerModel> Logs
        {
            get { return _logs; }
            set { SetProperty(ref _logs, value, nameof(Logs)); }
        }

        public LoggerModel Selected
        {
            get { return _selected; }
            set
            {
                SetProperty(ref _selected, value, nameof(Selected));
                CopyCmd.RaiseCanExecuteChanged();
            }
        }

        public SocialNetworks? SelectedNetwork
        {
            get { return _selectedNetwork; }
            set
            {
                SetProperty(ref _selectedNetwork, value, nameof(SelectedNetwork));
                OnPropertyChanged(nameof(NetworkIsSelected));
                ActivityTypes.Selected = null;
            }
        }

        public bool NetworkIsSelected
        {
            get { return SelectedNetwork.HasValue; }
        }

        public DelegateCommand CopyCmd { get; set; }
        public SelectableViewModel<ActivityType?> ActivityTypes { get; }

        public LogViewModel()
        {
            SyncObject = new object();
            Logs = new ObservableCollection<LoggerModel>();
            CopyCmd = new DelegateCommand(Copy, CanCopy);
            BindingOperations.EnableCollectionSynchronization(Logs, SyncObject);
            ActivityTypes =
                new SelectableViewModel<ActivityType?>(Enum.GetValues(typeof(ActivityType)).Cast<ActivityType?>());
        }

        private bool CanCopy()
        {
            return Selected != null;
        }

        public void Add(string message, LogLevel logLevel)
        {
            lock (SyncObject)
            {

                var messages = message.Split('\t');

                var log = messages.Length == 5
                    ? new LoggerModel
                    {
                        DateTime = DateTime.Now,
                        Network = messages[0].Trim(),
                        AccountCampaign = messages[1].Trim(),
                        ActivityType = messages[2].Trim(),
                        Message = messages[3].Trim(),
                        MessageCode = messages[4].Trim(),
                        LogType = logLevel.ToString()
                    }
                    : new LoggerModel
                    {
                        Network = SocialNetworks.Social.ToString(),
                        DateTime = DateTime.Now,
                        Message = message,
                        LogType = logLevel.ToString()
                    };

                Logs.Insert(0, log);

                OnPropertyChanged(nameof(Logs));

                if (Logs.Count > MaxLogSize)
                    Logs.RemoveAt(Logs.Count - 1);
            }
        }

        private void Copy()
        {
            if (Selected != null)
            {
                Clipboard.SetText(Selected.Message);
                ToasterNotification.ShowSuccess("Message copied");
            }
        }
    }
}
