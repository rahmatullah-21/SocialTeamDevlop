using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.CoreModels
{
    public class SQliteMigration<TDbContext> : SqliteCreateDatabaseIfNotExists<TDbContext> where TDbContext : DbContext
    {
        public SQliteMigration(DbModelBuilder modelBuilder)
            : base(modelBuilder, true)
        {

        }

        public override void InitializeDatabase(TDbContext context)
        {
            string databseFilePath = GetDatabasePathFromContext(context);
            bool dbExists = File.Exists(databseFilePath);
            if (dbExists)
            {
                var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;
                var tables = metadata.GetItemCollection(DataSpace.SSpace)
                   .GetItems<EntityContainer>()
                   .Single()
                   .BaseEntitySets
                   .OfType<EntitySet>()
                   .Where(s => !s.MetadataProperties.Contains("Type")
                               || s.MetadataProperties["Type"].ToString() == "Tables");
                foreach (var table in tables)
                {
                    var tableName = table.MetadataProperties.Contains("Table")
                                    && table.MetadataProperties["Table"].Value != null
                        ? table.MetadataProperties["Table"].Value.ToString()
                        : table.Name;
                    foreach (var member in table.ElementType.DeclaredMembers)
                    {
                        var cur = context.Database.SqlQuery<TableInfo>($"PRAGMA table_info ({tableName})").ToList();

                        if (cur.All(a => a.name != member.Name))
                        {
                            context.Database.ExecuteSqlCommand(
                                $"ALTER TABLE {tableName} ADD COLUMN {member.Name} {((TypeUsage)((ReadOnlyMetadataCollection<System.Data.Entity.Core.Metadata.Edm.MetadataProperty>)member.MetadataProperties["MetadataProperties"].Value)["TypeUsage"].Value).EdmType.Name} null;");
                        }
                    }
                }
            }
            base.InitializeDatabase(context);
        }
        private class TableInfo
        {
            public long cid { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public long notnull { get; set; }
            public string dflt_value { get; set; }
            public long pk { get; set; }
        }
    }
}
