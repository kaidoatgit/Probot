using Probot.Client.Clients.SubscriptionApi;
using Probot.Client.Mappers;
using Probot.Client.Models;
using Probot.Shared.Dtos.Order.Request;
using System.Collections.Concurrent;

namespace Probot.Client.Managers
{
    public class OrderManager
    {
        private readonly ConcurrentDictionary<ulong, Order> _orders = new();
        private readonly OrderClient _orderClient;
        private readonly Mapper _mapper;

        public OrderManager(OrderClient orderClient, Mapper mapper)
        {
            _orderClient = orderClient;
            _mapper = mapper;

            Console.WriteLine("Order Manager created");
        }

        public async Task<Order?> CreateOrderAsync(ulong userId, Cart? cart)
        {
            var orderRequest = new OrderRequest
            {
                UserId = userId,
                ProductOptionsId = cart!.CartItems.Select(ci => ci.ProductOptionId).ToList()
            };
            var apiResponse = await _orderClient.CreateOrderAsync(orderRequest);
            if (apiResponse.Data != null && apiResponse.Data.Invoice != null)
            {
                return _mapper.MapToOrder(apiResponse.Data);
            }
            return null;
        }

        public void AddOrder(Order order)
        {
            _orders.TryAdd(order.Id, order);
            Console.WriteLine($"New order added: {order.Id}");
        }

        public void RemoveOrder(Order order)
        {
            _orders.TryRemove(order.Id, out _);
        }

        public Order? GetOrder(ulong orderId)
        {
            _orders.TryGetValue(orderId, out var order);
            return order;
        }

        public IEnumerable<Order> Orders => _orders.Values.AsEnumerable();      
    }
}
