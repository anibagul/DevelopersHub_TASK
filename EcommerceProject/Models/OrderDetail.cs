using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceProject.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }

        [NotMapped]
        public decimal LineTotal => Quantity * Price;

        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
