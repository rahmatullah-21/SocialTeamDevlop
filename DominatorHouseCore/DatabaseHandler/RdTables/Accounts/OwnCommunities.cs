using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.RdTables.Accounts
{
    public class OwnCommunities
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        public int Id { get; set; }
        [Column(Order = 2)]
        public string WhitelistStatus { get; set; }
        [Column(Order = 3)]
        public bool IsNsfw { get; set; }
        [Column(Order = 4)]
        public int Subscribers { get; set; }
        [Column(Order = 5)]
        public string PrimaryColor { get; set; }
        [Column(Order = 6)]
        [Unique]
        public string CommunityId { get; set; }
        [Column(Order = 7)]
        public bool IsQuarantined { get; set; }
        [Column(Order = 8)]
        public string Name { get; set; }
        [Column(Order = 9)]
        public string Title { get; set; }
        [Column(Order = 10)]
        public string Url { get; set; }
        [Column(Order = 11)]
        public string DisplayText { get; set; }
        [Column(Order = 12)]
        public string Type { get; set; }
        [Column(Order = 13)]
        public string CommunityIcon { get; set; }
        [Column(Order = 14)]
        public bool IsOwn { get; set; }
    }
}