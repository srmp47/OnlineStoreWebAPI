using System.ComponentModel.DataAnnotations;

namespace OnlineStoreWebAPI.Repository
{
    public class UserUpdateDTO
    {
        [Required]
        [MaxLength(15)]
        public string? firstName { get; set; }
        [Required]
        [MaxLength(20)]
        public string? lastName { get; set; }
        [Required]
        [EmailAddress]
        public string? email { get; set; }
        [Required]
        [MaxLength(100)]
        [MinLength(8)]
        public string? password { get; set; }
        [Required]
        [MaxLength(200)]
        public string? address { get; set; }
    }
}
