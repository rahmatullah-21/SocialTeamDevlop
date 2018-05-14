using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DominatorHouseCore.DatabaseHandler.Utility
{
    public class DbOperations
    {

        private DbContext _context;

        public DbOperations(DbContext context)
        {
            _context = context;
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                return 0;
            }
        }

        public List<T> Get<T>( Expression<Func<T, bool>> expression = null) where T : class
        {
            try
            {
                return expression == null ? _context.Set<T>().ToList() : _context.Set<T>().Where(expression).ToList();
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        #region Update Operations

        public bool Update<T>(T t) where T : class
        {
            try
            {
                _context.Entry<T>(t).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #region Delete Operations

        public bool Remove<T>( T t) where T : class
        {
            try
            {
                _context.Entry<T>(t).State = EntityState.Deleted;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
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
                return false;
            }
        }

        #endregion

    }
}