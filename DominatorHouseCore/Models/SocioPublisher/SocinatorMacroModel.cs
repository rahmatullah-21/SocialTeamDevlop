using System.ComponentModel;
using System.Runtime.CompilerServices;
using DominatorHouseCore.Annotations;

namespace DominatorHouseCore.Models.SocioPublisher
{
    public class SocinatorMacroModel : INotifyPropertyChanged
    {
        private string _macroKey = string.Empty;

        public string MacroKey
        {
            get
            {
                return _macroKey;
            }
            set
            {
                if (_macroKey == value)
                    return;
                _macroKey = value;
                OnPropertyChanged(nameof(MacroKey));
            }
        }

        private string _macroValue;

        public string MacroValue
        {
            get
            {
                return _macroValue;
            }
            set
            {
                if (_macroValue == value)
                    return;
                _macroValue = value;
                OnPropertyChanged(nameof(MacroValue));
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