using SQLite;

namespace DominatorHouseCore.DatabaseHandler.Common
{
    public abstract class Entity
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }
    }
}
