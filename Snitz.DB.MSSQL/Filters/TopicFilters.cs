using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using SnitzCommon;


namespace SnitzData.Filters
{
    /// <summary>
    /// Topic filtering Extensions for search
    /// </summary>
    internal static class TopicFilters
    {
        /// <summary>
        /// Filters topics by subject
        /// </summary>
        /// <param name="topics"></param>
        /// <param name="searchfor"></param>
        /// <param name="match"></param>
        /// <param name="both">flag for searching subject or subject and message</param>
        /// <param name="dc"></param>
        /// <returns></returns>
        public static IQueryable<Topic> WhereMessageContains(this IQueryable<Topic> topics, string[] searchfor, string match, bool both, SnitzDataClassesDataContext dc)
        {
            if (searchfor == null)
                return topics;


            switch (match)
            {
                case "all":
                    if (!both)
                        return topics.LikeAll(t => t.Subject, searchfor);

                    var resall = (from t in topics select t)
                        .LikeAll(p => p.Subject + p.Message, searchfor)
                        .Concat(from t in topics
                                join reply in dc.Replies.LikeAll(r => r.Message, searchfor) on t.Id equals
                                    reply.TopicId
                                select t);
                    return resall;
                case "any":
                    if (!both)
                        return topics.LikeAny(t => t.Subject, searchfor);

                    var resany = (from t in topics select t)
                        .LikeAny(p => p.Subject + p.Message, searchfor)
                        .Concat(from t in topics
                                join reply in dc.Replies.LikeAny(r => r.Message, searchfor) on t.Id equals reply.TopicId
                                select t);
                    return resany;
                default:
                    if (!both)
                        return topics.LikeAny(t => t.Subject, searchfor);

                    var res = (from t in topics select t)
                        .LikeAny(p => p.Subject + p.Message, searchfor)
                        .Concat(from t in topics
                                join reply in dc.Replies.LikeAny(r => r.Message, searchfor) on t.Id equals reply.TopicId
                                select t);
                    return res;
            }
        }

        public static IQueryable<Topic> WhereAuthorIs(this IQueryable<Topic> topics, string author, SnitzDataClassesDataContext dc, string authorPostType)
        {
            if (string.IsNullOrEmpty(author))
                return topics;

            if (authorPostType == "topic")
                return topics.Join(dc.Members.Where(a => a.Name == author), t => t.T_AUTHOR, a => a.Id, (t, a) => t);

            var test = (from t in topics
                        join atopic in dc.Members on t.T_AUTHOR equals atopic.Id
                        where atopic.Name == author
                        select t);
            test = test.Concat(from t in topics
                               join reply in dc.Replies on t.Id equals reply.TopicId
                               join areply in dc.Members on reply.R_AUTHOR equals areply.Id
                               where areply.Name == author
                               select t);
            return test;
        }

        /// <summary>
        /// Find topics by dcreation date
        /// </summary>
        /// <param name="topics"></param>
        /// <param name="created"> </param>
        /// <returns></returns>
        public static IQueryable<Topic> CreatedSince(this IQueryable<Topic> topics, DateTime? created)
        {
            if (!created.HasValue || created.Value == DateTime.MinValue)
                return topics;
            return (topics.Where(t => String.Compare(t.T_DATE, created.Value.ToForumDateStr(), StringComparison.Ordinal) >= 0));
            //return topics.Where(t => t.T_DATE >= created.Value.ToForumDateStr());
        }
        public static IQueryable<Topic> CreatedBefore(this IQueryable<Topic> topics, DateTime? created)
        {
            if (!created.HasValue || created.Value == DateTime.MinValue)
                return topics;

            return (from t in topics
                    where String.Compare(t.T_DATE, created.Value.ToForumDateStr(), StringComparison.Ordinal) <= 0
                    select t);
        }
        /// <summary>
        /// Returns a page of topics
        /// </summary>
        /// <param name="topics"></param>
        /// <param name="startRow"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public static IEnumerable<Topic> WithPaging(this IQueryable<Topic> topics, int? startRow, int? rowCount)
        {
            if ((!startRow.HasValue) && (!rowCount.HasValue || rowCount.Value == 0))
                return topics;

            return (IQueryable<Topic>)topics.Skip((int)startRow).Take((int)rowCount);
        }

        private static IQueryable<TEntity> LikeAny<TEntity>(this IQueryable<TEntity> query,
            Expression<Func<TEntity, string>> selector, IEnumerable<string> values)
        {
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            var enumerable = values as string[] ?? values.ToArray();
            if (!enumerable.Any())
            {
                return query;
            }
            var p = selector.Parameters.Single();
            var conditions = enumerable.Select(v =>
                (Expression)Expression.Call(typeof(SqlMethods), "Like", null,
                    selector.Body, Expression.Constant("%" + v + "%")));
            var body = conditions.Aggregate(Expression.Or);
            return query.Where(Expression.Lambda<Func<TEntity, bool>>(body, p));
        }

        private static IQueryable<TEntity> LikeAll<TEntity>(this IQueryable<TEntity> query,
            Expression<Func<TEntity, string>> selector, IEnumerable<string> values)
        {
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            var enumerable = values as string[] ?? values.ToArray();
            if (!enumerable.Any())
            {
                return query;
            }
            var p = selector.Parameters.Single();
            var conditions = enumerable.Select(v =>
                (Expression)Expression.Call(typeof(SqlMethods), "Like", null,
                    selector.Body, Expression.Constant("%" + v + "%")));
            var body = conditions.Aggregate(Expression.And);
            return query.Where(Expression.Lambda<Func<TEntity, bool>>(body, p));
        }

    }
}
