using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
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

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for MediaGenerator.xaml
    /// </summary>

    public partial class MediaGenerator : UserControl, INotifyPropertyChanged
    {
        public MediaGenerator()
        {
            InitializeComponent();

            DataContext = MediaGeneratorViewModel;

        }
        private static MediaGenerator instance = null;
        public static MediaGenerator GetMediaGeneratorInstance()
        {
            return instance ?? (instance = new MediaGenerator());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private MediaGeneratorViewModel _mediaGeneratorViewModel = new MediaGeneratorViewModel();

        public MediaGeneratorViewModel MediaGeneratorViewModel
        {
            get
            {
                return _mediaGeneratorViewModel;
            }
            set
            {
                if (_mediaGeneratorViewModel == value)
                    return;
                _mediaGeneratorViewModel = value;
                OnPropertyChanged(nameof(MediaGeneratorViewModel));
            }
        }

        private void CopyCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
           e.CanExecute= true;
        }

        private void CopyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ListView lb = (ListView)(sender);
                var filePath = (lb?.SelectedItem as string);
                if (!string.IsNullOrEmpty(filePath))
                {
                    Clipboard.SetText(filePath);
                    ToasterNotification.ShowSuccess("File Path copied");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
