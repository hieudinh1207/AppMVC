using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_01.Models.Blog
{
    public class PostPhoto
    {
        [Key]
        public int id{get;set;}
        public string FileName {get;set;}
        public int PostId{get;set;}
        [ForeignKey("PostId")]
        public Post Post{get;set;}
    }
}