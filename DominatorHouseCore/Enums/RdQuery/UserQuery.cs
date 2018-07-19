using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Enums.RdQuery
{
    public enum UserQuery
    {
        [Description("LangKeyKeywords")]
        Keywords = 1,
        [Description("LangKeyCustomUsersList")]
        CustomUsers = 2,
        [Description("LangKeyUsersWhoCommentedOnPosts")]
        UsersWhoCommentedOnPost = 3,
    }
}
