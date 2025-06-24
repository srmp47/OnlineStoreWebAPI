using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineStoreWebAPI.Model
{
    public class Product
    {
        private int productId { get; set; }
        private string name { get; set; }
        private string description { get; set; }
        private double price { get; set; }
        private int StockQuantity { get; set; }
    }
}
