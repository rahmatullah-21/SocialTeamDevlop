using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;

namespace DominatorHouseCore.DatabaseHandler.Utility
{
    public class DbOperations
    {
        private DbContext _context;

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

        public bool Add<T>(T data) where T : class
        {
            try
            {
                _context.Set<T>().Add(data);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region Read Operations

        public int Count<T>(Expression<Func<T, bool>> expression = null) where T : class
        {
            try
            {
                return expression == null ? _context.Set<T>().Count() : _context.Set<T>().Where(expression).Count();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public List<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class
        {
            try
            {
                return expression == null ? _context.Set<T>().ToList() : _context.Set<T>().Where(expression).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public T GetSingle<T>(Expression<Func<T, bool>> expression) where T : class
        {
            try
            {
                return _context.Set<T>().FirstOrDefault(expression);
            }
            catch (Exception)
            {
                return null;
            }
        }



        public async Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> expression = null) where T : class
        {
            var lstData = new List<T>();
            try
            {
                lstData = await Task.Factory.StartNew(() => 
                expression == null 
                ? _context.Set<T>().ToList() 
                : _context.Set<T>().Where(expression).ToList());
            }
            catch (Exception)
            {
                return lstData;
            }
            return lstData;
        }


        #endregion

        #region Update Operations

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



        public bool UpdateAccountDetails(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
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

        #endregion

    }
}