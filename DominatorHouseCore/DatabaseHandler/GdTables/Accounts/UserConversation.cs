using DominatorHouseCore.DatabaseHandler.Common;
using DominatorHouseCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.DatabaseHandler.GdTables.Accounts
{
    public class UserConversation : Entity
    {
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string SenderName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string SenderId
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string ThreadId
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string Date
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public ActivityType ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string ConversationType { get; set; }
    }
}
