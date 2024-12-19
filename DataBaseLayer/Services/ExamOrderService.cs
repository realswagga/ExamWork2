using DataBaseLayer.Classes;
using DataBaseLayer.Data;
using DataBaseLayer.Models;

namespace DataBaseLayer.Services
{
    public class ExamOrderService
    {
        private readonly ExamContext _context = new();

        public int GetLastOrderId()
        {
            int? lastOrderId = _context.ExamOrders.Max(order => (int?)order.OrderId);
            return lastOrderId ?? 0;
        }

        public int GetPickupCode()
        {
            var random = new Random();
            bool isCodeUnique = false;
            while (!isCodeUnique)
            {
                int code = random.Next(10000);
                bool codeExists = _context.ExamOrders.Any(order => order.OrderPickupCode == code);

                if (!codeExists)
                {
                    isCodeUnique = true;
                    return code;
                }
            }

            throw new Exception("Не удалось сгенерировать уникальный код для выдачи товара.");
        }

        public async Task InsertExamOrderAsync(Order order)
        {
            var newOrder = new ExamOrder
            {
                UserId = order.UserID != 0 ? order.UserID : null,
                OrderStatus = order.OrderStatus,
                OrderDate = order.OrderDate,
                OrderDeliveryDate = order.OrderDeliveryDate,
                OrderPickupPoint = order.OrderPickupPoint,
                OrderPickupCode = order.OrderPickupCode
            };

            await _context.ExamOrders.AddAsync(newOrder);
            await _context.SaveChangesAsync();
        }
    }
}
