
using System.ComponentModel.DataAnnotations;
using MVC_01.Models.Blog;

namespace MVC_01.Areas.Blog.Models
{
    public class CreatePostModel : Post
    {
        [Display(Name = "Chuyên mục")]
        public int[] CategoryIDs { get; set; }

    }
}