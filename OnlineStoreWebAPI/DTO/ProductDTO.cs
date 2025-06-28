using OnlineStoreWebAPI.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStoreWebAPI.DTO
{
    public class ProductDTO
    {
        
        [Required]
        public string name { get; set; }
        public string? description { get; set; }
        [Required]
        public double price { get; set; }
        [Required]
        public int StockQuantity { get; set; }
    }
}
