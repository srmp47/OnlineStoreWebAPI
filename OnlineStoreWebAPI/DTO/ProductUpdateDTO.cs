using System.ComponentModel.DataAnnotations;

namespace OnlineStoreWebAPI.DTO
{
    public class ProductUpdateDTO
    {
        
        public string? name { get; set; }
        public string? description { get; set; }
        public double? price { get; set; }
        public int? StockQuantity { get; set; }
    }
}
