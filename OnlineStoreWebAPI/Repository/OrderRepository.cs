using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;

namespace OnlineStoreWebAPI.Repository
{
    public class OrderRepository:IOrderRepository
    {
        private readonly OnlineStoreDBContext _context;
        public OrderRepository(OnlineStoreDBContext _context)
        {
            this._context = _context;
        }
        public async Task<IEnumerable<Order>> getAllOrdersAsync(PaginationParameters paginationParameters)
        {
            var allOrders = await _context.Orders
                .Include(o=>o.User)
                .Include(o => o.orderItems)
                .ThenInclude(o => o.Product)
                .Skip(paginationParameters.PageSize * (paginationParameters.PageId - 1))
                .Take(paginationParameters.PageSize)
                .ToListAsync();
            return allOrders;
        }
        public async Task<Order?> getOrderByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.orderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            return order;
        }
        public async Task<IEnumerable<Order>> getAllOrdersOfUserByIdAsync(int userId, PaginationParameters paginationParameters)
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.orderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.userId == userId)
                .Skip(paginationParameters.PageSize * (paginationParameters.PageId - 1))
                .Take(paginationParameters.PageSize)
                .ToListAsync();
            return orders;
        }
        public async Task<bool> isThereOrderWithIdAsync(int orderId)
        {
            var isThere = await  _context.Orders.AnyAsync(o=> o.OrderId == orderId);
            return isThere;
        }
        public async Task createNewOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> deleteOrderWithIdAsync(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
            {
                return false; // Order not found
            }
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task updateOrder(Order order)
        {
            _context.Orders.Attach(order);
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task setUserInOrder(Order order, int userId)
        {
            order.userId = userId;
            User user = await _context.Users.FirstOrDefaultAsync(u => u.userId == userId);
            order.User = user;
        }
        public async Task setOrderAndProductInOrderItem(OrderItem orderItem)
        {
            orderItem.Product = await _context.Products.FirstOrDefaultAsync(p => p.productId == orderItem.productId);
            orderItem.Order = await  _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderItem.OrderId);

        }

        public async Task<int> getUserIdOfOrder(int orderId)
        {
            var userId = await _context.Orders.Where(o => o.OrderId == orderId)
                .Select(o => o.userId)
                .FirstOrDefaultAsync();
            return userId;
        }

    }
}
