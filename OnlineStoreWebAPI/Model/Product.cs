using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineStoreWebAPI.Model
{
    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Required]
        public int productId { get; set; }
        [Required]
        public string name { get; set; }
        public string description { get; set; }
        [Required]
        public double price { get; set; }
        [Required]
        public int StockQuantity { get; set; }
        public ICollection<OrderItem>? orderItems {get; set;}
    }
}
