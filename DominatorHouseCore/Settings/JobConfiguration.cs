using System.Collections.Generic;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Settings
{


    /// <summary> 
    /// Dont Use this, Instead use  DominatorHouseCore.Model.JobConfiguration
    /// </summary>
    [ProtoContract]
    public  class JobConfiguration : BindableBase
    {

        private RangeUtilities _operationCountPerJob = new RangeUtilities();

        [ProtoMember(1)]
        // Specify the operation count for the job in numbers
        public RangeUtilities OperationCountPerJob
        {
            get
            {
                return _operationCountPerJob;
            }
            set
            {
                if (value == _operationCountPerJob)
                    return;
                SetProperty(ref _operationCountPerJob, value);
            }
        }

       private RangeUtilities _delayBetweenJobs = new RangeUtilities();

        [ProtoMember(3)]
        // Specify the delay range for the jobs in minutes
        public RangeUtilities DelayBetweenJobs {
            get
            {
                return _delayBetweenJobs;
            }
            set
            {
                if (value == _delayBetweenJobs)
                    return;
                SetProperty(ref _delayBetweenJobs, value);
            }
        }

        private RangeUtilities _delayBetweenOperation= new RangeUtilities();

        [ProtoMember(4)]
        // Specify the delay range for the operations in seconds
        public RangeUtilities DelayBetweenOperation
        {
            get
            {
                return _delayBetweenOperation;
            }
            set
            {
                if (value == _delayBetweenOperation)
                    return;
                SetProperty(ref _delayBetweenOperation, value);
            }
        }


        // Specify the current job configurations are enable or not  
        private bool _isJobConfigEnabled;

        [ProtoMember(5)]
        public bool IsJobConfigEnabled
        {
            get
            {
                return this._isJobConfigEnabled;
            }
            set
            {
                if (value == _isJobConfigEnabled)
                    return;
                SetProperty(ref _isJobConfigEnabled, value);
            }
        }


        [ProtoMember(6)]
        // Specify the operation count per day in numbers
        public RangeUtilities OperationCountPerDay { get; set; } = new RangeUtilities();


        [ProtoMember(7)]
        // Specify the operation count per hours in numbers
        public RangeUtilities OperationCountPerHour { get; set; } = new RangeUtilities();


        [ProtoMember(8)]
        // Specify the operation count per week in numbers
        public RangeUtilities OperationCountPerWeek { get; set; } = new RangeUtilities();

        private bool _pauseCurrentJob;

        [ProtoMember(12)]
        // Whether the current job is paused or not
        public bool PauseCurrentJob
        {
            get
            {
                return _pauseCurrentJob;
            }
            set
            {
                if (value == _pauseCurrentJob)
                    return;
                SetProperty(ref _pauseCurrentJob, value);
            }
        }
        private int _pauseCurrentJobUntill;

        [ProtoMember(11)]
        // Specify the timespan value to untill the job is paused
        public int PauseCurrentJobUntill
        {
            get
            {
                return _pauseCurrentJobUntill;
            }
            set
            {
                if (value == _pauseCurrentJobUntill)
                    return;
                SetProperty(ref _pauseCurrentJobUntill, value);
            }
        }


        [ProtoMember(13)]
        // Specify the restricted user list
        public BlacklistSettings RestrictedUserList { get; set; } = new BlacklistSettings();


        [ProtoMember(14)]
        // Specify the restricted group list
        public BlacklistSettings RestrictedGroupList { get; set; } = new BlacklistSettings();


        [ProtoMember(9, OverwriteList = true)]
        // Specify the running timespam for the job 
        public List<RunningTimeSpanModel> RunningTimeSpan { get; set; } = RunningTimeSpanModel.DayWiseRunningTimeSpan;

        private bool _shouldSchedule;

        [ProtoMember(10)]
        // Is the job is going for next schedule in the day, verify the max hour or day or week count is already crossed or not
        public bool ShouldSchedule
        {
            get
            {
                return _shouldSchedule;
            }
            set
            {
                if (value == _shouldSchedule)
                    return;
                SetProperty(ref _shouldSchedule, value);
            }
        }


        // Specify the current status of the job
        public  string JobCurrentStatus { get; }

    }
}
