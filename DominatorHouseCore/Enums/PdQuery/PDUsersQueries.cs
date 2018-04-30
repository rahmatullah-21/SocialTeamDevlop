using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Enums.PdQuery
{
    public enum PDUsersQueries
    {
        [Description("PDlangKeywords")]
        Keywords = 1,
        [Description("PDlangSomeonesFollowers")]
        SomeonesFollowers = 2,
        [Description("PDlangSomeonesFollowings")]
        SomeonesFollowings = 3,
        [Description("PDlangFollowersOfSomeonesFollowings")]
        FollowersOfSomeonesFollowings = 4,
        [Description("PDlangFollowersOfSomeonesFollowers")]
        FollowersOfSomeonesFollowers = 5,
        [Description("PDlangCustomUser")]
        Customusers = 6,
        [Description("PDlangBoardFollowers")]
        BoardFollowers = 7,
        [Description("PDlangUsersWhoTried")]
        UserswhoTriedposts = 8,
        [Description("PDlangCustomBoard")]
        CustomBoard = 9
    }
}
