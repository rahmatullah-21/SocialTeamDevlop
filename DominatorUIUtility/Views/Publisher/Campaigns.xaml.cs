using System;
using System.Collections.Generic;
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
using DominatorHouseCore.Models;

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    /// Interaction logic for Campaigns.xaml
    /// </summary>
    public partial class Campaigns : UserControl
    {
        public Campaigns()
        {
            InitializeComponent();
            publishersHeader.HeaderText = FindResource("langCreateCampaign").ToString();

            #region Initialize Tabs
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title=FindResource("langAddPosts").ToString(),
                    Content=new Lazy<UserControl>(()=>AddPosts.GetSingeltonAddPosts())
                },
                new TabItemTemplates
                {
                    Title=FindResource("langClickableImagePost").ToString(),
                    //   Content=new Lazy<UserControl>(()=>new AddPosts())
                },
                new TabItemTemplates
                {
                    Title=FindResource("langSharePost").ToString(),
                    // Content=new Lazy<UserControl>(()=>new AddPosts())
                }
            };
            #endregion

            CreateCampaignTabs.ItemsSource = TabItems;
            ObjCreateCampaign = this;
            //  SetDataContext();
        }

     
        static Campaigns ObjCreateCampaign = null;
        public static Campaigns GetSingltonCreateCampaignObject()
        {
            return ObjCreateCampaign;
        }

        public void SetDataContext()
        {
            var SelectedTab = (CreateCampaignTabs.Items.CurrentItem as TabItemTemplates).Title.ToString();
            switch (SelectedTab)
            {
                case "Add Posts":
                    AddPosts ObjAddPosts = AddPosts.GetSingeltonAddPosts();

                    createCampign.DataContext = ObjAddPosts.AddPostViewModel.AddPostModel.CampaignDetails;
                    break;
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddPosts ObjAddPosts = AddPosts.GetSingeltonAddPosts();
            ObjAddPosts.manageDraft.Visibility = Visibility.Collapsed;
            ObjAddPosts.createCampaign.Visibility = Visibility.Visible;
        }


        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// OnPropertyChanged is used to notify that some property are changed 
        /// </summary>
        /// <param name="propertyName">property name</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


}
