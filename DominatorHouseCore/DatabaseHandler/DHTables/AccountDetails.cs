using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.DHTables
{
    public class AccountDetails
    {
        [Key]
        [Column(Order = 1)]
        [Index]
        [Autoincrement]
        public int Id { get; set; }
        [Column(Order = 2)]
        public string AccountNetwork { get; set; }
        [Column(Order = 3)]
        public string AccountId { get; set; }
        [Column(Order = 4)]
        public string AccountGroup { get; set; }
        [Column(Order = 5)]
        public string UserName { get; set; }
        [Column(Order = 6)]
        public string Password { get; set; }
        [Column(Order = 7)]
        public string UserFullName { get; set; }
        [Column(Order = 8)]
        public string Status { get; set; }
        [Column(Order = 9)]
        public string ProxyIP { get; set; }
        [Column(Order = 10)]
        public string ProxyPort { get; set; }
        [Column(Order = 11)]
        public string ProxyUserName { get; set; }
        [Column(Order = 12)]
        public string ProxyPassword { get; set; }
        [Column(Order = 13)]
        public string ProfilePictureUrl { get; set; }
        [Column(Order = 14)]
        public string Cookies { get; set; }
    }
}
