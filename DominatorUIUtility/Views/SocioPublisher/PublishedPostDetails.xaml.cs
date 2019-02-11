using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublishedPostDetails.xaml
    /// </summary>
    public partial class PublishedPostDetails : UserControl, INotifyPropertyChanged
    {
        private PublishedPostDetailsViewModel _publishedPostDetailsViewModel = new PublishedPostDetailsViewModel();

        public PublishedPostDetailsViewModel PublishedPostDetailsViewModel
        {
            get { return _publishedPostDetailsViewModel; }
            set
            {
                if (_publishedPostDetailsViewModel == value)
                    return;
                _publishedPostDetailsViewModel = value;
                OnPropertyChanged(nameof(PublishedPostDetailsViewModel));
            }
        }

        public PublishedPostDetails()
        {
            InitializeComponent();
        }

        public PublishedPostDetails(PublisherPostlistModel currentData) : this()
        {
            try
            {
                PublishedPostDetailsViewModel.PublisherPostlist = PostlistFileManager.GetByPostId(currentData.CampaignId, currentData.PostId);
                DataContext = PublishedPostDetailsViewModel;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CopyCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CopyExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            try
            {
                ListView lb = (ListView)(sender);
                var postLink = (lb?.SelectedItem as PublishedPostDetailsModel).Link;
                if (!string.IsNullOrEmpty(postLink))
                {
                    Clipboard.SetText(postLink);
                    ToasterNotification.ShowSuccess("Message copied");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
