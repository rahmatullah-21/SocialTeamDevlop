using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using SQLite;

namespace DominatorHouseCore.Interfaces
{
    public interface IDatabaseConnection
    {
        SQLiteConnection GetSqlConnection(string accountId);

    }

    public interface IGlobalDatabaseConnection
    {
        SQLiteConnection GetSqlConnection();
        SQLiteConnection GetSqlConnection(SocialNetworks networks, UserType userType);

    }
}