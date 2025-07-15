using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;

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

        public async Task<IEnumerable<Order>> getAllOrdersAsync(PaginationParameters paginationParameters)
        {
            //return await context.Orders
            //.Include(o => o.User)
            //.Include(o => o.orderItems)
            //.ThenInclude(oi => oi.Product)
            //.ToListAsync();
            IQueryable<Order> orders = context.Orders.Include(o => o.User).Include(o => o.orderItems)
            .ThenInclude(oi => oi.Product); 

            orders = orders
                .Skip(paginationParameters.PageSize * (paginationParameters.PageId - 1))
                .Take(paginationParameters.PageSize);

            return await orders.ToArrayAsync();
        }

        public async Task<IEnumerable<Order>> getAllOrdersOfUserByIdAsync
            (int userId, PaginationParameters paginationParameters)
        {
            //return await context.Orders.Where(o => o.userId == userId).ToListAsync();
            IQueryable<Order> orders = context.Orders.Include(o => o.User).Include(o => o.orderItems)
           .ThenInclude(oi => oi.Product).Where(o => o.userId == userId);

            orders = orders
                .Skip(paginationParameters.PageSize * (paginationParameters.PageId - 1))
                .Take(paginationParameters.PageSize);
            return await orders.ToArrayAsync();
        }

        public async Task<Order?> getOrderByOrderIdAsync(int orderId)
        {
            return await context.Orders.Where(o =>  o.OrderId == orderId)
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
        public async Task<Order> deleteOrderByIdAsync(int id)
        {
            var order = await context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
            context.Orders.Remove(order);
            await context.SaveChangesAsync();
            return order;
        }
        //in this function , I set User in Order by user id
        public async Task setUserInOrder(Order order, int userId)
        {
            order.userId = userId;
            order.User = await context.Users.FirstOrDefaultAsync(o => o.userId == userId);
        }
        public async Task setOrderAndProductInOrderItem(OrderItem orderItem)
        {
            orderItem.Product= context.Products.FirstOrDefault(p => p.productId == orderItem.productId);
            orderItem.Order = context.Orders.FirstOrDefault(o => o.OrderId == orderItem.OrderId);

        }

        public async Task<IEnumerable<OrderItem>> getAllOrderItemsByOrderIdAsync(int orderId)
        {
            return await context.OrderItems.Where(oi => oi.OrderId == orderId).ToListAsync();
        }

        public async Task<Order> changeOrderStatusByOrderIdAsync(int id, OrderStatus status)
        {
            var order = await context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
            order.status = status;
            await context.SaveChangesAsync();
            return order;
        }
    }
}
