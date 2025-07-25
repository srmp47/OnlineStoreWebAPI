﻿using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;

namespace OnlineStoreWebAPI.Repository
{
    public interface IOrderItemRepository
    {
        public Task<OrderItem?> getOrderItemByOrderItemId(int orderItemId);
        public Task<bool> isThereOrderItemById(int id);
        public Task<OrderItem> createNewOrderItemAsync(OrderItem orderItem);
        public Task<OrderItem> updateOrderItemAsync(OrderItem orderItem);
        public Task<OrderItem> deleteOrderItemByIdAsync(int id);
        public Task setOrderAndProductInOrderItem(OrderItem orderItem); 
        public Task<IEnumerable<OrderItem>> getAllOrderItemsAsync
            (PaginationParameters paginationParameters);
        public Task<OrderItem> changeQuantityByOrderItemId(int id,int quantity);

    }
}
