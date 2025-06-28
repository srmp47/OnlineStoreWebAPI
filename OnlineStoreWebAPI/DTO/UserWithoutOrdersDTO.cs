using System.ComponentModel.DataAnnotations;

namespace OnlineStoreWebAPI.DTO
{
    public class UserWithoutOrdersDTO
    {
        [Required]
        [Key]
        public int userId { get; set; }
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
        [Required]
        public bool isActive { get; set; }
    }
}
