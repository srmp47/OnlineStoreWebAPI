using AutoMapper;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Repository;
using OnlineStoreWebAPI.GraphQL;

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
            CreateMap<ProductDTO, Product>();
            CreateMap<OrderDTO, Order>();
            CreateMap<OrderItemDTO, OrderItem>();
            CreateMap<UserUpdateDTO, User>();
            CreateMap<User, UserUpdateDTO>();
            CreateMap<ProductUpdateDTO, Product>();
            
            
        }
    }
}
