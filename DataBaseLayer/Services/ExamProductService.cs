using DataBaseLayer.Data;
using DataBaseLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataBaseLayer.Services
{
    public class ExamProductService
    {
        private readonly ExamContext _context = new();

        public async Task<List<ExamProduct>> GetProductsDataAsync(string searchString, int? sortMethod, decimal minCost,
                                                 decimal? maxCost, string? manufacturer)
        {
            IQueryable<ExamProduct> products = _context.ExamProducts.AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
                products = products.Where(p => EF.Functions.Like(p.ProductName, $"%{searchString}%"));

            products = products.Where(p => p.ProductCost >= minCost);
            if (maxCost.HasValue)
                products = products.Where(p => p.ProductCost <= maxCost);

            if (manufacturer == "System.Windows.Controls.ComboBoxItem: Все производители")
                manufacturer = "";
            if (!string.IsNullOrEmpty(manufacturer))
                products = products.Where(p => p.ProductManufacturer == manufacturer);

            if (sortMethod == 0)
                products = products.OrderBy(p => p.ProductCost);
            else if (sortMethod == 1)
                products = products.OrderByDescending(p => p.ProductCost);

            string query = products.ToQueryString();

            return await products.ToListAsync();
        }

        public async Task<List<string>> GetManufacturersAsync()
        {
            var manufacturers = await _context.ExamProducts
                .Select(p => p.ProductManufacturer)
                .Distinct()
                .Where(m => !string.IsNullOrEmpty(m))
                .OrderBy(m => m)
                .ToListAsync();

            return manufacturers;
        }

        public int GetProductsCount()
            => _context.ExamProducts.Count();
    }
}
