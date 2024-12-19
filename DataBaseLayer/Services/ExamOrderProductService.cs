using DataBaseLayer.Data;
using DataBaseLayer.Models;

namespace DataBaseLayer.Services
{
    public class ExamOrderProductService
    {
        private readonly ExamContext _context = new();

        public async Task InsertExamOrderProductAsync(int orderId, string article, int amount)
        {
            var newProduct = new ExamOrderProduct
            {
                OrderId = orderId,
                ProductArticleNumber = article,
                Amount = (short)amount
            };

            await _context.ExamOrderProducts.AddAsync(newProduct);
            await _context.SaveChangesAsync();
        }
    }
}
