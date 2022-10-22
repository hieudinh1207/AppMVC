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
        private int ITEMS_PER_PAGE = 5;
        public int currentPage { get; set; }
        public int countPages { get; set; }
        public ViewPostController(ILogger<ViewPostController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        // /post/{categoryslug?}
        [Route("post/{categoryslug?}")]
        public async Task<IActionResult> Index(string categoryslug, [FromQuery(Name ="p")]int currentPage)
        {
            var categories = GetCategory();
            ViewBag.categories = categories;
            ViewBag.categoryslug = categoryslug;
            Category category = null;
            if(!string.IsNullOrEmpty(categoryslug))
            {
                category =_context.Categories.Where(c => c.Slug == categoryslug).FirstOrDefault();
                if(category == null)
                {
                    return NotFound("Không thấy chuyên mục bài viết");
                }
            }
            ViewBag.category = category;
            var posts = _context.Posts.Include(p => p.Author)
            .Include(p=>  p.PostCategories)
            .ThenInclude(p => p.Category).AsQueryable().OrderByDescending(p => p.DateUpdated);
   

            //posts = posts.Where(p => p.PostCategories.Select(pc => pc.CategoryID).FirstOrDefault() == category.Id).AsQueryable().ToList();
            //ViewBag.postsInCategory = postsInCategory;
           if(category!=null)
           {
                List<int> ids = new List<int>();
                category.ChildCategoryIDs(null,ids);
                ids.Add(category.Id);
                posts = posts.Where(p => p.PostCategories.Where(p => ids.Contains(p.CategoryID)).Any()).OrderByDescending(p =>p.DateUpdated);
           }
            //ViewBag.posts = posts.ToList();
            int totalPosts =await posts.CountAsync();
            int totalPages = (int) Math.Ceiling((double)totalPosts/ITEMS_PER_PAGE);
            if(currentPage<1)
            {
                currentPage =1;
            }
            if(currentPage>totalPages)
            {
                currentPage =totalPages;
            }
            var postsperPage = posts.Skip((currentPage-1)*ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE);
            var padingModel = new PagingModel()
            {
                currentpage = currentPage,
                countpages = totalPages,
                generateUrl = (pageNumber) => Url.Action("Index",new
                {
                    p = pageNumber
                })
            };
            ViewData["padingmodel"] = padingModel;
            return View(postsperPage.ToList());
        }

        [Route("post/{postslug}.html")]
        public IActionResult Detail(string postslug)
        {
            var categories = GetCategory();
            ViewBag.categories = categories;
            Post post = null;
          
            post = _context.Posts.Where(p => p.Slug == postslug).Include(p => p.Author).Include(p => p.PostCategories).ThenInclude(p => p.Category).FirstOrDefault();
            if(post == null)
            {
                return NotFound("Không tìm thấy bài viết");
            }
            Category category = post.PostCategories.FirstOrDefault()?.Category;
            ViewBag.category = category;
            
            var otherPosts = _context.Posts.Where(p =>p.PostCategories.Any(c => c.CategoryID == category.Id)).OrderByDescending(p => p.DateUpdated).Take(5).ToList();
            ViewBag.otherPosts = otherPosts;
          
            return View(post);
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