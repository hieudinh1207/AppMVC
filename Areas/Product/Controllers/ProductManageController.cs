using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_01.Areas.Blog.Models;
using MVC_01.Areas.Product.Models;
using MVC_01.Data;
using MVC_01.Models;
using MVC_01.Models.Blog;
using MVC_01.Models.Product;
using MVC_01.Utilities;

namespace MVC_01.Areas.Product
{
    [Area("Product")]
    [Route("admin/productmanage/[action]/{id?}")]
    [Authorize(Roles = RoleName.Administrator + "," + RoleName.Editor)]
    public class ProductManageController : Controller
    {
        private readonly AppDbContext _context;
        private int ITEMS_PER_PAGE = 10;
        public int currentPage { get; set; }
        public int countPages { get; set; }
        private readonly UserManager<AppUser> _userManager;

        public ProductManageController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Blog/Post
        public async Task<IActionResult> Index([FromQuery(Name = "p")] int currentPage, int pagesize)
        {
            //var appDbContext = _context.Posts.Include(p => p.Author);
            //return View(await appDbContext.ToListAsync());
            pagesize = 10;
            var products = _context.Products.Include(p => p.Author).OrderByDescending(p => p.DateUpdated);
            int totalProducts = await products.CountAsync();
            countPages = (int)Math.Ceiling((double)totalProducts / ITEMS_PER_PAGE);
            if (currentPage < 1)
            {
                currentPage = 1;
            }
            if (currentPage > countPages)
            {
                currentPage = countPages;
            }
            var pagingmodel = new PagingModel()
            {
                currentpage = currentPage,
                countpages = countPages,
                generateUrl = (pageNumber) => Url.Action("Index", new
                {
                    p = pageNumber,
                    pagesize = pagesize

                })
            };
            ViewBag.pagingmodel = pagingmodel;
            ViewBag.totalroducts = totalProducts;
            ViewBag.PostIndex = (currentPage - 1) * pagesize;
            var postInPages = products.Skip((currentPage - 1) * ITEMS_PER_PAGE).
            Take(ITEMS_PER_PAGE).Include(p => p.ProductCategoryProducts).ThenInclude(pc => pc.Category);
            return View(await postInPages.ToListAsync());
        }

        // GET: Blog/Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Blog/Post/Create
        public async Task<IActionResult> Create()
        {
            var categoryproducts = await _context.CategoryProducts.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categoryproducts, "Id", "Title");
            //ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Blog/Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,Content,Published,CategoryIDs,Price")] CreateProductModel product)
        {
            if (_context.Posts.Any(p => p.Slug == product.Slug))
            {
                ModelState.AddModelError("Slug", "Nhập chuỗi Url khác");
                return View(product);
            }
            if (product.Slug == null)
            {
                product.Slug = AppUtilities.GenerateSlug(product.Title);
            }
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(this.User);
                product.DateCreated = product.DateUpdated = DateTime.Now;
                product.AuthorId = user.Id;
                if (product.CategoryIDs != null)
                {
                    foreach (var CateId in product.CategoryIDs)
                    {
                        _context.Add(new ProductCategoryProduct()
                        {
                            CategoryID = CateId,
                            Product = product
                        });
                    }
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                StatusMessage = "Vừa tạo bài viết mới";
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", product.AuthorId);
            return View(product);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.Include(p => p.ProductCategoryProducts).FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            var productEdit = new CreateProductModel()
            {
                ProductId =product.ProductId,
                Title = product.Title,
                Content = product.Content,
                Description = product.Description,
                Slug = product.Slug,
                Published = product.Published,
                CategoryIDs = product.ProductCategoryProducts.Select(pc => pc.CategoryID).ToArray(),
                Price = product.Price
            };

            var categoryProducts = await _context.CategoryProducts.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categoryProducts, "Id", "Title");

            return View(productEdit);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Title,Description,Slug,Content,Published,CategoryIDs,Price")] CreateProductModel product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }
            var categories = await _context.CategoryProducts.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");


            if (product.Slug == null)
            {
                product.Slug = AppUtilities.GenerateSlug(product.Title);
            }

            if (await _context.Products.AnyAsync(p => p.Slug == product.Slug && p.ProductId != id))
            {
                ModelState.AddModelError("Slug", "Nhập chuỗi Url khác");
                return View(product);
            }


            if (ModelState.IsValid)
            {
                try
                {

                    var productUpdate = await _context.Products.Include(p => p.ProductCategoryProducts).FirstOrDefaultAsync(p => p.ProductId == id);
                    if (productUpdate == null)
                    {
                        return NotFound();
                    }

                    productUpdate.Title = product.Title;
                    productUpdate.Description = product.Description;
                    productUpdate.Content = product.Content;
                    productUpdate.Published = product.Published;
                    productUpdate.Price = product.Price;
                    productUpdate.Slug = product.Slug;
                    productUpdate.DateUpdated = DateTime.Now;

                    // Update ProductCategory
                    if (product.CategoryIDs == null) product.CategoryIDs = new int[] { };

                    var oldCateIds = productUpdate.ProductCategoryProducts.Select(c => c.CategoryID).ToArray();
                    var newCateIds = product.CategoryIDs;

                    var removeCateProducts = from productCate in productUpdate.ProductCategoryProducts
                                          where (!newCateIds.Contains(productCate.CategoryID))
                                          select productCate;
                    _context.ProductCategoryProducts.RemoveRange(removeCateProducts);

                    var addCateIds = from CateId in newCateIds
                                     where !oldCateIds.Contains(CateId)
                                     select CateId;

                    foreach (var CateId in addCateIds)
                    {
                        _context.ProductCategoryProducts.Add(new ProductCategoryProduct()
                        {
                            ProductID = id,
                            CategoryID = CateId
                        });
                    }

                    _context.Update(productUpdate);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                StatusMessage = "Vừa cập nhật bài viết";
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", product.AuthorId);
            return View(product);
        }

        // GET: Blog/Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        [TempData]
        public string StatusMessage { get; set; }
        // POST: Blog/Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(product);
            await _context.SaveChangesAsync();
            StatusMessage = "Bạn vừa xóa bài viết " + product.Title;
            return RedirectToAction(nameof(Index));
        }
 
        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
        public class UploadOneFile{
            [Required(ErrorMessage ="Phải chọn 1 file để upload")]
            [DataType(DataType.Upload)]
            [FileExtensions(Extensions ="png,jpg,jpeg,gif")]
            [Display(Name ="Chọn file upload")]
            public IFormFile FileUpload{get;set;}
        }
        [HttpGet]
        public IActionResult UploadPhoto(int id)
        {
            var product = _context.Products.Where(p =>p.ProductId ==id).Include(p=>p.Photos).FirstOrDefault();
            if(product == null)
            {
                return NotFound("Không có sản phẩm");
            }
            ViewData["product"] = product;
            return View(new UploadOneFile());
        }

        [HttpPost,ActionName("UploadPhoto")]
        public async Task<IActionResult> UploadPhotoAsync(int id,[Bind("FileUpload")] UploadOneFile f)
        {
            var product = _context.Products.Where(e =>e.ProductId ==id).Include(p => p.Photos).FirstOrDefault();
            if(product == null)
            {
                return NotFound("Không có sản phẩm");
            }
            ViewData["product"] = product;
            if(f !=null)
            {
                var file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                            + Path.GetExtension(f.FileUpload.FileName);
                var file = Path.Combine("Uploads","Products",file1);
                using(var filestream = new FileStream(file,FileMode.Create))
                {
                    await f.FileUpload.CopyToAsync(filestream);
                }
                _context.Add(new ProductPhoto(){
                    ProductId = product.ProductId,
                    FileName = file1
                });
                await _context.SaveChangesAsync();
            }
            return View(f);
        }
        [HttpPost]
        public IActionResult ListPhotos(int id)
        {
            var product = _context.Products.Where(p =>p.ProductId ==id).Include(p=>p.Photos).FirstOrDefault();
            if(product == null)
            {
                return Json(
                    new {
                        success =0,
                        message="Product not found"
                    }
                );    
            }
            var listphotos = product.Photos.Select(photo => new{
                id = photo.Id,
                path = "/contents/Products/" + photo.FileName
            });
            return Json(
                new{
                    success =1,
                    photos = listphotos
                }
            );
        }
        [HttpPost]
        public IActionResult DeletePhoto(int id)
        {
            var photo = _context.ProductPhotos.Where(p => p.Id == id).FirstOrDefault();
            if(photo != null)
            {
                _context.Remove(photo);
                _context.SaveChanges();
                var fileName= "Uploads/Products/" + photo.FileName;
                System.IO.File.Delete(fileName);
            }
            return Ok();
        }
         [HttpPost]
        public async Task<IActionResult> UploadPhotoApi(int id,[Bind("FileUpload")] UploadOneFile f)
        {
            var product = _context.Products.Where(e =>e.ProductId ==id).Include(p => p.Photos).FirstOrDefault();
            if(product == null)
            {
                return NotFound("Không có sản phẩm");
            }
            if(f !=null)
            {
                var file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                            + Path.GetExtension(f.FileUpload.FileName);
                var file = Path.Combine("Uploads","Products",file1);
                using(var filestream = new FileStream(file,FileMode.Create))
                {
                    await f.FileUpload.CopyToAsync(filestream);
                }
                _context.Add(new ProductPhoto(){
                    ProductId = product.ProductId,
                    FileName = file1
                });
                await _context.SaveChangesAsync();
            }
            return Ok();
        }
    }
}
