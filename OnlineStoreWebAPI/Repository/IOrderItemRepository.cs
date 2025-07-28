using Microsoft.AspNetCore.Mvc;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;

namespace OnlineStoreWebAPI.Repository
{
    public interface IOrderItemRepository
    {
        Task<IEnumerable<OrderItem>> getAllOrderItemsAsync(PaginationParameters paginationParameters);
        Task createNewOrderItemAsync(OrderItem orderItem);
        Task<bool> deleteOrderItemByIdAsync(int id);
        Task<OrderItem?> getOrderItemByIdAsync(int id);
        Task<bool> isThereOrderItemWithIdAsync(int id);
        Task<OrderItem> updateOrderItemAsync(OrderItem orderItem);
        Task<IEnumerable<OrderItem>> getAllOrderItemsByOrderIdAsync(int orderId, PaginationParameters paginationParameters);
        public Task setOrderAndProductInOrderItem(OrderItem orderItem);

    }
}
