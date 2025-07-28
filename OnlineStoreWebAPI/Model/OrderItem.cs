using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineStoreWebAPI.Model
{
    public class OrderItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Required]
        public int OrderItemId { get; set; }
        [ForeignKey("Order")]
        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }
        [ForeignKey("Product")]
        [Required]
        public int productId { get; set; }
        public Product Product { get; set; }
        [Required]
        public int quantity { get; set; }
        [Required]
        public double price { get; set; }

        
    }
}
