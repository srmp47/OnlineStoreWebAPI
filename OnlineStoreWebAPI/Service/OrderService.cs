using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.DBContext;
using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;
using System.Text.RegularExpressions;

namespace OnlineStoreWebAPI.Repository
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository orderRepository;
        private readonly IMapper mapper;
        public OrderService(IOrderRepository orderRepository, IMapper inputMapper)
        {
            this.orderRepository = orderRepository;
            this.mapper = inputMapper;
        }
        public async Task cancelOrderByIdAsync(int id)
        {
            var order = await orderRepository.getOrderByIdAsync(id);
            order.status = OrderStatus.Cancelled;
            await orderRepository.updateOrder(order);

        }

        public async Task<Order> createNewOrderAsync(Order order)
        {
            // TODO you can handle price by data loader ... ?!
            foreach(var orderItem in order.orderItems)
            {
                order.totalAmount += orderItem.price;
            }
            await orderRepository.createNewOrderAsync(order);
            return order;
        }

        public async Task<IEnumerable<Order>> getAllOrdersAsync(PaginationParameters paginationParameters)
        {
            var orders = await orderRepository.getAllOrdersAsync(paginationParameters);
            return orders;
        }

        public async Task<IEnumerable<Order>> getAllOrdersOfUserByIdAsync
            (int userId, PaginationParameters paginationParameters)
        {
            var orders = await orderRepository.getAllOrdersOfUserByIdAsync(userId, paginationParameters);
            return orders;
        }

        public async Task<Order?> getOrderByOrderIdAsync(int orderId)
        {
            var order = await orderRepository.getOrderByIdAsync(orderId);
            return order;
        }



        public async Task<bool> isThereOrderByIdAsync(int id)
        {
            var isThere = await orderRepository.isThereOrderWithIdAsync(id);
            return isThere;
        }

        public async Task updateOrderAsync(Order order)
        {
            await orderRepository.updateOrder(order);
        }
        public async Task<bool> deleteOrderByIdAsync(int id)
        {
            var deletedSuccessfully = await orderRepository.deleteOrderWithIdAsync(id);
            return deletedSuccessfully;
        }
        //in this function , I set User in Order by user id
        // TODO correct this function and it's usage (in controller)
        public async Task setUserInOrder(Order order, int userId)
        {
            await orderRepository.setUserInOrder(order, userId);
        }
        public async Task setOrderAndProductInOrderItem(OrderItem orderItem)
        {
            await orderRepository.setOrderAndProductInOrderItem(orderItem);

        }



        public async Task<Order> changeOrderStatusByOrderIdAsync(int id, OrderStatus status)
        {
            var order = await orderRepository.getOrderByIdAsync(id);
            order.status = status;
            await orderRepository.updateOrder(order);
            return order;
        }

        public async Task<int> getUserIdOfOrder(int orderId)
        {
            var userId = await orderRepository.getUserIdOfOrder(orderId);
            return userId;
        }

        public void setPricesOfOrderItems(Order Order)
        {
            foreach(var item in Order.orderItems)
            {
                item.price = item.quantity * item.Product.price;
            }
        }
    }
}
