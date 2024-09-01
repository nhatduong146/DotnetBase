using System.Collections.Generic;

namespace DotnetBase.Infrastructure.Common.Models
{
    public class SortAndPaginationModel
    {
        public int? PageIndex { get; set; }

        public int? PageSize { get; set; }

        public List<BaseSortingProperty> SortingProperties { get; set; } = [];

        public string DefaultSortingProperty { get; set; }
    }
}
