using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.Repository
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly OnlineStoreDBContext context;
        public OrderItemRepository(OnlineStoreDBContext inputContext)
        {
            this.context = inputContext;
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
    }
}
