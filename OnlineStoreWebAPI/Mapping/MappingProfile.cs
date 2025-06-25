using AutoMapper;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.DTO;

namespace OnlineStoreWebAPI.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDTO, User>();
            CreateMap<OrderItem, UserDTO>();
        }
    }
}
