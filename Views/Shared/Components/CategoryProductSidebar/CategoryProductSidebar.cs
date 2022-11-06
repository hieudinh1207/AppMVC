using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MVC_01.Models.Blog;
using MVC_01.Models.Product;

namespace MVC_01.Components
{
    [ViewComponent]
    public class CategoryProductSidebar : ViewComponent
    {
        public class CategoryProductSidebarData
        {
            public List<CategoryProduct> Categories { get; set; }
            public int level { get; set; }
            public string categoryslug { get; set; }
        }
        public IViewComponentResult Invoke(CategoryProductSidebarData data)
        {
            return View(data);
        }
    }
}