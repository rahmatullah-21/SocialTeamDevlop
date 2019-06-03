using DominatorHouseCore.StartupActivity;
using FaceDominatorCore.FDEnums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouse.Utilities.Facebook
{
    class FacebookCommentScraperActivity : BaseActivity
    {
        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(CommentScraperParameter)).ToList();
        }
    }
}
