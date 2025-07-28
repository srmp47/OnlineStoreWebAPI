using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;

namespace OnlineStoreWebAPI.Repository
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository orderItemRepository;
        private readonly IMapper mapper;

        public OrderItemService(IOrderItemRepository orderItemRepository , IMapper inputMapper)
        {
            this.orderItemRepository = orderItemRepository;
            this.mapper = inputMapper;
            
        }

        public async Task<OrderItem> changeQuantityByOrderItemId(int id, int quantity)
        {
            var orderItem = await orderItemRepository.getOrderItemByIdAsync(id);
            var order = orderItem.Order;
            order.totalAmount += (quantity - orderItem.quantity) * orderItem.Product.price;
            orderItem.quantity = quantity;
            orderItem.price = quantity * orderItem.Product.price;
            await orderItemRepository.updateOrderItemAsync(orderItem);
            return orderItem;
        }

        public async Task<OrderItem> createNewOrderItemAsync(OrderItem orderItem)
        {
            await orderItemRepository.createNewOrderItemAsync(orderItem);
            return orderItem;
        }

        public async Task<bool> deleteOrderItemByIdAsync(int id)
        {
            var deletedSuccessfully = await orderItemRepository.deleteOrderItemByIdAsync(id);
            return deletedSuccessfully;
        }

        public async Task<IEnumerable<OrderItem>> getAllOrderItemsAsync
            (PaginationParameters paginationParameters)
        {
            var orderItems = await orderItemRepository.getAllOrderItemsAsync(paginationParameters);
            return  orderItems;
        }

        public async Task<OrderItem?> getOrderItemByOrderItemId( int orderItemId)
        {
            var orderItem = await orderItemRepository.getOrderItemByIdAsync(orderItemId);
            return orderItem;
        }

        public async Task<bool> isThereOrderItemById(int id)
        {
            var isThere = await orderItemRepository.isThereOrderItemWithIdAsync(id);
            return isThere;
        }

        public async Task setOrderAndProductInOrderItem(OrderItem orderItem)
        {
            await orderItemRepository.setOrderAndProductInOrderItem(orderItem);
        }

        public async Task<OrderItem> updateOrderItemAsync(OrderItem orderItem)
        {
           var newOrderItem = await orderItemRepository.updateOrderItemAsync(orderItem);
           return newOrderItem;

        }
        public async Task<IEnumerable<OrderItem>> getAllOrderItemsByOrderIdAsync(int orderId, PaginationParameters paginationParameters)
        {
            var orders = await orderItemRepository.getAllOrderItemsByOrderIdAsync(orderId, paginationParameters);
            return orders;
        }
    }
}
