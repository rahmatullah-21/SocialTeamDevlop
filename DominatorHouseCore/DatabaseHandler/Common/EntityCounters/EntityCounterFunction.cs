using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Utility;
using System;
using System.Linq.Expressions;

namespace DominatorHouseCore.DatabaseHandler.Common.EntityCounters
{
    public class EntityCounterFunction<T> where T : class, new()
    {
        private readonly IFilterPredicate<T> _datePredicate;
        private readonly IFilterPredicate<T> _activityTypePredicate;


        public EntityCounterFunction(IFilterPredicate<T> datePredicate)
        {
            _datePredicate = datePredicate;
        }
        public EntityCounterFunction(IFilterPredicate<T> datePredicate, IFilterPredicate<T> activityTypePredicate) :
            this(datePredicate)
        {
            _activityTypePredicate = activityTypePredicate;
        }

        public EntityCounter GetCounter(string accountId, SocialNetworks networks, ActivityType? activityType)
        {
            var getStartDateofWeek = DateTime.Now.GetStartOfWeek();
            var getTodayDate = DateTime.Today;
            var currentTimeStamp = DateTime.UtcNow.AddSeconds(-3600);
            var dbOperations = ServiceLocator.Current.ResolveAccountDbOperations(accountId, networks);
            var countLastWeek = dbOperations.Count(BuildExpressionFor(_datePredicate, _activityTypePredicate, getStartDateofWeek, activityType));
            var countLastDay = dbOperations.Count(BuildExpressionFor(_datePredicate, _activityTypePredicate, getTodayDate, activityType));
            var countLastHour = dbOperations.Count(BuildExpressionFor(_datePredicate, _activityTypePredicate, currentTimeStamp, activityType));

            return new EntityCounter(countLastWeek, countLastDay, countLastHour);
        }

        private Expression<Func<T, bool>> BuildExpressionFor(IFilterPredicate<T> dateFilter,
            IFilterPredicate<T> activityFilter, DateTime filter, ActivityType? activityType)
        {
            var dateExpression = dateFilter.GetFilterExpression(filter);
            if (activityFilter == null)
            {
                return dateExpression;
            }

            if (!activityType.HasValue)
            {
                throw new InvalidOperationException($"Filter by activity type is set, but no activity type provided. EntityCounter:{this.GetType()}");
            }

            var exp = Expression.And(dateExpression, activityFilter.GetFilterExpression(activityType.Value));
            return Expression.Lambda<Func<T, bool>>(exp);
        }
    }

    public interface IFilterPredicate<TSource> where TSource : class, new()
    {
        Expression<Func<TSource, bool>> GetFilterExpression(object filterInput);
    }

    public abstract class FilterPredicate<TSource, TInput, TValue> : IFilterPredicate<TSource> where TSource : class, new()
    {
        protected readonly Expression<Func<TSource, TValue>> _filterExpression;
        protected readonly Func<TInput, TValue> _valueConversionFunc;
        protected FilterPredicate(Expression<Func<TSource, TValue>> filterExpression, Func<TInput, TValue> valueConversionFunc)
        {
            _filterExpression = filterExpression;
            _valueConversionFunc = valueConversionFunc;
        }

        public virtual Expression<Func<TSource, bool>> GetFilterExpression(object filterInput)
        {
            var value = _valueConversionFunc((TInput)filterInput);
            return Expression.Lambda<Func<TSource, bool>>(
                Expression.GreaterThanOrEqual(
                    _filterExpression.Body,
                    Expression.Constant(value, typeof(TValue))
                ), _filterExpression.Parameters);
        }
    }

    public class DateFilterPredicate<TSource> : FilterPredicate<TSource, DateTime, DateTime> where TSource : class, new()
    {
        public DateFilterPredicate(Expression<Func<TSource, DateTime>> filterExpression)
        : base(filterExpression, a => a)
        {
        }
    }

    public class DateEpochFilterPredicate<TSource> : FilterPredicate<TSource, DateTime, int> where TSource : class, new()
    {
        public DateEpochFilterPredicate(Expression<Func<TSource, int>> filterExpression)
            : base(filterExpression, a => a.ConvertToEpoch())
        {
        }
    }

    public class ActivityTypeFilterPredicate<TSource, TValue> : FilterPredicate<TSource, ActivityType, TValue> where TSource : class, new()
    {
        public ActivityTypeFilterPredicate(Expression<Func<TSource, TValue>> filterExpression, Func<ActivityType, TValue> valueConversionFunc)
            : base(filterExpression, valueConversionFunc)
        {
        }

        public override Expression<Func<TSource, bool>> GetFilterExpression(object filterInput)
        {
            var value = _valueConversionFunc((ActivityType)filterInput);
            return Expression.Lambda<Func<TSource, bool>>(
                Expression.Equal(
                    _filterExpression.Body,
                    Expression.Constant(value, typeof(TValue))
                ), _filterExpression.Parameters);
        }
    }

    public class ActivityTypeAsStringFilterPredicate<TSource> : FilterPredicate<TSource, ActivityType, string> where TSource : class, new()
    {
        public ActivityTypeAsStringFilterPredicate(Expression<Func<TSource, string>> filterExpression)
            : base(filterExpression, a => a.ToString())
        {
        }

        public override Expression<Func<TSource, bool>> GetFilterExpression(object filterInput)
        {
            var value = _valueConversionFunc((ActivityType)filterInput);
            return Expression.Lambda<Func<TSource, bool>>(
                Expression.Equal(
                    _filterExpression.Body,
                    Expression.Constant(value, typeof(string))
                ), _filterExpression.Parameters);
        }
    }
}
