#region

using SQLite;

#endregion

namespace DominatorHouseCore.DatabaseHandler.PdTables.Accounts
{
    public class OwnBoards
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        // Row ID
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public int BoardFollowers { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public int PinsCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        //[Unique]
        public string BoardUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string BoardName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string BoardDescription { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string Username { get; set; }
    }
}