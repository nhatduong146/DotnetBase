namespace DotnetBase.Domain.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Category")]
    public class Category : BaseEntity
    {
        public Category() : base()
        {

        }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        public List<ProductInCategory> ProductInCategories { get; set; }
    }
}
