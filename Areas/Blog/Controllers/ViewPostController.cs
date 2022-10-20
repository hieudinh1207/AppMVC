using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MVC_01.Models;
using MVC_01.Models.Blog;

namespace MVC_01.Areas.Blog
{
    [Area("Blog")]

    public class ViewPostController : Controller
    {
        private readonly ILogger<ViewPostController> _logger;
        private readonly AppDbContext _context;

        public ViewPostController(ILogger<ViewPostController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        // /post/{categoryslug?}
        [Route("post/{categoryslug?}")]
        public IActionResult Index(string categoryslug, int page)
        {
            // return Content(categoryslug);
            var categories = GetCategory();
            ViewBag.categories = categories;
            ViewBag.categoryslug = categoryslug;
            return View();
        }
        [Route("post/{postslug}.html")]
        public IActionResult Detail(string postslug)
        {
            return Content(postslug);
        }
        private List<Category> GetCategory()
        {
            var categories = _context.Categories
                                        .Include(c => c.CategoryChildren).AsEnumerable()
                                        .Where(c => c.ParentCategory == null).ToList();
            return categories;

        }
    }
}