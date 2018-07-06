using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Models
{
    public class LoggerModel : BindableBase
    {
       
        private DateTime _dateTime;

        public DateTime DateTime
        {
            get { return _dateTime; }
            set
            {
                if (_dateTime != null && value == _dateTime)
                    return;
                SetProperty(ref _dateTime, value);
            }
        }
        private string _network;

        public string Network
        {
            get { return _network; }
            set
            {
                if (_network != null && value == _network)
                    return;
                SetProperty(ref _network, value);
            }
        }
        private string _activityType;

        public string ActivityType
        {
            get { return _activityType; }
            set
            {
                if (_activityType != null && value == _activityType)
                    return;
                SetProperty(ref _activityType, value);
            }
        }
        private string _accountCampaign;

        public string AccountCampaign
        {
            get { return _accountCampaign; }
            set
            {
                if (_accountCampaign != null && value == _accountCampaign)
                    return;
                SetProperty(ref _accountCampaign, value);
            }
        }
        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                if (_message != null && value == _message)
                    return;
                SetProperty(ref _message, value);
            }
        }
        private string _logType = "Info";

        public string LogType
        {
            get { return _logType; }
            set
            {
                if (_logType != null && value == _logType)
                    return;
                SetProperty(ref _logType, value);
            }
        }
        private string _messageCode ;

        public string MessageCode
        {
            get { return _messageCode; }
            set
            {
                if (_messageCode != null && value == _messageCode)
                    return;
                SetProperty(ref _messageCode, value);
            }
        }
    }
}
