using System.Collections.ObjectModel;
using System.Linq;
using DominatorHouseCore.Utility;

namespace DominatorHouse.Social.Settings.ViewModel
{
    public class AppearanceViewModel : BindableBase
    {   
        public AppearanceViewModel()
        {
            _lstColorsCollection = new ObservableCollection<ColorsCollection>()
            {
              new  ColorsCollection("Default","#003569"),
              new  ColorsCollection("red","#e51400"),
              new  ColorsCollection("green","#60a917"),
              new  ColorsCollection("blue","#2196F3"),
              new  ColorsCollection("purple","#800080"),
              new  ColorsCollection("orange","#fa6800"),
              new  ColorsCollection("lime","#a4c400"),
              new  ColorsCollection("teal","#00aba9"),
              new  ColorsCollection("cyan","#00BCD4"),
              new  ColorsCollection("indigo","#6a00ff"),
              new  ColorsCollection("violet","#aa00ff"),
              new  ColorsCollection("pink","#f472d0"),
              new  ColorsCollection("magenta","#d80073"),
              new  ColorsCollection("crimson","#a20025"),
              new  ColorsCollection("yellow","#e3c800"),
              new  ColorsCollection("brown","#825a2c"),
              new  ColorsCollection("olive","#6d8764"),
              new  ColorsCollection("sienna","#a0522d"),
              new  ColorsCollection("emerald","#008a00"),
              new  ColorsCollection("cobalt","#0050ef"),
              new  ColorsCollection("amber","#f0a30a"),
              new  ColorsCollection("steel"," #647687"),
              new  ColorsCollection("mauve","#76608a"),
              new  ColorsCollection("taupe","#87794E"),

            };

            _lstThemeCollection.Add(new ThemeCollection("Light", "White"));
            _lstThemeCollection.Add(new ThemeCollection("Dark", "Black"));

            ApplyDefaultThemeAndColor();

        }

        private void ApplyDefaultThemeAndColor()
        {
            this.selectedTheme = this.lstThemeCollection.FirstOrDefault();
            this.selectedAccentColor = this.lstColorsCollection.FirstOrDefault();
        }


        #region Properties


        private ObservableCollection<ThemeCollection> _lstThemeCollection = new ObservableCollection<ThemeCollection>();
        public ObservableCollection<ThemeCollection> lstThemeCollection
        {
            get
            {
                return _lstThemeCollection;
            }
            set
            {
                if (_lstThemeCollection != null && value == _lstThemeCollection)
                    return;
                SetProperty(ref _lstThemeCollection, value);
            }
        }

        private ObservableCollection<ColorsCollection> _lstColorsCollection;
        public ObservableCollection<ColorsCollection> lstColorsCollection
        {
            get
            {
                return _lstColorsCollection;
            }
            set
            {
                if (_lstColorsCollection != null && value == _lstColorsCollection)
                    return;
                SetProperty(ref _lstColorsCollection, value);
            }

        }

        private ObservableCollection<ColorsCollection> _lstRecentColorsCollection = new ObservableCollection<ColorsCollection>();
        public ObservableCollection<ColorsCollection> lstRecentColorsCollection
        {
            get
            {
                return _lstRecentColorsCollection;
            }
            set
            {
                if (_lstRecentColorsCollection != null && value == _lstRecentColorsCollection)
                    return;
                SetProperty(ref _lstRecentColorsCollection, value);
            }
        }

        private ThemeCollection selectedTheme;
        public ThemeCollection SelectedTheme
        {
            get
            {
                return this.selectedTheme;
            }
            set
            {
                if (this.selectedTheme != null && value == this.selectedTheme)
                    return;
                SetProperty(ref this.selectedTheme, value);
            }

        }

        private ColorsCollection selectedAccentColor;
        public ColorsCollection SelectedAccentColor
        {

            get
            {
                return this.selectedAccentColor;
            }
            set
            {
                if (this.selectedAccentColor != null && value == this.selectedAccentColor)
                    return;
                SetProperty(ref this.selectedAccentColor, value);
            }

        }

        private ColorsCollection selectedRecentColor;
        public ColorsCollection SelectedRecentColor
        {

            get
            {
                return this.selectedRecentColor;
            }
            set
            {
                if (this.selectedRecentColor != null && value == this.selectedRecentColor)
                    return;
                SetProperty(ref this.selectedRecentColor, value);
            }

        }


        #endregion
    }

    public class ColorsCollection
    {
        public string Name { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public ColorsCollection(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
    }

    public class ThemeCollection
    {
        string _name = string.Empty;
        string _value = string.Empty;

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
            }
        }
        public ThemeCollection(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }

    }
}
