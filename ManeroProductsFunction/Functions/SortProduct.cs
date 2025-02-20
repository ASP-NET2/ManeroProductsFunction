using ManeroProductsFunction.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace ManeroProductsFunction.Functions
{
    public class SortProduct
    {
        private readonly ILogger<SortProduct> _logger;
        private readonly DataContext _context;

        public SortProduct(ILogger<SortProduct> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("SortProduct")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            var category = req.Query["category"].ToString().ToLower();
            var subCategory = req.Query["subCategory"].ToString().ToLower();
            var format = req.Query["format"].ToString().ToLower();
            var title = req.Query["title"].ToString().ToLower();
            var onSaleQuary = req.Query["onSale"].ToString().ToLower();
            var bestSellerQuary = req.Query["bestSeller"].ToString().ToLower();
            var featuredProductQuary = req.Query["featuredProduct"].ToString().ToLower();
            var isFavoriteQuary = req.Query["isFavorite"].ToString().ToLower();
            var minPriceQuery = req.Query["minPrice"].ToString();
            var maxPriceQuery = req.Query["maxPrice"].ToString();
            var ratingNotNullQuery = req.Query["ratingNotNull"].ToString().ToLower();
            var minRatingQuery = req.Query["minRating"].ToString();
            var maxRatingQuery = req.Query["maxRating"].ToString();
            var id = req.Query["id"].ToString().ToLower();

            var products = await _context.Product.ToListAsync();

            if (products == null || products.Count == 0)
            {
                return new NoContentResult();
            }

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.CategoryName!.ToLower() == category).ToList();
            }

            if (!string.IsNullOrEmpty(format))
            {
                products = products.Where(p => p.FormatName!.ToLower() == format).ToList();
            }

            if (!string.IsNullOrEmpty(title))
            {
                products = products.Where(p => p.Title!.ToLower() == title).ToList();
            }
            
            if (!string.IsNullOrEmpty(id))
            {
                products = products.Where(p => p.Id!.ToLower() == id).ToList();
            }

            if (!string.IsNullOrEmpty(subCategory))
            {
                products = products.Where(p => p.SubCategoryName!.ToLower() == subCategory).ToList();
            }

            if (!string.IsNullOrEmpty(onSaleQuary) && bool.TryParse(onSaleQuary, out bool onSale))
            {
                products = products.Where(p => p.OnSale == onSale).ToList();
            }

            

            if (!string.IsNullOrEmpty(bestSellerQuary) && bool.TryParse(bestSellerQuary, out bool bestSeller))
            {
                products = products.Where(p => p.BestSeller == bestSeller).ToList();
            }

            if (!string.IsNullOrEmpty(featuredProductQuary) && bool.TryParse(featuredProductQuary, out bool featuredProduct))
            {
                products = products.Where(p => p.FeaturedProduct == featuredProduct).ToList();
            }

            if (!string.IsNullOrEmpty(isFavoriteQuary) && bool.TryParse(isFavoriteQuary, out bool isFavorite))
            {
                products = products.Where(p => p.IsFavorite == isFavorite).ToList();
            }

            if (decimal.TryParse(minPriceQuery, out decimal minPrice))
            {
                products = products.Where(p => decimal.TryParse(p.Price, out decimal price) && price >= minPrice).ToList();
            }

            if (decimal.TryParse(maxPriceQuery, out decimal maxPrice))
            {
                products = products.Where(p => decimal.TryParse(p.Price, out decimal price) && price <= maxPrice).ToList();
            }

            if (!string.IsNullOrEmpty(ratingNotNullQuery) && bool.TryParse(ratingNotNullQuery, out bool ratingNotNull) && ratingNotNull)
            {
                products = products.Where(p => p.Rating != null).ToList();
            }

            if (decimal.TryParse(minRatingQuery, out decimal minRating))
            {
                products = products.Where(p => decimal.TryParse(p.Rating, out decimal rating) && rating >= minRating).ToList();
            }

            if (decimal.TryParse(maxRatingQuery, out decimal maxRating))
            {
                products = products.Where(p => decimal.TryParse(p.Rating, out decimal rating) && rating <= maxRating).ToList();
            }

            return new OkObjectResult(products);
        }
    }
}
