using System.ComponentModel.DataAnnotations;

namespace OnlineStoreWebAPI.DTO
{
    public class ProductUpdateDTO
    {
        [Required]
        public string? name { get; set; }
        [Required]
        public string? description { get; set; }
        [Required]
        public double? price { get; set; }
        [Required]
        public int? StockQuantity { get; set; }
    }
}
