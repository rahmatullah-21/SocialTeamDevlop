using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DominatorHouseCore.DatabaseHandler.Utility
{
    public interface IDbOperations : IDisposable
    {
        bool Add<T>(T data) where T : class, new();
        bool AddRange<T>(List<T> data) where T : class, new();
        int Count<T>(Expression<Func<T, bool>> expression = null) where T : class, new();
        List<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new();
        T GetSingle<T>(Expression<Func<T, bool>> expression) where T : class, new();
        Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> expression = null) where T : class, new();
        bool Update<T>(T t) where T : class, new();
        bool UpdateAccountDetails(DominatorAccountModel dominatorAccountModel);
        bool Remove<T>(T t) where T : class;
        bool RemoveAll<T>() where T : class, new();
        void Remove<T>(Expression<Func<T, bool>> expression) where T : class, new();
        void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new();
        bool Any<T>(Expression<Func<T, bool>> expression) where T : class, new();
        bool UpdateRange<T>(List<T> data) where T : class, new();
        SocialNetworks SocialNetworks { get; }
        string AccountId { get; }
    }

    public class DbOperations : IDbOperations
    {
        private readonly SQLiteConnection _context;
        public SocialNetworks SocialNetworks { get; }
        public string AccountId { get; }

        public DbOperations(SQLiteConnection context)
        {
            SocialNetworks = SocialNetworks.Social;
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
            AccountId = id;
            SocialNetworks = networks;
            if (type == ConstantVariable.GetAccountDb)
            {
                var databaseConnection = ServiceLocator.Current.GetInstance<IAccountDatabaseConnection>(networks.ToString());
                _context = databaseConnection.GetSqlConnection(id);
            }
            if (type == ConstantVariable.GetCampaignDb)
            {
                var databaseConnection = ServiceLocator.Current.GetInstance<ICampaignDatabaseConnection>(networks.ToString());
                _context = databaseConnection.GetSqlConnection(id);
            }
        }


        #region Create operations

        /// <summary>
        /// To add the data to the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Add<T>(T data) where T : class, new()
        {
            return _context.Insert(data) > 0;
        }

        public bool AddRange<T>(List<T> data) where T : class, new()
        {
            return _context.InsertAll(data) > 0;
        }
        #endregion

        #region Read Operations

        /// <summary>
        /// To get the count of the matched expression count
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="expression">Filtering expression</param>
        /// <returns></returns>
        public int Count<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return expression == null ? _context.Table<T>().Count() : _context.Table<T>().Where(expression).Count();
        }


        /// <summary>
        /// To get the records which matches the expression
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="expression">Matching expression</param>
        /// <returns></returns>
        public List<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return expression == null ? _context.Table<T>().ToList() : _context.Table<T>().Where(expression).ToList();
        }

        /// <summary>
        /// To get the first record which matches the given expression
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="expression">Match Expression</param>
        /// <returns></returns>
        public T GetSingle<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return _context.Table<T>().Where(expression).FirstOrDefault();
        }


        /// <summary>
        /// To get the records which matches the expression in async mode
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="expression">Match Expression</param>
        /// <returns></returns>
        public async Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return expression == null
                ? _context.Table<T>().ToList()
                : _context.Table<T>().Where(expression).ToList();
        }


        #endregion

        #region Update Operations

        /// <summary>
        /// To update the record in the database
        /// </summary>
        /// <typeparam name="T">Targer type</typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Update<T>(T t) where T : class, new()
        {
            return _context.Update(t) > 0;
        }


        public bool UpdateAccountDetails(DominatorAccountModel dominatorAccountModel)
        {
            var dataToUpdate = _context.Table<AccountDetails>().FirstOrDefault(x => x.AccountId == dominatorAccountModel.AccountId);

            if (dataToUpdate == null)
                return false;

            dataToUpdate.AccountNetwork = dominatorAccountModel.AccountBaseModel.AccountNetwork.ToString();
            dataToUpdate.UserFullName = dominatorAccountModel.AccountBaseModel.UserFullName;
            dataToUpdate.Status = dominatorAccountModel.AccountBaseModel.Status.ToString();
            dataToUpdate.Cookies = JsonConvert.SerializeObject(dominatorAccountModel.Cookies);
            dataToUpdate.ProfilePictureUrl = dominatorAccountModel.AccountBaseModel.ProfilePictureUrl;
            return Update(dataToUpdate);
        }
        #endregion

        #region Delete Operations

        public bool Remove<T>(T t) where T : class
        {
            return _context.Delete<T>(t) > 0;
        }

        public bool RemoveAll<T>() where T : class, new()
        {
            return _context.DeleteAll<T>() > 0;
        }


        public void Remove<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            Remove(_context.Table<T>().Where(expression).FirstOrDefault());
        }


        public void RemoveMatch<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            var matchedItems = _context.Table<T>().Where(expression);
            foreach (var items in matchedItems)
                Remove(items);
        }


        public bool Any<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return _context.Table<T>().Where(expression).Any();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion
        public List<string> GetSingleColumn<T>(Func<T, string> query, Expression<Func<T, bool>> expression = null) where T : class, new()
        {
            return expression == null ? _context.Table<T>().Select(query).ToList() : _context.Table<T>().Where(expression).Select(query).ToList();
        }
        public bool UpdateRange<T>(List<T> data) where T : class, new()
        {
            return _context.UpdateAll(data) > 0;
        }
    }
}