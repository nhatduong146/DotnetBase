using DotnetBase.Infrastructure.Common.Models;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace DotnetBase.Infrastructure.Helpers
{
    public static class QueryHelper
    {
        public static PagedList<T> SortAndPaginationDynamic<T>(IQueryable<T> data, SortAndPaginationModel sortAndPaginationModel)
        {
            var order = new StringBuilder();
            foreach (var sortingProperty in sortAndPaginationModel.SortingProperties)
            {
                var matchedPropertyName = typeof(T).GetProperties().Select(p => p.Name).FirstOrDefault(x => x.ToLower() == sortingProperty.PropertySort);
                if (!string.IsNullOrEmpty(matchedPropertyName))
                {
                    var orderType = sortingProperty.IsDesc ? "DESC" : "ASC";
                    order.Append($"{matchedPropertyName} {orderType},");
                }
            }

            // remove last comma
            // order.Length--;

            var dataOrder = data.OrderBy(string.IsNullOrEmpty(order.ToString()) ? $"{sortAndPaginationModel.DefaultSortingProperty} ASC" : order.ToString());
            return new PagedList<T>(dataOrder,
                sortAndPaginationModel.PageIndex ?? CommonConstants.Config.DEFAULT_SKIP,
                sortAndPaginationModel.PageSize ?? CommonConstants.Config.DEFAULT_TAKE);
        }
    }
}
