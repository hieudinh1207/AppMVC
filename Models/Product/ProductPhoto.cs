using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_01.Models.Product
{
    [Table("ProductPhoto")]
    public class ProductPhoto 
    {
        [Key]
        public int Id{get;set;}
        //abc.pgn, 123.jpg..
        public string FileName{get;set;}
        public int ProductId{get;set;}
        [ForeignKey("ProductId")]
        public ProductModel Product {set;get;}

    }
}