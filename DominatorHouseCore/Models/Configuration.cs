using ProtoBuf;
using System;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class Configuration
    {
        [ProtoMember(1)]
        public DateTime ConfigurationDate { get; set; }

        [ProtoMember(2)]
        public string ConfigurationType { get; set; }

        [ProtoMember(3)]
        public string ConfigurationSetting { get; set; }


    }
    [ProtoContract]
    public class Themes
    {
        [ProtoMember(1)]
        public Theme SelectedTheme { get; set; }

        [ProtoMember(2)]
        public AccentColors SelectedAccentColor { get; set; }

    }
    public class AccentColors
    {
        public string Name { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public AccentColors(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
    }

    public class Theme
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        public Theme(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }

    }
}
