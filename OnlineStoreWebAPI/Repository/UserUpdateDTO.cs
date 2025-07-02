using System.ComponentModel.DataAnnotations;

namespace OnlineStoreWebAPI.Repository
{
    public class UserUpdateDTO
    {
        [MaxLength(15)]
        public string? firstName { get; set; }
        [MaxLength(20)]
        public string? lastName { get; set; }
        [EmailAddress]
        public string? email { get; set; }
        [MaxLength(100)]
        [MinLength(8)]
        public string? password { get; set; }
        [MaxLength(200)]
        public string? address { get; set; }
    }
}
