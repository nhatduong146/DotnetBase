using System;

namespace DotnetBase.Infrastructure.Common.Attributes
{
    public class ExportExcelAttribute : Attribute
    {
        public string DisplayName { get; set; }

        public int Priority { get; set; }
    }
}
