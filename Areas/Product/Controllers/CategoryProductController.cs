using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_01.Data;
using MVC_01.Models;
using MVC_01.Models.Product;

namespace MVC_01.Areas.Product
{
    [Area("Product")]
    [Route("admin/categoryproduct/category/{action}/{id?}")]
    [Authorize(Roles = RoleName.Administrator)]
    public class CategoryProductController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Product/Category
        public async Task<IActionResult> Index()
        {
            var qr = (from c in _context.CategoryProducts select c)
            .Include(c => c.ParentCategory)
            .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync()).
            Where(c => c.ParentCategory == null).ToList();
            return View(categories);
        }

        // GET: Product/Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.CategoryProducts
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        private void CreateSelectItems(List<CategoryProduct> source, List<CategoryProduct> des, int level)
        {
            var predix = string.Concat(Enumerable.Repeat("---", level));
            foreach (var category in source)
            {
                //category.Title = predix + category.Title;
                des.Add(new CategoryProduct()
                {
                    Id = category.Id,
                    Title = predix + category.Title
                });
                if (category.CategoryChildren?.Count > 0)
                {
                    CreateSelectItems(category.CategoryChildren.ToList(), des, level + 1);
                }
            }
        }

        // GET: Blog/Category/Create
        public async Task<IActionResult> CreateAsync()
        {
            var qr = (from c in _context.CategoryProducts select c)
          .Include(c => c.ParentCategory)
          .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync()).
            Where(c => c.ParentCategory == null).ToList();
            categories.Insert(0, new CategoryProduct()
            {
                Id = -1,
                Title = "Không có danh mục cha"
            });
            var items = new List<CategoryProduct>();
            CreateSelectItems(categories, items, 0);
            // ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Title");
            var selectList = new SelectList(items, "Id", "Title");
            ViewData["ParentCategoryId"] = selectList;

            return View();
        }

        // POST: Blog/Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Slug,ParentCategoryId")] CategoryProduct category)
        {
            if (ModelState.IsValid)
            {
                if (category.ParentCategoryId == -1) category.ParentCategoryId = null;
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var qr = (from c in _context.CategoryProducts select c)
        .Include(c => c.ParentCategory)
        .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync()).
            Where(c => c.ParentCategory == null).ToList();
            categories.Insert(0, new CategoryProduct()
            {
                Id = -1,
                Title = "Không có danh mục cha"
            });
            // ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Title");
            var selectList = new SelectList(categories, "Id", "Title");
            ViewData["ParentCategoryId"] = selectList;
            return View(category);
        }

        // GET: Blog/Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.CategoryProducts.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var qr = (from c in _context.CategoryProducts select c)
      .Include(c => c.ParentCategory)
      .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync()).
            Where(c => c.ParentCategory == null).ToList();
            categories.Insert(0, new CategoryProduct()
            {
                Id = -1,
                Title = "Không có danh mục cha"
            });
            var items = new List<CategoryProduct>();
            CreateSelectItems(categories, items, 0);
            // ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Title");
            var selectList = new SelectList(items, "Id", "Title");
            ViewData["ParentCategoryId"] = selectList;
            return View(category);
        }

        // POST: Blog/Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Slug,ParentCategoryId")] CategoryProduct category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }
            if (category.ParentCategoryId == category.Id)
            {
                ModelState.AddModelError(string.Empty, "Phải chọn danh mục cha khác.");
            }

            if (ModelState.IsValid && category.ParentCategoryId != category.Id)
            {
                try
                {
                    if (category.ParentCategoryId == -1) category.ParentCategoryId = null;
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            var qr = (from c in _context.CategoryProducts select c)
           .Include(c => c.ParentCategory)
           .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync()).
            Where(c => c.ParentCategory == null).ToList();
            categories.Insert(0, new CategoryProduct()
            {
                Id = -1,
                Title = "Không có danh mục cha"
            });
            // ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Title");
            var selectList = new SelectList(categories, "Id", "Title");
            ViewData["ParentCategoryId"] = selectList;

            return View(category);
        }

        // GET: Blog/Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.CategoryProducts
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Blog/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.CategoryProducts.Include(c => c.CategoryChildren).FirstOrDefaultAsync(c => c.Id == id);
            _context.CategoryProducts.Remove(category);
            if (category == null)
            {
                return NotFound();
            }
            foreach (var cCategory in category.CategoryChildren)
            {
                cCategory.ParentCategoryId = category.ParentCategoryId;
            }
            _context.CategoryProducts.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.CategoryProducts.Any(e => e.Id == id);
        }
    }
}
