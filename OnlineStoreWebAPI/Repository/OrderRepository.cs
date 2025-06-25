using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OnlineStoreDBContext context;
        private readonly IMapper mapper;
        public OrderRepository(OnlineStoreDBContext inputContext, IMapper inputMapper)
        {
            this.context = inputContext;
            this.mapper = inputMapper;
        }

        public async  Task cancelOrderByIdAsync(int id)
        {
            var order = await context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
            order.status = OrderStatus.Cancelled; 
            await context.SaveChangesAsync();
        }

        public async Task<Order> createNewOrderAsync(Order order)
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync();
            return order;
        }

        public async Task<IEnumerable<Order>> getAllOrdersAsync()
        {
            return await context.Orders.OrderBy(o => o.OrderId).ToListAsync();
        }

        public async Task<IEnumerable<Order>> getAllOrdersOfUserByIdAsync(int userId)
        {
            return await context.Orders.Where(o => o.userId == userId).ToListAsync();
        }

        public async Task<Order?> getOrderByUserIdAndOrderIdAsync(int userId, int orderId)
        {
            return await context.Orders.Where(o => o.userId == userId && o.OrderId == orderId)
                .FirstOrDefaultAsync();
        }



        public async Task<bool> isThereOrderByIdAsync(int id)
        {
            return await context.Orders.AnyAsync(o => o.OrderId == id);
        }

        public async Task<Order> updateOrderAsync(Order order)
        {
            var currentOrder = await context.Orders.FirstOrDefaultAsync(o => o.OrderId == order.OrderId);
            mapper.Map(currentOrder, order);
            await context.SaveChangesAsync();
            return order;
        }

        
    }
}
