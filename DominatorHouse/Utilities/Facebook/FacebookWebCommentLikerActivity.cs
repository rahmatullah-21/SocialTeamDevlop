using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.StartupActivity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Legion.Utilities.Facebook
{
    class FacebookWebCommentLikerActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(WebCommentLikerParameter);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(WebCommentLikerParameter)).ToList();
        }
    }
}
