using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC_01.Models;

namespace MVC_01.Controllers
{
    [Area("ProductManage")]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly ILogger<ProductController> _logger;
        public ProductController(ProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            var products = _productService.OrderBy(p => p.Name).ToList();
            return View(products);
        }

    }
}
//\Areas\ProductManage\Views\Product\Index.cshtml