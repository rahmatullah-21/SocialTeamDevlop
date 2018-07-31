using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using ProtoBuf;
using System.Windows.Input;
using DominatorHouseCore.Command;
using Newtonsoft.Json;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for JobConfigControl.xaml
    /// </summary>
    public partial class JobConfigControl : UserControl, INotifyPropertyChanged
    {
        public JobConfigControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            InitilizeFavoriteTime();
            SelectFavoriteTime = new BaseCommand<object>((sender) => true, SelectFavoriteTimeExecute);
        }

        public JobConfiguration JobConfiguration
        {
            get { return (JobConfiguration)GetValue(JobConfigurationProperty); }
            set { SetValue(JobConfigurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for JobConfiguration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty JobConfigurationProperty =
            DependencyProperty.Register("JobConfiguration", typeof(JobConfiguration), typeof(JobConfigControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var model = ((FrameworkElement)((FrameworkElement)sender).DataContext).DataContext;

                if (JobConfiguration.SelectedItem == "Slow")
                {
                    var slowSpeed = ((dynamic)model).SlowSpeed;
                    JobConfiguration.ActivitiesPerDay = slowSpeed.ActivitiesPerDay;
                    JobConfiguration.ActivitiesPerHour = slowSpeed.ActivitiesPerHour;
                    JobConfiguration.ActivitiesPerWeek = slowSpeed.ActivitiesPerWeek;
                    JobConfiguration.ActivitiesPerJob = slowSpeed.ActivitiesPerJob;
                    JobConfiguration.DelayBetweenJobs = slowSpeed.DelayBetweenJobs;
                    JobConfiguration.DelayBetweenActivity = slowSpeed.DelayBetweenActivity;
                }
                else if (JobConfiguration.SelectedItem == "Medium")
                {
                    var mediumSpeed = ((dynamic)model).MediumSpeed;
                    JobConfiguration.ActivitiesPerDay = mediumSpeed.ActivitiesPerDay;
                    JobConfiguration.ActivitiesPerHour = mediumSpeed.ActivitiesPerHour;
                    JobConfiguration.ActivitiesPerWeek = mediumSpeed.ActivitiesPerWeek;
                    JobConfiguration.ActivitiesPerJob = mediumSpeed.ActivitiesPerJob;
                    JobConfiguration.DelayBetweenJobs = mediumSpeed.DelayBetweenJobs;
                    JobConfiguration.DelayBetweenActivity = mediumSpeed.DelayBetweenActivity;
                }
                else if (JobConfiguration.SelectedItem == "Fast")
                {
                    var fastSpeed = ((dynamic)model).FastSpeed;
                    JobConfiguration.ActivitiesPerDay = fastSpeed.ActivitiesPerDay;
                    JobConfiguration.ActivitiesPerHour = fastSpeed.ActivitiesPerHour;
                    JobConfiguration.ActivitiesPerWeek = fastSpeed.ActivitiesPerWeek;
                    JobConfiguration.ActivitiesPerJob = fastSpeed.ActivitiesPerJob;
                    JobConfiguration.DelayBetweenJobs = fastSpeed.DelayBetweenJobs;
                    JobConfiguration.DelayBetweenActivity = fastSpeed.DelayBetweenActivity;
                }
                else if (JobConfiguration.SelectedItem == "Superfast")
                {
                    var superfastSpeed = ((dynamic)model).SuperfastSpeed;
                    JobConfiguration.ActivitiesPerDay = superfastSpeed.ActivitiesPerDay;
                    JobConfiguration.ActivitiesPerHour = superfastSpeed.ActivitiesPerHour;
                    JobConfiguration.ActivitiesPerWeek = superfastSpeed.ActivitiesPerWeek;
                    JobConfiguration.ActivitiesPerJob = superfastSpeed.ActivitiesPerJob;
                    JobConfiguration.DelayBetweenJobs = superfastSpeed.DelayBetweenJobs;
                    JobConfiguration.DelayBetweenActivity = superfastSpeed.DelayBetweenActivity;
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ChkAdvance_OnChecked(object sender, RoutedEventArgs e)
        {
            Speed.SelectedIndex = -1;
        }

        private void BtnCreateFavorite_OnClick(object sender, RoutedEventArgs e)
        {
            string favoriteTimeName = "Favorite Time";
            while (true)
            {
                try
                {

                    favoriteTimeName = Dialog.GetInputDialog("Favorite Time", "Enter Favorite time", favoriteTimeName, "Save", "Cancel");
                    if (!string.IsNullOrEmpty(favoriteTimeName))
                    {
                        if (!LstFavoriteTime.Any(x => x.FavoriteTimeName == favoriteTimeName))
                        {
                            FavoriteTime favoriteTime = new FavoriteTime
                            {
                                FavoriteTimeName = favoriteTimeName,
                                LstFavoriteTimes = JobConfiguration.RunningTime
                            };
                            GenericFileManager.AddModule<FavoriteTime>(favoriteTime, ConstantVariable.GetFavoriteTimeFile());
                            LstFavoriteTime.Add(favoriteTime);
                            break;
                        }
                        else
                        {
                            var result = Dialog.ShowCustomDialog("Warning", $"Favorite Time with name {favoriteTimeName} already exist.\nDo you want to override ?", "Yes", "No");
                            if (result == MessageDialogResult.Affirmative)
                            {
                                var oldLstFavoriteTime = LstFavoriteTime.FirstOrDefault(x => x.FavoriteTimeName == favoriteTimeName);
                                oldLstFavoriteTime.LstFavoriteTimes = JobConfiguration.RunningTime;
                                GenericFileManager.UpdateModuleDetails<FavoriteTime>(LstFavoriteTime.ToList(), ConstantVariable.GetFavoriteTimeFile());
                                break;
                            }

                        }
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }


        }

        private void SelectFavoriteTimeExecute(object sender)
        {
            try
            {
                var selectedFavoriteTime = sender as FavoriteTime;

                JobConfiguration.RunningTime = selectedFavoriteTime.LstFavoriteTimes.DeepCloneObject();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        void InitilizeFavoriteTime()
        {
            try
            {
                var lstFavoriteTimes = GenericFileManager.GetModuleDetails<FavoriteTime>(ConstantVariable.GetFavoriteTimeFile());

                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    LstFavoriteTime.Clear();
                    lstFavoriteTimes.ForEach(x =>
                    {
                        LstFavoriteTime.Add(x);
                    });

                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }
        private void JobConfigControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitilizeFavoriteTime();
        }

        private ObservableCollection<FavoriteTime> _lstFavoriteTime = new ObservableCollection<FavoriteTime>();

        public ObservableCollection<FavoriteTime> LstFavoriteTime
        {
            get
            {
                return _lstFavoriteTime;
            }
            set
            {
                _lstFavoriteTime = value;
                OnPropertyChanged(nameof(LstFavoriteTime));
            }
        }
        public ICommand SelectFavoriteTime { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    [ProtoContract]
    public class FavoriteTime : INotifyPropertyChanged
    {
        [ProtoMember(1)]
        private string favoriteTimeName = String.Empty;
        public string FavoriteTimeName
        {
            get
            {
                return favoriteTimeName;
            }

            set
            {
                if (value == favoriteTimeName) return;
                favoriteTimeName = value;
                OnPropertyChanged(nameof(FavoriteTimeName));
            }
        }
        [ProtoMember(2)]
        public List<RunningTimes> _lstFavoriteTimes = new List<RunningTimes>();
        public List<RunningTimes> LstFavoriteTimes
        {
            get
            {
                return _lstFavoriteTimes;
            }

            set
            {
                if (value == _lstFavoriteTimes) return;
                _lstFavoriteTimes = value;
                OnPropertyChanged(nameof(LstFavoriteTimes));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
