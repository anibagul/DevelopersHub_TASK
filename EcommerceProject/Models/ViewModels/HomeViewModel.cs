using EcommerceProject.Models;

namespace EcommerceProject.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Product> RecommendedProducts { get; set; } = new List<Product>();
        public List<Product> DealsProducts { get; set; } = new List<Product>();
    }
}