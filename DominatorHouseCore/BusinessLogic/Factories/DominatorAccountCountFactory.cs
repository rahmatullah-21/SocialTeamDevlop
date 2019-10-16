using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.BusinessLogic.Factories
{
    public class DominatorAccountCountFactory : IAccountCountFactory
    {
        private static DominatorAccountCountFactory _instance;

        public static DominatorAccountCountFactory Instance
            => _instance ?? (_instance = new DominatorAccountCountFactory());

        private DominatorAccountCountFactory() { }

        public string HeaderColumn1Value { get; set; } = "LangKeyFollowersCount".FromResourceDictionary();
        public bool HeaderColumn1Visiblity { get; set; } = true;
        public string HeaderColumn2Value { get; set; } = "LangKeyFollowingsCount".FromResourceDictionary();
        public bool HeaderColumn2Visiblity { get; set; } = true;
        public string HeaderColumn3Value { get; set; } = "LangKeyTweetCount".FromResourceDictionary();
        public bool HeaderColumn3Visiblity { get; set; } = true;
        public string HeaderColumn4Value { get; set; } = string.Empty;
        public bool HeaderColumn4Visiblity { get; set; } = false;
    }
}