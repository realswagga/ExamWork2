using DataBaseLayer.Data;

namespace DataBaseLayer.Services
{
    public class ExamPickupPointService
    {
        private readonly ExamContext _context = new();

        public int GetPickupPoint()
        {
            var randomPickupPoint = _context.ExamPickupPoints
                .OrderBy(p => Guid.NewGuid())
                .Select(p => p.OrderPickupPoint)
                .FirstOrDefault();

            if (randomPickupPoint == null)
                throw new Exception("Пункты выдачи отсутствуют в базе данных.");

            return randomPickupPoint;
        }
    }
}
