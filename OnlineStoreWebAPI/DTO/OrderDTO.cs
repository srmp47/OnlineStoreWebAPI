using OnlineStoreWebAPI.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStoreWebAPI.DTO
{
    public class OrderDTO
    {
        [Required]
        public int userId { get; set; }
        [Range(0, double.MaxValue)]
        public double totalAmount { get; set; } = 0;
        public ICollection<OrderItemDTO>? orderItemDTOs { get; set; }

    }
}
