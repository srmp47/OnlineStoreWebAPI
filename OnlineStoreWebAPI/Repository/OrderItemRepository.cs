using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.DTO;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;

namespace OnlineStoreWebAPI.Repository
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly OnlineStoreDBContext _context;

        public OrderItemRepository(OnlineStoreDBContext _context)
        {
            this._context = _context;
        }
        public async Task createNewOrderItemAsync(OrderItem orderItem)
        {
            orderItem.price = orderItem.Product.price * orderItem.quantity;
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderItem.OrderId);
            order.totalAmount += orderItem.price;
            _context.Orders.Update(order);
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> deleteOrderItemByIdAsync(int id)
        {
            var orderItem = await _context.OrderItems.FirstOrDefaultAsync(oi => oi.OrderItemId == id);
            if (orderItem == null)
            {
                return false; // order item not found
            }
            var order = orderItem.Order;
            _context.OrderItems.Remove(orderItem);
            order.totalAmount -= orderItem.price;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<OrderItem>> getAllOrderItemsAsync(PaginationParameters paginationParameters)
        {
            var orderItems = await _context.OrderItems
                .Skip((paginationParameters.PageId - 1) * paginationParameters.PageSize)
                .Take(paginationParameters.PageSize)
                .ToListAsync();
            return orderItems;
        }

        public async Task<IEnumerable<OrderItem>> getAllOrderItemsByOrderIdAsync(int orderId, PaginationParameters paginationParameters)
        {
            var orderItems = await _context.OrderItems.Where(oi => oi.OrderId == orderId).ToListAsync();
            return orderItems;
        }

        public async Task<OrderItem?> getOrderItemByIdAsync(int id)
        {
            var orderItem = await  _context.OrderItems.Include(oi => oi.Order).Include(oi => oi.Product)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == id);
            return orderItem;
        }

        public async Task<bool> isThereOrderItemWithIdAsync(int id)
        {
            var isThere = await _context.OrderItems.AnyAsync(oi => oi.OrderItemId == id);
            return isThere;
        }

        public async Task setOrderAndProductInOrderItem(OrderItem orderItem)
        {
            orderItem.Product = await _context.Products.FirstOrDefaultAsync
                (p => p.productId == orderItem.productId);
            orderItem.Order = await _context.Orders.FirstOrDefaultAsync
                (o => o.OrderId == orderItem.OrderId);
        }

        public async Task<OrderItem> updateOrderItemAsync(OrderItem orderItem)
        {
            _context.OrderItems.Attach(orderItem);
            _context.Entry(orderItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return orderItem;
        }

        

    }
}
