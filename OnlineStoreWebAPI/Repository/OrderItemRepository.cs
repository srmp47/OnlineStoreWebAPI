using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.Repository
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly OnlineStoreDBContext context;
        private readonly IMapper mapper;
        public OrderItemRepository(OnlineStoreDBContext inputContext , IMapper inputMapper)
        {
            this.context = inputContext;
            this.mapper = inputMapper;
        }

        public async Task<OrderItem> createNewOrderItemAsync(OrderItem orderItem)
        {
            context.OrderItems.Add(orderItem);
            await context.SaveChangesAsync();
            return orderItem;
        }

        public async Task<OrderItem> deleteOrderItemByIdAsync(int id)
        {
            var orderItem = await context.OrderItems.FirstOrDefaultAsync(oi => oi.OrderItemId == id);
            context.OrderItems.Remove(orderItem);
            await context.SaveChangesAsync(); 
            return orderItem;
        }

        public async Task<IEnumerable<OrderItem>> getAllOrderItemsByOrderIdAsync(int orderId)
        {
            return await context.OrderItems.Where(oi => oi.OrderId == orderId).ToListAsync();
        }

        public async Task<OrderItem?> getOrderItemByOrderIdAndOrderItemId(int orderId, int orderItemId)
        {
            return await context.OrderItems.Where(oi => oi.OrderId == orderId && oi.OrderItemId == orderItemId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> isThereOrderItemById(int id)
        {
            return await context.OrderItems.AnyAsync(oi => oi.OrderItemId == id);
        }

        public async Task<OrderItem> updateOrderItemAsync(OrderItem orderItem)
        {
            var currentOrderItem =await context.OrderItems.FirstOrDefaultAsync
                (oi =>  oi.OrderItemId==orderItem.OrderItemId);
            mapper.Map(currentOrderItem, orderItem);
            await context.SaveChangesAsync();
            return orderItem;
            
        }
    }
}
