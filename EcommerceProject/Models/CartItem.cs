using System.ComponentModel.DataAnnotations;

namespace EcommerceProject.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int ProductId { get; set; }

        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
        public int Quantity { get; set; } = 1;

        public ApplicationUser? User { get; set; }
        public Product? Product { get; set; }
    }
}
