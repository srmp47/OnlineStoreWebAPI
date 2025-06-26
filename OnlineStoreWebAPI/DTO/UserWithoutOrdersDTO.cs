namespace OnlineStoreWebAPI.DTO
{
    public class UserWithoutOrdersDTO
    {
        public int userId { get; set; }
        public string firstName { get; set; }
        public string? lastName { get; set; }
        public string? email { get; set; }
        public string password { get; set; }
        public string? address { get; set; }
        public bool isActive { get; set; }
    }
}
