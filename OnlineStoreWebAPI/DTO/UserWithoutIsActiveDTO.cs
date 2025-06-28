using OnlineStoreWebAPI.Model;
using System.ComponentModel.DataAnnotations;
namespace OnlineStoreWebAPI.DTO
{
    public class UserWithoutIsActiveDTO
    {
        [Required]
        [MaxLength(15)]
        public string firstName { get; set; }
        [MaxLength(20)]
        public string? lastName { get; set; }
        [EmailAddress]
        public string? email { get; set; }
        [MaxLength(100)]
        [Required]
        [MinLength(8)]
        public string password { get; set; }
        [MaxLength(200)]
        public string? address { get; set; }
        public ICollection<Order>? orders { get; set; }
    }
}
