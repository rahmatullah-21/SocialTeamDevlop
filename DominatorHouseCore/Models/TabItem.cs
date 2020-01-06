using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace DominatorHouseCore.Models
{
    public class TabItemTemplates
    {
        public string Title { get; set; }

        public Lazy<UserControl> Content { get; set; }

        public Visual ImagePath { get; set; }
    }
}