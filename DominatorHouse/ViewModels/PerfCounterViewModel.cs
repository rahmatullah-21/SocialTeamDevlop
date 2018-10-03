using DominatorHouseCore.Utility;
using Prism.Commands;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Windows;
using System.Windows.Threading;

namespace DominatorHouse.ViewModels
{
    public interface IPerfCounterViewModel : IDisposable
    {

    }

    public class PerfCounterViewModel : BindableBase, IPerfCounterViewModel
    {
        private readonly DispatcherTimer _timer;
        private GridLength _logViewHeight;

        private static PerformanceCounter PerformanceCounter { get; }
            = new PerformanceCounter("Memory", "Available MBytes");

        private static ManagementObject Processor { get; }
            = new ManagementObject("Win32_PerfFormattedData_PerfOS_Processor.Name='_Total'");

        public string LoadedMemory { get; }

        public string AvailableMemory
        {
            get { return GetMemoryUsage().ToString(CultureInfo.InvariantCulture); }
        }

        public string CpuUsage
        {
            get { return GetCpuUsage(); }
        }

        public string CurrentDateTime
        {
            get { return DateTime.Now.ToString(CultureInfo.InvariantCulture); }
        }

        public GridLength LogViewHeight
        {
            get { return _logViewHeight; }
            set { SetProperty(ref _logViewHeight, value, nameof(LogViewHeight)); }
        }

        public DelegateCommand ShowHideLogCmd { get; }

        public PerfCounterViewModel()
        {
            LogViewHeight = new GridLength(3, GridUnitType.Star);
            LoadedMemory = GetRamsize();
            ShowHideLogCmd = new DelegateCommand(ShowHideLog);
            _timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += TimexTick;
            _timer.Start();
        }

        private void ShowHideLog()
        {
            if (LogViewHeight.Value <= 200 && LogViewHeight.Value > 45)
                LogViewHeight = new GridLength(45);
            else
                LogViewHeight = new GridLength(200);
        }

        private void TimexTick(object sender, EventArgs eventArgs)
        {
            OnPropertyChanged(nameof(AvailableMemory));
            OnPropertyChanged(nameof(CpuUsage));
            OnPropertyChanged(nameof(CurrentDateTime));
        }


        private static string GetRamsize()
        {
            var objManagementClass = new ManagementClass("Win32_ComputerSystem");
            var objManagementObjectCollection = objManagementClass.GetInstances();
            foreach (var item in objManagementObjectCollection)
                return Convert.ToString(
                           Math.Round(Convert.ToDouble(item.Properties["TotalPhysicalMemory"].Value) / 1048576, 0),
                           CultureInfo.InvariantCulture) + " MB";

            return "0 MB";
        }


        private static string GetCpuUsage()
        {
            try
            {
                Processor.Get();
                return Processor.Properties["PercentProcessorTime"].Value.ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        private static double GetMemoryUsage()
        {
            var memAvailable = (double)PerformanceCounter.NextValue();
            return memAvailable;
        }

        public void Dispose()
        {
            _timer.Tick -= TimexTick;
            this._timer.Stop();
        }
    }
}
