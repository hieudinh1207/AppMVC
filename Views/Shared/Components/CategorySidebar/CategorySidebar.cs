using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MVC_01.Models.Blog;

namespace MVC_01.Components
{
    [ViewComponent]
    public class CategorySidebar : ViewComponent
    {
        public class CategorySidebarData
        {
            public List<Category> Categories { get; set; }
            public int level { get; set; }
            public string categoryslug { get; set; }
        }
        public IViewComponentResult Invoke(CategorySidebarData data)
        {
            return View(data);
        }
    }
}