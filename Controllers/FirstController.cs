using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC_01.Models;

namespace MVC_01.Controllers
{
    public class FirstController : Controller
    {
        private readonly ILogger<FirstController> _logger;
        private readonly ProductService _productService;
        public FirstController(ILogger<FirstController> logger, ProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }
        public string Index()
        {
            //this.HttpContext
            //this.Request
            // this.Response
            // this.RouteData
            // this.User
            // this.ModelState
            // this.Url
            // this.ViewData
            // this.ViewBag
            // this.TempData
            //LogLevel
            _logger.LogInformation("Index Action");

            return "Toi la index cua First";
        }
        public void Nothing()
        {
            _logger.LogInformation("Nothing action");
            Response.Headers.Add("hi", "Xin chao cac ban");
        }
        public object Anything()
        {
            return new int[] { 1, 2, 3, 4 };
        }
        public IActionResult Readme()
        {
            var content = @"
            in chaof
            ca ban
            
            
            
            
            
            
            
            hi";
            return this.Content(content, "text/html");
        }
        public IActionResult Bird()
        {
            string filePath = Path.Combine(Startup.ContentRootPath, "Files/Bird.jpg");
            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "image/jpg");
        }
        public IActionResult IphonePrice()
        {
            return Json(new
            {
                productname = "Hoa hong",
                Price = 1000
            });

        }
        public IActionResult Privacy()
        {
            var url = "https://google.com";
            return Redirect(url);
        }
        public IActionResult HelloView(string username)
        {
            //View()
            if (string.IsNullOrEmpty(username))
            {
                username = "Khách";
            }
            // return View("xinchao2", username);
            return View("xinchao3", username);
        }
        [TempData]
        public string StatusMessage { set; get; }
        public IActionResult ViewProduct(int? id)
        {
            var product = _productService.Where(p => p.Id == id).FirstOrDefault();
            if (product == null)
            {
                //TempData["StatusMessage"] = "Ban pham ban yeu cau khong co";
                StatusMessage = "Khong co san pham";
                return Redirect(Url.Action("Index", "Home"));
            }

            // this.ViewData["product"] = product;
            // ViewData["Title"] = "Hello";
            // return View("ViewProduct2");
            TempData["StatusMessage"] = "abcdefgh";
            ViewBag.product = product;
            return View("ViewProduct3");
        }

    }
}