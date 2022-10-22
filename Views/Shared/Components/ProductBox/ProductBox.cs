using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MVC_01.Models;
using MVC_01.Models.Blog;

namespace MVC_01.Components{
    public class ProductBox: ViewComponent
    {
        private readonly ProductService _product;
        public ProductBox(ProductService product)
        {
            _product = product;
        }
        public IViewComponentResult Invoke(bool tangdan = true)
        {
            return View("Default");
            // if(tangdan == true)
            // {
            //     _product.Sort((a,b) => a.Price.CompareTo(b.Price));
            // }
            // else{
            //     _product.Sort((a,b) => b.Price.CompareTo(a.Price));
            // }
            // return View(_product);
        }
    }
}