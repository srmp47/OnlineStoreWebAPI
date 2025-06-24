using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineStoreWebAPI.Model
{
    public class Order
    {
        private int OrderId{ get; set; }
        private int userId { get; set; }
        private DateTime date { get; set; }
        public double totalAmount { get; set; }
    }
}
