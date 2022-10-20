using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_01.Areas.Blog.Models;
using MVC_01.Data;
using MVC_01.Models;
using MVC_01.Models.Blog;
using MVC_01.Utilities;

namespace MVC_01.Areas.Blog
{
    [Area("Blog")]
    [Route("admin/blog/post/[action]/{id?}")]
    [Authorize(Roles = RoleName.Administrator + "," + RoleName.Editor)]
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private int ITEMS_PER_PAGE = 10;
        public int currentPage { get; set; }
        public int countPages { get; set; }
        private readonly UserManager<AppUser> _userManager;

        public PostController(AppDbContext context, UserManager<AppUser> userManager)
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
            var posts = _context.Posts.Include(p => p.Author).OrderByDescending(p => p.DateUpdated);
            int totalPosts = await posts.CountAsync();
            countPages = (int)Math.Ceiling((double)totalPosts / ITEMS_PER_PAGE);
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
            ViewBag.totalPosts = totalPosts;
            ViewBag.PostIndex = (currentPage - 1) * pagesize;
            var postInPages = posts.Skip((currentPage - 1) * ITEMS_PER_PAGE).
            Take(ITEMS_PER_PAGE).Include(p => p.PostCategories).ThenInclude(pc => pc.Category);
            return View(await postInPages.ToListAsync());
        }

        // GET: Blog/Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Blog/Post/Create
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");
            //ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Blog/Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,Content,Published,CategoryIDs")] CreatePostModel post)
        {
            if (_context.Posts.Any(p => p.Slug == post.Slug))
            {
                ModelState.AddModelError("Slug", "Nhập chuỗi Url khác");
                return View(post);
            }
            if (post.Slug == null)
            {
                post.Slug = AppUtilities.GenerateSlug(post.Title);
            }
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(this.User);
                post.DateCreated = post.DateUpdated = DateTime.Now;
                post.AuthorId = user.Id;
                if (post.CategoryIDs != null)
                {
                    foreach (var CateId in post.CategoryIDs)
                    {
                        _context.Add(new PostCategory()
                        {
                            CategoryID = CateId,
                            Post = post
                        });
                    }
                }
                _context.Add(post);
                await _context.SaveChangesAsync();
                StatusMessage = "Vừa tạo bài viết mới";
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", post.AuthorId);
            return View(post);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // var post = await _context.Posts.FindAsync(id);
            var post = await _context.Posts.Include(p => p.PostCategories).FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            var postEdit = new CreatePostModel()
            {
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                Description = post.Description,
                Slug = post.Slug,
                Published = post.Published,
                CategoryIDs = post.PostCategories.Select(pc => pc.CategoryID).ToArray()
            };

            var categories = await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");

            return View(postEdit);
        }

        // GET: Blog/Post/Edit/5
        // public async Task<IActionResult> Edit(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }

        //     // var post = await _context.Posts.FindAsync(id);
        //     var post = await _context.Posts.Include(p => p.PostCategories).FirstOrDefaultAsync(p => p.PostId == id);
        //     if (post == null)
        //     {
        //         return NotFound();
        //     }
        //     var postEdit = new CreatePostModel()
        //     {
        //         PostId = post.PostId,
        //         Title = post.Title,
        //         Content = post.Content,
        //         Description = post.Description,
        //         Slug = post.Slug,
        //         Published = post.Published,
        //         CategoryIDs = post.PostCategories.Select(pc => pc.CategoryID).ToArray()
        //     };
        //     var categories = await _context.Categories.ToListAsync();
        //     ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");
        //     return View(post);
        // }

        // POST: Blog/Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Description,Slug,Content,Published,AuthorId,CategoryIDs")] CreatePostModel post)
        // {
        //     if (id != post.PostId)
        //     {
        //         return NotFound();
        //     }
        //     var categories = await _context.Categories.ToListAsync();
        //     ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");
        //     if (post.Slug == null)
        //     {
        //         post.Slug = AppUtilities.GenerateSlug(post.Title);
        //     }
        //     if (_context.Posts.Any(p => p.Slug == post.Slug && p.PostId != id))
        //     {
        //         ModelState.AddModelError("Slug", "Nhập chuỗi Url khác");
        //         return View(post);
        //     }

        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             var postUpdate = await _context.Posts.Include(p => p.PostCategories).FirstOrDefaultAsync(p => p.PostId == id);
        //             if (postUpdate == null)
        //             {
        //                 return NotFound();
        //             }
        //             postUpdate.Title = post.Title;
        //             postUpdate.Description = post.Description;
        //             postUpdate.Content = post.Content;
        //             postUpdate.Published = post.Published;
        //             postUpdate.Slug = post.Slug;
        //             postUpdate.DateUpdated = post.DateUpdated;

        //             //Update PostCategory
        //             if (post.CategoryIDs == null) post.CategoryIDs = new int[] { };
        //             var oldCateIds = postUpdate.PostCategories.Select(c => c.CategoryID).ToArray();
        //             var newCateIds = post.CategoryIDs;
        //             var removeCatePosts = from postCate in postUpdate.PostCategories
        //                                   where (!newCateIds.Contains(postCate.CategoryID))
        //                                   select postCate;
        //             _context.PostCategories.RemoveRange(removeCatePosts);
        //             var addCateIds = from CateId in newCateIds
        //                              where !oldCateIds.Contains(CateId)
        //                              select CateId;
        //             foreach (var CateId in addCateIds)
        //             {
        //                 _context.PostCategories.Add(new PostCategory()
        //                 {
        //                     PostID = id,
        //                     CategoryID = CateId
        //                 });
        //             }
        //             _context.Update(post);
        //             await _context.SaveChangesAsync();
        //         }
        //         catch (DbUpdateConcurrencyException)
        //         {
        //             if (!PostExists(post.PostId))
        //             {
        //                 return NotFound();
        //             }
        //             else
        //             {
        //                 throw;
        //             }
        //         }
        //         StatusMessage = "Vừa cập nhật bài viết";
        //         return RedirectToAction(nameof(Index));
        //     }
        //     ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", post.AuthorId);
        //     return View(post);
        // }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Description,Slug,Content,Published,CategoryIDs")] CreatePostModel post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }
            var categories = await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");


            if (post.Slug == null)
            {
                post.Slug = AppUtilities.GenerateSlug(post.Title);
            }

            if (await _context.Posts.AnyAsync(p => p.Slug == post.Slug && p.PostId != id))
            {
                ModelState.AddModelError("Slug", "Nhập chuỗi Url khác");
                return View(post);
            }


            if (ModelState.IsValid)
            {
                try
                {

                    var postUpdate = await _context.Posts.Include(p => p.PostCategories).FirstOrDefaultAsync(p => p.PostId == id);
                    if (postUpdate == null)
                    {
                        return NotFound();
                    }

                    postUpdate.Title = post.Title;
                    postUpdate.Description = post.Description;
                    postUpdate.Content = post.Content;
                    postUpdate.Published = post.Published;
                    postUpdate.Slug = post.Slug;
                    postUpdate.DateUpdated = DateTime.Now;

                    // Update PostCategory
                    if (post.CategoryIDs == null) post.CategoryIDs = new int[] { };

                    var oldCateIds = postUpdate.PostCategories.Select(c => c.CategoryID).ToArray();
                    var newCateIds = post.CategoryIDs;

                    var removeCatePosts = from postCate in postUpdate.PostCategories
                                          where (!newCateIds.Contains(postCate.CategoryID))
                                          select postCate;
                    _context.PostCategories.RemoveRange(removeCatePosts);

                    var addCateIds = from CateId in newCateIds
                                     where !oldCateIds.Contains(CateId)
                                     select CateId;

                    foreach (var CateId in addCateIds)
                    {
                        _context.PostCategories.Add(new PostCategory()
                        {
                            PostID = id,
                            CategoryID = CateId
                        });
                    }

                    _context.Update(postUpdate);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
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
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", post.AuthorId);
            return View(post);
        }

        // GET: Blog/Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }
        [TempData]
        public string StatusMessage { get; set; }
        // POST: Blog/Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            StatusMessage = "Bạn vừa xóa bài viết " + post.Title;
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
