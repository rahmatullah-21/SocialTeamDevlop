using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using Newtonsoft.Json;

namespace DominatorHouseCore.DatabaseHandler.CoreModels
{
    public class DataBaseConnectionGlobal
    {
        public DataBaseConnectionGlobal(string connectionString, Action<DbModelBuilder> ConfigureDbModelBuilder = null)
        {
            ConnectionString = connectionString;
            this.ConfigureDbModelBuilder = ConfigureDbModelBuilder;

        }




        private string ConnectionString { get; } = string.Empty;

        private Action<DbModelBuilder> ConfigureDbModelBuilder { get; }


        public int Count<T>(Expression<Func<T, bool>> expression = null) where T : class
        {
            try
            {
                using (var sqLiteConnection = new SQLiteConnection(@"data source=" + ConnectionString))
                {
                    sqLiteConnection.Open();
                    using (var context = new GlobalDbContext(sqLiteConnection, false, ConfigureDbModelBuilder))
                    {
                        return expression == null
                            ? context.Set<T>().Count()
                            : context.Set<T>().Where(expression).Count();
                    }
                }
            }
            catch (Exception ex)
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
                    using (var context = new GlobalDbContext(sqLiteConnection, false, ConfigureDbModelBuilder))
                    {
                        context.Set<T>().Add(data);
                        context.SaveChanges();
                    }
                }

                return true;
            }
            catch (Exception ex)
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
                    using (var context = new GlobalDbContext(sqLiteConnection, false, ConfigureDbModelBuilder))
                    {
                        return Expression == null
                            ? context.Set<T>().ToList()
                            : context.Set<T>().Where(Expression).ToList();
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
            var lstData = new List<T>();
            try
            {
                lstData = await Task.Factory.StartNew(() =>
                {
                    using (var sqLiteConnection = new SQLiteConnection(@"data source=" + ConnectionString))
                    {
                        sqLiteConnection.Open();
                        using (var context = new GlobalDbContext(sqLiteConnection, false, ConfigureDbModelBuilder))
                        {
                            return Expression == null
                                ? context.Set<T>().ToList()
                                : context.Set<T>().Where(Expression).ToList();
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
                    using (var context = new GlobalDbContext(sqLiteConnection, false, ConfigureDbModelBuilder))
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
                    using (var context = new GlobalDbContext(sqLiteConnection, false, ConfigureDbModelBuilder))
                    {
                        context.Entry(t).State = EntityState.Deleted;
                        context.SaveChanges();
                    }

                    sqLiteConnection.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void Remove<T>(Expression<Func<T, bool>> Expression) where T : class
        {
            try
            {
                using (var sqLiteConnection = new SQLiteConnection(@"data source=" + ConnectionString))
                {
                    sqLiteConnection.Open();
                    using (var context = new GlobalDbContext(sqLiteConnection, false, ConfigureDbModelBuilder))
                    {
                        Remove(context.Set<T>().FirstOrDefault(Expression));
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public bool Update<T>(T t) where T : class
        {
            try
            {
                using (var sqLiteConnection = new SQLiteConnection(@"data source=" + ConnectionString))
                {
                    sqLiteConnection.Open();
                    using (var context = new GlobalDbContext(sqLiteConnection, false, ConfigureDbModelBuilder))
                    {
                        context.Entry(t).State = EntityState.Modified;
                        context.SaveChangesAsync();
                    }

                    sqLiteConnection.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool UpdateAccountDetails(DominatorAccountModel dominatorAccountModel) 
        {
            try
            {
                using (var sqLiteConnection = new SQLiteConnection(@"data source=" + ConnectionString))
                {
                    sqLiteConnection.Open();
                    using (var context = new GlobalDbContext(sqLiteConnection, false, ConfigureDbModelBuilder))
                    {
                        var dataToUpdate = context.Set<AccountDetails>().FirstOrDefault(x => x.AccountId == dominatorAccountModel.AccountId);
                        dataToUpdate.AccountNetwork = dominatorAccountModel.AccountBaseModel.AccountNetwork.ToString();
                        dataToUpdate.UserFullName = dominatorAccountModel.AccountBaseModel.UserFullName;
                        dataToUpdate.Status = dominatorAccountModel.AccountBaseModel.Status;
                        dataToUpdate.Cookies = JsonConvert.SerializeObject(dominatorAccountModel.Cookies);
                        dataToUpdate.ProfilePictureUrl = dominatorAccountModel.AccountBaseModel.ProfilePictureUrl;
                        context.SaveChangesAsync();
                    }

                    sqLiteConnection.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}