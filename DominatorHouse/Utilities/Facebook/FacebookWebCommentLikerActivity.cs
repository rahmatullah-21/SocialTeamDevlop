using DominatorHouseCore.StartupActivity;
using FaceDominatorCore.FDEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouse.Utilities.Facebook
{
    class FacebookWebCommentLikerActivity : BaseActivity
    {
        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(WebCommentLikerParameter)).ToList();
        }
    }
}
