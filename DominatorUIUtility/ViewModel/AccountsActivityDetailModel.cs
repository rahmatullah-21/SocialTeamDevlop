using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;

namespace DominatorUIUtility.ViewModel
{

    public class AccountsActivityDetailModel
    {
        public DominatorAccountModel DominatorAccountModel { get; set; }

        public ObservableCollection<AutoActivityModuleDetails> AutoActivityModuleDetailsCollections { get; set; }

    }


    public class ActivityDetailsModel : BindableBase
    {

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title == value)
                    return;
                SetProperty(ref _title, value);
            }
        }


        private string _status;
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (_status == value)
                    return;
                SetProperty(ref _status, value);
            }
        }

        private ActivityRatio _ratio = new ActivityRatio();

        public ActivityRatio Ratio
        {
            get
            {
                return _ratio;
            }
            set
            {
                if (_ratio == value)
                    return;
                SetProperty(ref _ratio, value);
            }
        }
    }

    public class ActivityRatio : BindableBase
    {
        private int _total;
        private int _completed;
        private double _percentage;

        public int Total
        {
            get
            {
                return _total;
            }
            set
            {
                if (_total == value)
                    return;
                SetProperty(ref _total, value);
              
            }
        }

        public int Completed
        {
            get
            {
                return _completed;
            }
            set
            {
                if (_completed == value)
                    return;
                SetProperty(ref _completed, value);
                GetPercentages();
            }
        }

        public double Percentage
        {
            get
            {
                return _percentage;
            }
            set
            {
                SetProperty(ref _percentage, value);

            }
        }

        private void GetPercentages()
        {
            var value = (double)(((double)Completed / (double)Total) * 100);
            Percentage = System.Math.Round(value, 2);
        }
    }

}
