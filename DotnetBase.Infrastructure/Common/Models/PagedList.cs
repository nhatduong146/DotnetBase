namespace DotnetBase.Infrastructure.Common.Models
{
    using System.Linq;

    public class PagedList<TEntity>
    {
        #region Properties

        public int PageIndex { get; private set; }

        public int PageSize { get; private set; }

        public int TotalCount { get; private set; }

        public int TotalPages { get; private set; }

        public IQueryable<TEntity> Sources { get; set; }

        #endregion

        #region Constructor

        public PagedList(IQueryable<TEntity> source, int pageIndex, int pageSize)
        {
            var total = source.Count();
            TotalCount = total;
            TotalPages = total / pageSize;

            if (total % pageSize > 0)
                TotalPages++;

            PageSize = pageSize;
            PageIndex = pageIndex;
            Sources = source.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        #endregion

        #region Methods

        public bool HasPreviousPage
        {
            get { return (PageIndex > 1); }
        }

        public bool HasNextPage
        {
            get { return (PageIndex < TotalPages); }
        }

        #endregion
    }
}
