using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.DTO
{
    public class UserWithoutIsActiveDTO
    {

        public string firstName { get; set; } 
        public string? lastName { get; set; }
        public string? email { get; set; }
        public string password { get; set; }
        public string? address { get; set; }
        public ICollection<Order>? orders { get; set; }
    }
}
