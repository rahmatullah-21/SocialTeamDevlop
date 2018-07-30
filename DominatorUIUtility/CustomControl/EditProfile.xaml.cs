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
using DominatorHouseCore.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for EditProfile.xaml
    /// </summary>
    public partial class EditProfile : UserControl,INotifyPropertyChanged
    {
        public EditProfile()
        {
            InitializeComponent();
        }
        public EditProfile(EditProfileModel editProfileModel) :this()
        {
            EditProfileViewModel.EditProfileModel = editProfileModel;
            this.DataContext = EditProfileViewModel;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSubmit.IsDefault = true;
            }
        }
        private EditProfileViewModel _editProfileViewModel=new EditProfileViewModel();

        public EditProfileViewModel EditProfileViewModel
        {
            get { return _editProfileViewModel; }
            set
            {
                _editProfileViewModel = value;
                OnPropertyChanged(nameof(EditProfileViewModel));
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
