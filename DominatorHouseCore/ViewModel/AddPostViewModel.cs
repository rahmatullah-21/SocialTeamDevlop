using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.ViewModel
{
    public class AddPostViewModel : BindableBase
    {
        public AddPostViewModel()
        {
            Enum.GetNames(typeof(DayOfWeek)).ToList().ForEach(day =>
            {
                AddPostModel.JobConfigurations.Weekday.Add(new ContentSelectGroup()
                {
                    Content = day
                });
            });
            AddPostModel.OtherConfiguration.MakeImagesUniqueStatus.Add("Medium");

        }
        private AddPostModel _addPostModel = new AddPostModel();

        public AddPostModel AddPostModel
        {
            get
            {
                return _addPostModel;
            }
            set
            {
                if (_addPostModel == null & _addPostModel == value)
                    return;
                SetProperty(ref _addPostModel, value);
            }
        }
        private ObservableCollection<AddPostModel> _lstAddPostModel;// = new ObservableCollection<AddPostModel>(GdBinFileHelper.GetBinFileDetails<AddPostModel>());

        public ObservableCollection<AddPostModel> LstAddPostModel
        {
            get
            {
                return _lstAddPostModel;
            }
            set
            {
                if (_lstAddPostModel == null & _lstAddPostModel == value)
                    return;
                SetProperty(ref _lstAddPostModel, value);
            }
        }
    }
}
