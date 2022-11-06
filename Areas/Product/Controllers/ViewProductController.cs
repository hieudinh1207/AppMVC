using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MVC_01.Areas.Product.Models;
using MVC_01.Models;
using MVC_01.Models.Product;

namespace MVC_01.Areas.Product
{
    [Area("Product")]

    public class ViewProductController : Controller
    {
        private readonly ILogger<ViewProductController> _logger;
        private readonly AppDbContext _context;
        private readonly CartService _cartService;
        private int ITEMS_PER_PAGE = 6;
        public int currentPage { get; set; }
        public int countPages { get; set; }
        public ViewProductController(ILogger<ViewProductController> logger, AppDbContext context,CartService cartService)
        {
            _logger = logger;
            _context = context;
            _cartService = cartService;
        }
        // /post/{categoryslug?}
        [Route("product/{categoryproductslug?}")]
        public async Task<IActionResult> Index(string categoryproductslug, [FromQuery(Name ="p")]int currentPage)
        {
            var categories = GetCategory();
            ViewBag.categories = categories;
            ViewBag.categoryslug = categoryproductslug;
            CategoryProduct category = null;
            if(!string.IsNullOrEmpty(categoryproductslug))
            {
                category =_context.CategoryProducts.Where(c => c.Slug == categoryproductslug).FirstOrDefault();
                if(category == null)
                {
                    return NotFound("Không thấy chuyên mục bài viết");
                }
            }
            ViewBag.category = category;
            var products = _context.Products.Include(p => p.Author)
            .Include(p =>p.Photos)
            .Include(p=>  p.ProductCategoryProducts)
            .ThenInclude(p => p.Category).AsQueryable().OrderByDescending(p => p.DateUpdated);
   

            //posts = posts.Where(p => p.PostCategories.Select(pc => pc.CategoryID).FirstOrDefault() == category.Id).AsQueryable().ToList();
            //ViewBag.postsInCategory = postsInCategory;
           if(category!=null)
           {
                List<int> ids = new List<int>();
                category.ChildCategoryIDs(null,ids);
                ids.Add(category.Id);
                products = products.Where(p => p.ProductCategoryProducts.Where(p => ids.Contains(p.CategoryID)).Any()).OrderByDescending(p =>p.DateUpdated);
           }
            //ViewBag.posts = posts.ToList();
            int totalProducts =await products.CountAsync();
            int totalPages = (int) Math.Ceiling((double)totalProducts/ITEMS_PER_PAGE);
            if(currentPage<1)
            {
                currentPage =1;
            }
            if(currentPage>totalPages)
            {
                currentPage =totalPages;
            }
            var productsperPage = products.Skip((currentPage-1)*ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE);
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
            return View(productsperPage.ToList());
        }

        [Route("product/{productslug}.html")]
        public IActionResult Detail(string productslug)
        {
            var categories = GetCategory();
            ViewBag.categories = categories;
            ProductModel product = null;
          
            product = _context.Products.Where(p => p.Slug == productslug)
            .Include(p => p.Photos)
            .Include(p => p.Author).Include(p => p.ProductCategoryProducts).ThenInclude(p => p.Category).FirstOrDefault();
            if(product == null)
            {
                return NotFound("Không tìm thấy bài viết");
            }
            CategoryProduct category = product.ProductCategoryProducts.FirstOrDefault()?.Category;
            ViewBag.category = category;
            
            var otherProducts = _context.Products.Where(p =>p.ProductCategoryProducts.Any(c => c.CategoryID == category.Id)).OrderByDescending(p => p.DateUpdated).Take(5).ToList();
            ViewBag.otherProducts = otherProducts;
          
            return View(product);
        }
        private List<CategoryProduct> GetCategory()
        {
            var categories = _context.CategoryProducts
                                        .Include(c => c.CategoryChildren).AsEnumerable()
                                        .Where(c => c.ParentCategory == null).ToList();
            return categories;

        }
        [Route ("addcart/{productid:int}", Name = "addcart")]
        public IActionResult AddToCart ([FromRoute] int productid) {

            var product = _context.Products
                .Where (p => p.ProductId == productid)
                .FirstOrDefault ();
            if (product == null)
                return NotFound ("Không có sản phẩm");

            // Xử lý đưa vào Cart ...
            var cart = _cartService.GetCartItems ();
            var cartitem = cart.Find (p => p.product.ProductId == productid);
            if (cartitem != null) {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity++;
            } else {
                //  Thêm mới
                cart.Add (new CartItem () { quantity = 1, product = product });
            }

            // Lưu cart vào Session
            _cartService.SaveCartSession (cart);
            // Chuyển đến trang hiện thị Cart
            return RedirectToAction (nameof (Cart));
        }
        // Hiện thị giỏ hàng
        [Route ("/cart", Name = "cart")]
        public IActionResult Cart () {
            return View (_cartService.GetCartItems());
        }
        /// xóa item trong cart
        [Route ("/removecart/{productid:int}", Name = "removecart")]
        public IActionResult RemoveCart ([FromRoute] int productid) {
            var cart = _cartService.GetCartItems ();
            var cartitem = cart.Find (p => p.product.ProductId == productid);
            if (cartitem != null) {
                // Đã tồn tại, tăng thêm 1
                cart.Remove(cartitem);
            }

            _cartService.SaveCartSession (cart);
            return RedirectToAction (nameof (Cart));
        }
        /// Cập nhật
        [Route ("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart ([FromForm] int productid, [FromForm] int quantity) {
            // Cập nhật Cart thay đổi số lượng quantity ...
            var cart = _cartService.GetCartItems ();
            var cartitem = cart.Find (p => p.product.ProductId == productid);
            if (cartitem != null) {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity = quantity;
            }
            _cartService.SaveCartSession (cart);
            // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
            return Ok();
        }
        public string Checkout()
        {
            return "Đơn hàng được gửi thành công";
        }
    }
}