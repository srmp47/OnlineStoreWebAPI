using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;

namespace OnlineStoreWebAPI.Repository
{
    public interface IOrderItemService
    {
        public Task<OrderItem?> getOrderItemByOrderItemId(int orderItemId);
        public Task<bool> isThereOrderItemById(int id);
        public Task<OrderItem> createNewOrderItemAsync(OrderItem orderItem);
        public Task<OrderItem> updateOrderItemAsync(OrderItem orderItem);
        public Task<bool> deleteOrderItemByIdAsync(int id);
        public Task setOrderAndProductInOrderItem(OrderItem orderItem); 
        public Task<IEnumerable<OrderItem>> getAllOrderItemsAsync
            (PaginationParameters paginationParameters);
        public Task<OrderItem> changeQuantityByOrderItemId(int id,int quantity);

        public  Task<IEnumerable<OrderItem>> getAllOrderItemsByOrderIdAsync(int orderId,PaginationParameters paginationParameters);

    }
}
