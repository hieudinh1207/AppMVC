
using System.ComponentModel.DataAnnotations;
using MVC_01.Models.Product;

namespace MVC_01.Areas.Product.Models
{
    public class CreateProductModel : ProductModel
    {
        [Display(Name = "Chuyên mục")]
        public int[] CategoryIDs { get; set; }

    }
}