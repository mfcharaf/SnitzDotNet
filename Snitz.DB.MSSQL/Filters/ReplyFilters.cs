using System;
using System.Linq;

namespace SnitzData.Filters
{
    internal static class ReplyFilters
    {
        public static IQueryable<Reply> WhereAuthorIs(this IQueryable<Reply> replies, string author)
        {
            return string.IsNullOrEmpty(author) ? replies : replies.Where(r => r.Author.Name == author);
        }
        public static IQueryable<Reply> WhereBodyContains(this IQueryable<Reply> replies, string bodyText)
        {
            return string.IsNullOrEmpty(bodyText) ? replies : replies.Where(p => p.Message.Contains(bodyText));
        }

        public static IQueryable<Reply> IsCreatedOn(this IQueryable<Reply> replies, DateTime? createdOn)
        {
            if (!createdOn.HasValue || createdOn.Value == DateTime.MinValue)
                return replies;

            return replies.Where(p => p.Date == createdOn.Value.Date);
        }

        public static IQueryable<Reply> WithPaging(this IQueryable<Reply> replies, int? startRow, int? rowCount)
        {
            if ((!startRow.HasValue) && (!rowCount.HasValue || rowCount.Value == 0))
                return replies;

            return (IQueryable<Reply>) replies.Skip((int)startRow).Take((int)rowCount);
        }

    }
}