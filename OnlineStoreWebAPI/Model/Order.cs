using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineStoreWebAPI.Model
{
    public class Order
    {
        public Order()
        {
            this.date = DateTime.Now;
            this.status = OrderStatus.Pending;
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Required]
        public int OrderId{ get; set; }
        [ForeignKey("User")]
        [Required]
        public int userId { get; set; }
        public User User { get; set; }
        [Required]
        public DateTime date { get; set; } = DateTime.Now;
        [Range(0, double.MaxValue)]
        public double totalAmount { get; set; } = 0;
        public ICollection<OrderItem> orderItems { get; set; } = new List<OrderItem>();
        [Required]
        public OrderStatus status { get; set; } = OrderStatus.Pending;



    }
}
