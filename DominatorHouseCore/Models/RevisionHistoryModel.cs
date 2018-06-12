using System;
using System.Collections.Generic;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Models
{
   public class RevisionHistoryModel:BindableBase
    {
        private string _version=String.Empty;
        public string Version
        {
            get { return _version; }
            set
            {
                if (_version != null && value == _version)
                    return;
                SetProperty(ref _version, value);
            }
        }
        private string _revisionDate = String.Empty;
        public string RevisionDate
        {
            get { return _revisionDate; }
            set
            {
                if (_revisionDate != null && value == _revisionDate)
                    return;
                SetProperty(ref _revisionDate, value);
            }
        }
        private List<string> _lstContent = new List<string>();
        public List<string> LstContent
        {
            get { return _lstContent; }
            set
            {
                if (_lstContent != null && value == _lstContent)
                    return;
                SetProperty(ref _lstContent, value);
            }
        }
    }
}
