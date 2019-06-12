using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Enums.FdQuery
{
    public enum ReactionType
    {
        [Description("Like")]
        Like = 1,

        [Description("Love")]
        Love = 2,

        [Description("Haha")]
        Haha = 4,

        [Description("Wow")]
        Wow = 3,

        [Description("Sad")]
        Sad = 7,

        [Description("Angry")]
        Angry = 8,

        [Description("Unlike")]
        Unlike = 0,

        //        [Description(" ")]
        //        NotLiked = 0,
    }
}
