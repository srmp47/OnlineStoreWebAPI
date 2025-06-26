using AutoMapper;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.DTO;

namespace OnlineStoreWebAPI.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<UserWithoutIsActiveDTO, User>();
            CreateMap<OrderItem, UserWithoutIsActiveDTO>();
            CreateMap<User, UserWithoutOrdersDTO>();
            CreateMap<UserWithoutOrdersDTO, User>();
        }
    }
}
