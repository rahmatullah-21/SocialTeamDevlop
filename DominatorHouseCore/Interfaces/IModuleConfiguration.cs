using System.Data.Entity;

namespace DominatorHouseCore.Interfaces
{
    public interface IModuleConfiguration
    {
        void Configuration(DbModelBuilder modelBuilder);      
    }
}