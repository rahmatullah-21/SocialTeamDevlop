using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.Views.Publisher.AdvancedOptions;
using MahApps.Metro.Controls;
using DominatorHouseCore.FileManagers;

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    /// Interaction logic for AddPosts.xaml
    /// </summary>
    public partial class AddPosts : UserControl, INotifyPropertyChanged
    {
       
        private AddPosts()
        {
            InitializeComponent();
            ObjAddPosts = this;
            SetDataContext();
        }

        public void SetDataContext()
        {
            AddPostViewModel = new AddPostViewModel();
            AddPostViewModel.AddPostModel.CampaignDetails.LstStatus = new ObservableCollection<string>()
                    {
                    FindResource("langAll").ToString(),
                    FindResource("langDraft").ToString(),
                    FindResource("langPending").ToString(),
                    FindResource("langPublished").ToString()
                    };

            var postDetails = PostFileManager.GetAllPost();
            postDetails.ForEach(x =>
            {
                AddPostViewModel.AddPostModel.CampaignDetails.LstCampaign.Add(new Campaign { CampaignName = x.CampaignDetails.CampaignName });
            });
            MainGrid.DataContext = AddPostViewModel.AddPostModel;

            Campaigns ObjCampaigns = Campaigns.GetSingltonCreateCampaignObject();
            
            ObjCampaigns.createCampign.DataContext = AddPostViewModel.AddPostModel.CampaignDetails;

           Home ObjPublisher = Home.GetSingletonHome();
            ObjPublisher.PublisherDetailCollection = CollectionViewSource.GetDefaultView(postDetails);
            ObjPublisher.publisherDetail.ItemsSource = ObjPublisher.PublisherDetailCollection;

        }

        private void btnAddToDrafts_Click(object sender, RoutedEventArgs e)
        {
            Campaigns ObjCampaigns = Campaigns.GetSingltonCreateCampaignObject();

            //  AddPostViewModel.AddPostModel.SerialNo = ProtoBuffBase.DeserializeObjects<AddPostModel>(PostDetailFilePath).Count + 1;
              AddPostViewModel.AddPostModel.SerialNo =PostFileManager.GetAllPost().Count + 1;
            AddPostViewModel.AddPostModel.Status = AddPostViewModel.AddPostModel.CampaignDetails.SelectedStatus;
          //  PostFileManager.SavePost(AddPostViewModel.AddPostModel);

            // ProtoBuffBase.SerializeObjects(AddPostViewModel.AddPostModel, PostDetailFilePath);
            //manageDraft.Visibility = Visibility.Visible;
            //Campaigns.Visibility = Visibility.Collapsed;
        }
        static AddPosts ObjAddPosts = null;
        public static AddPosts GetSingeltonAddPosts()
        {
            if (ObjAddPosts == null)
                ObjAddPosts = new AddPosts();
            return ObjAddPosts;
        }
        private AddPostViewModel _addPostViewModel;

        public event PropertyChangedEventHandler PropertyChanged;

        public AddPostViewModel AddPostViewModel
        {
            get
            {
                return _addPostViewModel;
            }
            set
            {
                _addPostViewModel = value;
                OnPropertyChanged(nameof(AddPostViewModel));
            }
        }
        /// <summary>
        /// OnPropertyChanged is used to notify that some property are changed 
        /// </summary>
        /// <param name="propertyName">property name</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void btnPhotos_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "Jpg (.jpg)|*.jpg|Png (*.png)|*.png|All files (*.*)|*.*"
            };
            var openFileDialogResult = openFileDialog.ShowDialog();
            if (openFileDialogResult != true)
                return;

            foreach (var fileName in openFileDialog.FileNames)
            {
                AddPostViewModel.AddPostModel.PhotoUrl.Add(fileName);
            }
        }

        private void btnVideo_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "Mp4 (.mp4)|*.mp4|Avi (*.avi)|*.avi|All files (*.*)|*.*"
            };
            var openFileDialogResult = openFileDialog.ShowDialog();
            if (openFileDialogResult != true)
                return;

            foreach (var fileName in openFileDialog.FileNames)
            {
                AddPostViewModel.AddPostModel.VideoUrl.Add(fileName);
            }
        }

        private void btnImportPostFromCsv_Click(object sender, RoutedEventArgs e)
        {
            FileUtilities.FileBrowseAndReader().ForEach(x =>
                 AddPostViewModel.AddPostModel.ImportedText = AddPostViewModel.AddPostModel.ImportedText + "\r\n" + x);
        }

        private void btnAddToPostList_Click(object sender, RoutedEventArgs e)
        {
            Campaigns ObjCampaigns = Campaigns.GetSingltonCreateCampaignObject();
            var portDetails = PostFileManager.GetAllPost();
            int PendingCount = 0;
            int DraftCount = 0;
            int PublishedCount = 0;
            if (!string.IsNullOrEmpty(ObjCampaigns.cmbCampaign.Text) && ObjCampaigns.cmbCampaign.Text == AddPostViewModel.AddPostModel.CampaignDetails.CampaignName)
            {
                var postToupdate= portDetails.FirstOrDefault(post => post.CampaignDetails.CampaignName == AddPostViewModel.AddPostModel.CampaignDetails.CampaignName);
                PendingCount = postToupdate.PostStatus.PendingCount;
                DraftCount = postToupdate.PostStatus.DraftCount;
                PublishedCount = postToupdate.PostStatus.PublishedCount;
            

                if (AddPostViewModel.AddPostModel.CampaignDetails.SelectedStatus == FindResource("langAll").ToString())
                {
                    AddPostViewModel.AddPostModel.PostStatus.PendingCount = PendingCount + 1;

                    AddPostViewModel.AddPostModel.PostStatus.DraftCount = DraftCount + 1;

                    AddPostViewModel.AddPostModel.PostStatus.PublishedCount = PublishedCount + 1;

                }
                else
                {
                    AddPostViewModel.AddPostModel.PostStatus.PendingCount = 
                        AddPostViewModel.AddPostModel.CampaignDetails.SelectedStatus == "Pending" ? PendingCount+1 : PendingCount;

                    AddPostViewModel.AddPostModel.PostStatus.DraftCount =
                        AddPostViewModel.AddPostModel.CampaignDetails.SelectedStatus == "Draft" ? DraftCount+ 1 : DraftCount;

                    AddPostViewModel.AddPostModel.PostStatus.PublishedCount =
                        AddPostViewModel.AddPostModel.CampaignDetails.SelectedStatus == "Published" ? PublishedCount +1 : PublishedCount;
                }
                AddPostViewModel.AddPostModel.CampaignDetails.SelectedAccount = ObjCampaigns.publishersHeader.cmbAccounts.SelectedValue.ToString();
                PostFileManager.EditPost(AddPostViewModel.AddPostModel);
            }
            else
            {
                AddPostViewModel.AddPostModel.CampaignDetails.SelectedAccount = ObjCampaigns.publishersHeader.cmbAccounts.SelectedValue.ToString();
                AddPostViewModel.AddPostModel.CampaignDetails.CampaignCreatedDate = DateTime.Now;
                AddPostViewModel.AddPostModel.SerialNo = portDetails.Count + 1;


                if (AddPostViewModel.AddPostModel.CampaignDetails.SelectedStatus == FindResource("langAll").ToString())
                {
                    AddPostViewModel.AddPostModel.PostStatus.PendingCount = 1;

                    AddPostViewModel.AddPostModel.PostStatus.DraftCount = 1;

                    AddPostViewModel.AddPostModel.PostStatus.PublishedCount = 1;

                }
                else
                {
                    AddPostViewModel.AddPostModel.PostStatus.PendingCount = AddPostViewModel.AddPostModel.CampaignDetails.SelectedStatus == "Pending" ?
                        1 : 0;
                    AddPostViewModel.AddPostModel.PostStatus.DraftCount = AddPostViewModel.AddPostModel.CampaignDetails.SelectedStatus == "Draft" ?
                        1 : 0;
                    AddPostViewModel.AddPostModel.PostStatus.PublishedCount = AddPostViewModel.AddPostModel.CampaignDetails.SelectedStatus == "Published" ?
                         1 : 0;
                }
                AddPostViewModel.AddPostModel.CampaignDetails.LstCampaign.Add(
                    new Campaign { CampaignName = AddPostViewModel.AddPostModel.CampaignDetails.CampaignName });
            
                PostFileManager.SavePost(AddPostViewModel.AddPostModel);
            }
            SetDataContext();
        }

        private void numericMaxPost_ValueDecremented(object sender, NumericUpDownChangedRoutedEventArgs args)
        {
            try
            {
                AddPostViewModel.AddPostModel.LstTimer.RemoveAt((int)numericMaxPost.Value - 1);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message + ex.StackTrace);
            }

        }

        private void numericMaxPost_ValueIncremented(object sender, NumericUpDownChangedRoutedEventArgs args)
        {
            try
            {
                AddPostViewModel.AddPostModel.LstTimer.Add(new lstTimeSpan() { TimeSpan = new TimeSpan(12, 0, 0) });
            }
            catch (Exception ex)
            {

                GlobusLogHelper.log.Error(ex.Message + ex.StackTrace);
            }
        }

        private void btnInstagramAdvancedOptions_Click(object sender, RoutedEventArgs e)
        {
            InstagramAdvancedOptions objInstagramAdvancedOptions = new InstagramAdvancedOptions();
            Dialog dialog = new Dialog();
            Window window = dialog.GetMetroWindow(objInstagramAdvancedOptions, "Instagram Advanced Options");
            window.Show();
        }

        private void btnAdvancedOptions_Click(object sender, RoutedEventArgs e)
        {
            AdvancedOption objAdvancedOption = new AdvancedOption();
            Dialog dialog = new Dialog();
            Window window = dialog.GetMetroWindow(objAdvancedOption, "Advanced Options");
            window.Show();
        }

        private void btnAdvancedSettings_Click(object sender, RoutedEventArgs e)
        {
            CampaignsAdvanceSetting ObjCampaignsAdvanceSetting = new CampaignsAdvanceSetting();
            Dialog dialog = new Dialog();
            Window window = dialog.GetMetroWindow(ObjCampaignsAdvanceSetting, "Campaign - Advanced Settings");
            window.Show();
        }

       
    }
}
