using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OnlineStoreDBContext context;
        public OrderRepository(OnlineStoreDBContext inputContext)
        {
            this.context = inputContext;
        }

        public void cancleOrderByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Order> createNewOrderAsync(Order order)
        {
            throw new NotImplementedException();
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

        public async Task<bool> isThereOrderById(int id)
        {
            return await context.Orders.AnyAsync(o => o.OrderId == id);
        }

        public Task<bool> isThereOrderByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Order> updateOrderAsync(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
