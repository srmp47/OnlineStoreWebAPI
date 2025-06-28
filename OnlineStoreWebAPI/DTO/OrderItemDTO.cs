using OnlineStoreWebAPI.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace OnlineStoreWebAPI.DTO

{
    public class OrderItemDTO
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int productId { get; set; }
        [Required]
        public int quantity { get; set; }

    }
}
