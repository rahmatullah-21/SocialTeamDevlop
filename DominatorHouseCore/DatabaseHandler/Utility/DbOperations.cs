using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DominatorHouseCore.DatabaseHandler.Utility
{
    public class DbOperations
    {
        public DbContext _context;
        public DbContext Context => _context;

        public DbOperations(DbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// To Get the database operation with auto generated dbcontext for your account or campaigns
        /// </summary>
        /// <param name="id">If you need to perform with account, then pass id as account id where as in campaign case pass campaign id</param>
        /// <param name="networks">Accounts or campaign which belongs to which social network</param>
        /// <param name="type">Specify whether you account id or campaign id in <see cref="DominatorHouseCore.Utility.ConstantVariable.GetAccountDb"/> or <see cref="DominatorHouseCore.Utility.ConstantVariable.GetCampaignDb"/> </param>
        public DbOperations(string id, SocialNetworks networks, string type)
        {
            if (type == ConstantVariable.GetAccountDb)
            {
                var databaseConnection = SocinatorInitialize.GetSocialLibrary(networks).GetNetworkCoreFactory().AccountDatabase;
                _context = databaseConnection.GetContext(id);
            }
            if (type == ConstantVariable.GetCampaignDb)
            {
                var databaseConnection = SocinatorInitialize.GetSocialLibrary(networks).GetNetworkCoreFactory().CampaignDatabase;
                _context = databaseConnection.GetContext(id);
            }
        }

        #region Create operations

        /// <summary>
        /// To add the data to the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Add<T>(T data) where T : class
        {
            try
            {
                // Add to context
                _context.Set<T>().Add(data);
                // save the changes
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddRange<T>(List<T> data) where T : class
        {
            try
            {
                // Add to context
                _context.Set<T>().AddRange(data);
                // save the changes
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Read Operations

        /// <summary>
        /// To get the count of the matched expression count
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="expression">Filtering expression</param>
        /// <returns></returns>
        public int Count<T>(Expression<Func<T, bool>> expression = null) where T : class
        {
            try
            {
                // return the matched counts of the given expression
                return expression == null ? _context.Set<T>().Count() : _context.Set<T>().Where(expression).Count();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// To get the records which matches the expression
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="expression">Matching expression</param>
        /// <returns></returns>
        public List<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class
        {
            try
            {
                // Get the matched expression records, If expression is null returns full details
                return expression == null ? _context.Set<T>().ToList() : _context.Set<T>().Where(expression).ToList();
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        /// <summary>
        /// To get the first record which matches the given expression
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="expression">Match Expression</param>
        /// <returns></returns>
        public T GetSingle<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            try
            {
                // Get the first item with matched expression
                return _context.Set<T>().FirstOrDefault(expression);
            }
            catch (Exception)
            {
                return new T();
            }
        }


        /// <summary>
        /// To get the records which matches the expression in async mode
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="expression">Match Expression</param>
        /// <returns></returns>
        public async Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> expression = null) where T : class
        {
            var lstData = new List<T>();
            try
            {
                // Make with thread factory, If the expression is null then return the whole records otherwise only matched expression
                lstData = await (expression == null
                ? _context.Set<T>().ToListAsync()
                : _context.Set<T>().Where(expression).ToListAsync());
            }
            catch (Exception)
            {
                return lstData;
            }
            return lstData;
        }


        #endregion

        #region Update Operations

        /// <summary>
        /// To update the record in the database
        /// </summary>
        /// <typeparam name="T">Targer type</typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Update<T>(T t) where T : class
        {
            try
            {
                _context.Entry(t).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private static volatile int i;
        public bool UpdateAccountDetails(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                i++;
                var dataToUpdate = _context.Set<AccountDetails>().FirstOrDefault(x => x.AccountId == dominatorAccountModel.AccountId);

                if (dataToUpdate == null)
                    return false;

                dataToUpdate.AccountNetwork = dominatorAccountModel.AccountBaseModel.AccountNetwork.ToString();
                dataToUpdate.UserFullName = dominatorAccountModel.AccountBaseModel.UserFullName;
                dataToUpdate.Status = dominatorAccountModel.AccountBaseModel.Status.ToString();
                dataToUpdate.Cookies = JsonConvert.SerializeObject(dominatorAccountModel.Cookies);
                dataToUpdate.ProfilePictureUrl = dominatorAccountModel.AccountBaseModel.ProfilePictureUrl;
                _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }
        #endregion

        #region Delete Operations

        public bool Remove<T>(T t) where T : class
        {
            try
            {
                _context.Entry(t).State = EntityState.Deleted;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }
        public bool RemoveAll<T>() where T : class
        {
            try
            {
                var dataList = _context.Set<T>().ToList();
                _context.Set<T>().RemoveRange(dataList);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }


        public void Remove<T>(Expression<Func<T, bool>> expression) where T : class
        {
            try
            {
                Remove(_context.Set<T>().FirstOrDefault(expression));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class
        {
            try
            {
                var matchedItems = _context.Set<T>().Where(expression);
                foreach (var items in matchedItems)
                    Remove(items);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public bool Any<T>(Expression<Func<T, bool>> expression) where T : class
        {
            try
            {
                return _context.Set<T>().Any(expression);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                throw;
            }
        }

        #endregion


    }
}