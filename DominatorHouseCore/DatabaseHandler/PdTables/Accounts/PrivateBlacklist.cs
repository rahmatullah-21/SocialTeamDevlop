using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.PdTables.Accounts
{
    public class PrivateBlacklist
    {
        [Key]
        [Column(Order = 1)]
        [Index]
        [Autoincrement]
        public int Id { get; set; }

        [Column(Order = 2)]
        public string UserName { get; set; }
        [Column(Order = 3)]
        public string UserId { get; set; }
        [Column(Order = 4)]
        public int InteractionTimeStamp { get; set; }
    }
}
