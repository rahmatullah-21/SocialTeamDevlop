using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.DatabaseHandler.CoreModels
{
    public class DataBaseConnectionCampaign
    {
        private string ConnectionString { get; set; } = string.Empty;

        private Action<DbModelBuilder, SocialNetworks> ConfigureDbModelBuilder { get; set; }

        private SocialNetworks Network { get; set; }

        public DataBaseConnectionCampaign(string connectionString, SocialNetworks networks, Action<DbModelBuilder, SocialNetworks> ConfigureDbModelBuilder = null)
        {
            this.ConnectionString = connectionString;
            this.ConfigureDbModelBuilder = ConfigureDbModelBuilder;
            this.Network = networks;
        }

        public int Count<T>(Expression<Func<T, bool>> expression = null) where T : class
        {
            try
            {
                using (var sqLiteConnection = new SQLiteConnection(@"data source=" + ConnectionString))
                {
                    sqLiteConnection.Open();
                    using (var context = new CampaignDbContext(sqLiteConnection, false, Network, this.ConfigureDbModelBuilder))
                    {
                        return expression == null ? context.Set<T>().Count() : context.Set<T>().Where(expression).Count();
                    }
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }


        public bool Add<T>(T data) where T : class
        {
            try
            {
                using (var sqLiteConnection = new SQLiteConnection(@"data source=" + ConnectionString))
                {
                    sqLiteConnection.Open();
                    using (var context = new CampaignDbContext(sqLiteConnection, false, Network, this.ConfigureDbModelBuilder))
                    {
                        context.Set<T>().Add(data);
                        context.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<T> Get<T>(Expression<Func<T, bool>> Expression = null) where T : class
        {
            try
            {
                using (var sqLiteConnection = new SQLiteConnection(@"data source=" + ConnectionString))
                {
                    sqLiteConnection.Open();
                    using (var context = new CampaignDbContext(sqLiteConnection, false, Network, this.ConfigureDbModelBuilder))
                    {
                        return Expression == null ? context.Set<T>().ToList() : context.Set<T>().Where(Expression).ToList();
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> Expression = null) where T : class
        {
            List<T> lstData = new List<T>();
            try
            {
                lstData = await Task.Factory.StartNew(() =>
                {
                    using (var sqLiteConnection = new SQLiteConnection(@"data source=" + ConnectionString))
                    {
                        sqLiteConnection.Open();
                        using (var context = new CampaignDbContext(sqLiteConnection, false, Network, this.ConfigureDbModelBuilder))
                        {
                            return Expression == null ? context.Set<T>().ToList() : context.Set<T>().Where(Expression).ToList();
                        }
                    }
                });

            }
            catch (Exception)
            {
                return lstData;
            }
            return lstData;
        }


        public T GetSingle<T>(Expression<Func<T, bool>> Expression) where T : class
        {
            try
            {
                using (var sqLiteConnection = new SQLiteConnection(@"data source=" + ConnectionString))
                {
                    sqLiteConnection.Open();
                    using (var context = new CampaignDbContext(sqLiteConnection, false, Network, this.ConfigureDbModelBuilder))
                    {
                        return context.Set<T>().FirstOrDefault(Expression);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool Remove<T>(T t) where T : class
        {
            try
            {
                using (var sqLiteConnection = new SQLiteConnection(@"data source=" + ConnectionString))
                {
                    sqLiteConnection.Open();
                    using (var context = new CampaignDbContext(sqLiteConnection, false, Network, this.ConfigureDbModelBuilder))
                    {
                        context.Entry<T>(t).State = System.Data.Entity.EntityState.Deleted;
                        context.SaveChanges();
                    }
                    sqLiteConnection.Close();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Update<T>(T t) where T : class
        {
            try
            {
                using (var sqLiteConnection = new SQLiteConnection(@"data source=" + ConnectionString))
                {
                    sqLiteConnection.Open();
                    using (var context = new CampaignDbContext(sqLiteConnection, false, Network, this.ConfigureDbModelBuilder))
                    {
                        context.Entry<T>(t).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    sqLiteConnection.Close();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}