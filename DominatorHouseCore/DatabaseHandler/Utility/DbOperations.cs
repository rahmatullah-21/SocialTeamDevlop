using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DominatorHouseCore.DatabaseHandler.Utility
{
    public class DbOperations
    {
        public int Count<T>(DbContext context, Expression<Func<T, bool>> expression = null) where T : class
        {
            try
            {
                return expression == null ? context.Set<T>().Count() : context.Set<T>().Where(expression).Count();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public bool Add<T>(DbContext context, T data) where T : class
        {
            try
            {
                context.Set<T>().Add(data);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<T> Get<T>(DbContext context, Expression<Func<T, bool>> expression = null) where T : class
        {
            try
            {
                return expression == null ? context.Set<T>().ToList() : context.Set<T>().Where(expression).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public T GetSingle<T>(DbContext context, Expression<Func<T, bool>> expression) where T : class
        {
            try
            {
                return context.Set<T>().FirstOrDefault(expression);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool Remove<T>(DbContext context, T t) where T : class
        {
            try
            {
                context.Entry<T>(t).State = EntityState.Deleted;
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update<T>(DbContext context, T t) where T : class
        {
            try
            {
                context.Entry<T>(t).State = EntityState.Modified;
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool RemoveAll<T>(DbContext context) where T : class
        {
            try
            {
                var dataList = context.Set<T>().ToList();
                context.Set<T>().RemoveRange(dataList);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}