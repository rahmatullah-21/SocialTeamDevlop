using SQLite.CodeFirst;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DominatorHouseCore.DatabaseHandler.PdTables.Accounts
{
    public class OwnBoards
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        // Row ID
        public int Id { get; set; }

        [Column(Order = 2)]
        public int BoardFollowers { get; set; }

        [Column(Order = 3)]
        public int PinsCount { get; set; }

        [Column(Order = 4)]
        [Unique]
        public string BoardUrl { get; set; }

        [Column(Order = 5)]
        public string BoardName { get; set; }

        [Column(Order = 6)]
        public string BoardDescription { get; set; }

        [Column(Order = 7)]
        public string Username { get; set; }
    }
}
