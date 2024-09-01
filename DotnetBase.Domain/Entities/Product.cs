namespace DotnetBase.Domain.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Product")]
    public class Product : BaseEntity
    {
        public Product() : base()
        {

        }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        public decimal ImportPrice { get; set; }

        public decimal RevenuPercent { get; set; }

        public List<ProductInCategory> ProductInCategories { get; set; }
    }
}
