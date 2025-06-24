using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineStoreWebAPI.Model
{
    public class OrderItem
    {
        private int OrderItemId { get; set; }
        private int OrderId { get; set; }
        private int productId { get; set; }
        private int quantity { get; set; }
        private double price { get; set; }

    }
}
