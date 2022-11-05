using System.ComponentModel.DataAnnotations.Schema;
using MVC_01.Models.Product;

namespace MVC_01.Models.Blog
{
    [Table("PostCategory")]
    public class PostCategory
    {
        public int PostID { set; get; }

        public int CategoryID { set; get; }

        [ForeignKey("PostID")]
        public Post Post { set; get; }

        [ForeignKey("CategoryID")]
        public Category Category { set; get; }
        public ProductModel Product { get; internal set; }
    }
}