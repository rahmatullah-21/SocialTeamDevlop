using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.ViewModel
{
    public class EditProfileViewModel
    {
        public ICommand UploadPhotoCommand { get; set; }

        public EditProfileViewModel()
        {
            UploadPhotoCommand = new BaseCommand<object>(UploadPhotoCanExecute, UploadPhotoExecute);
        }

        public EditProfileModel EditProfileModel { get; set; } = new EditProfileModel();


        private bool UploadPhotoCanExecute(object sender) => true;

        private void UploadPhotoExecute(object sender)
        {
            string filters = "Image Files | *.jpg; *.jpeg; *.png; *.gif";
            var picPath = FileUtilities.GetImageOrVideo(false, filters);
            if (picPath != null)
                EditProfileModel.ProfilePicPath = picPath;
        }
    }
}