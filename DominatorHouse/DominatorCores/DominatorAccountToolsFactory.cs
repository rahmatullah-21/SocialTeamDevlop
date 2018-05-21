
﻿using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using Socinator.Social.AutoActivity.Views;

namespace Socinator.DominatorCores
{
    public class DominatorAccountToolsFactory : IAccountToolsFactory
    {
        private static DominatorAccountToolsFactory _instance;

        public static DominatorAccountToolsFactory Instance
            => _instance ?? (_instance = new DominatorAccountToolsFactory());

        private DominatorAccountToolsFactory()  { }

        public UserControl GetStartupToolsView() 
            => SocialAutoActivity.GetSingletonSocialAutoActivity();

        public IEnumerable<ActivityType> GetImportantActivityTypes()
        {
            return new List<ActivityType>();
        }
    }
}