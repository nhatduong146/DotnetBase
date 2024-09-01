using System.Collections.Generic;

namespace DotnetBase.Infrastructure.Common.Models
{
    public class BaseRequestModel
    {
        public BaseRequestModel()
        {
        }

        public int? PageIndex { get; set; }

        public int? PageSize { get; set; }

        public List<BaseSortingProperty> SortingProperties { get; set; } = [];

        public string SearchTerm { get; set; }
    }
}
