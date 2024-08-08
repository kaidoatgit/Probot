using ProPayments.Client.Clients.ProPayments;
using ProPayments.Client.Dtos.Order.Request;
using ProPayments.Client.Exceptions;
using ProPayments.Client.Mappers;
using ProPayments.Client.Models;
using ProPayments.Client.Models.Enums;
using System.Collections.Concurrent;

namespace ProPayments.Client.Services.Managers
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

        public async Task<Order> CreateOrderAsync(ulong userId, Plan plan, int period)
        {
            int planOptionId = plan.GetPlanOptionId(period);
            var orderRequest = new OrderRequest
            {
                UserId = userId,
                PlanOptionId = planOptionId
            };
            var apiResponse = await _orderClient.RegisterOrderAsync(orderRequest);
            if (apiResponse.Data == null || apiResponse.Data.Invoice == null)
            {
                throw new OrderException(apiResponse.StatusCode, string.Empty);
            }
            else
            {
                return _mapper.MapToOrder(apiResponse.Data);
            }
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

        //public IEnumerable<Order> GetOrdersByType(OrderStatus orderType)
        //{
        //    return _orders.Values.Where(o => o.Status == orderType);
        //}        
    }
}
