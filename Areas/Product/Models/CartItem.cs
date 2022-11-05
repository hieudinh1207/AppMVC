

using MVC_01.Models.Product;

namespace MVC_01.Areas.Product.Models{
    public class CartItem
    {
        public int quantity {set; get;}
        public ProductModel product {set; get;}
    }
}
