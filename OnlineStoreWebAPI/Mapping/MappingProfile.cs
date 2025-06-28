using AutoMapper;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Repository;

namespace OnlineStoreWebAPI.Mapping
{
    public class MappingProfile:Profile
    {
        private readonly IUserRepository userRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IProductRepository productRepository;
        public MappingProfile(IUserRepository userRepository , IOrderRepository orderRepository,
            IProductRepository productRepository)
        {
            this.orderRepository = orderRepository;
            this.userRepository = userRepository;
            this.productRepository = productRepository;
            CreateMap<UserWithoutIsActiveDTO, User>();
            CreateMap<OrderItem, UserWithoutIsActiveDTO>();
            CreateMap<User, UserWithoutOrdersDTO>();
            CreateMap<UserWithoutOrdersDTO, User>();
            CreateMap<ProductDTO, Product>();
            CreateMap<OrderDTO, Order>().ForMember
                (dest => dest.User, opt => opt.MapFrom
                (src => userRepository.getUserByIdAsync(src.userId).Result));
            CreateMap<OrderItemDTO, OrderItem>().ForMember
                (dest => dest.Order, opt => opt.MapFrom
                (src => orderRepository.getOrderByOrderIdAsync(src.OrderId).Result)).ForMember
                (dest => dest.User, opt => opt.MapFrom
               (src => productRepository.getProductByIdAsync(src.productId).Result));
        }
    }
}
