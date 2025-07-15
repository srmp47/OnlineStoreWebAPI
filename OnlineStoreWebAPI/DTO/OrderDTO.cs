using OnlineStoreWebAPI.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStoreWebAPI.DTO
{
    public class OrderDTO
    {
        public ICollection<OrderItemDTO>? orderItemDTOs { get; set; }

    }
}
