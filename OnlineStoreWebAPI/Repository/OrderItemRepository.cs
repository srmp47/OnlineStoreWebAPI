using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;

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

        public async Task<OrderItem> changeQuantityByOrderItemId(int id, int quantity)
        {
            var orderItem = await context.OrderItems.FirstOrDefaultAsync(oi => oi.OrderItemId == id);
            orderItem.quantity = quantity;
            context.Update(orderItem);
            await context.SaveChangesAsync();
            return orderItem;
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

        public async Task<IEnumerable<OrderItem>> getAllOrderItemsAsync
            (PaginationParameters paginationParameters)
        {
            //return await context.OrderItems.OrderBy(oi => oi.OrderItemId).ToListAsync();
            IQueryable<OrderItem> orderItems  = context.OrderItems;
            orderItems = orderItems.Skip(paginationParameters.PageSize*(paginationParameters.PageId-1))
                .Take(paginationParameters.PageSize);
            return await orderItems.ToArrayAsync();
        }

        public async Task<OrderItem?> getOrderItemByOrderItemId( int orderItemId)
        {
            // use Include to get Order from order item
            return await context.OrderItems.Include(oi=>oi.Order).Include(oi=>oi.Product)
                .Where(oi => oi.OrderItemId == orderItemId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> isThereOrderItemById(int id)
        {
            return await context.OrderItems.AnyAsync(oi => oi.OrderItemId == id);
        }

        public async Task setOrderAndProductInOrderItem(OrderItem orderItem)
        {
            orderItem.Product = await context.Products.FirstOrDefaultAsync
                (p => p.productId == orderItem.productId);
            orderItem.Order = await context.Orders.FirstOrDefaultAsync
                (o => o.OrderId == orderItem.OrderId);
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
