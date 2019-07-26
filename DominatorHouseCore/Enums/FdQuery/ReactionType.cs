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

    public enum BrowserReactionType
    {
        [Description("Like")]
        Like = 0,

        [Description("Love")]
        Love = 1,

        [Description("Haha")]
        Haha = 2,

        [Description("Wow")]
        Wow = 3,

        [Description("Sad")]
        Sad = 4,

        [Description("Angry")]
        Angry = 5,

        [Description("Share")]
        Share = 6,

        [Description("Comment")]
        Comment = 7

        //        [Description(" ")]
        //        NotLiked = 0,
    }
}
