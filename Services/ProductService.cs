using System.Collections.Generic;

namespace MVC_01.Models
{
    public class ProductService : List<ProductModel>
    {
        public ProductService()
        {
            this.AddRange(new ProductModel[]{
            new ProductModel(){
                Id = 1,Name="Iphone X", Price =1000,
            },
                new ProductModel(){
                Id = 2,Name="SamSung", Price =900,
            },
                new ProductModel(){
                Id = 3,Name="Oppo", Price =800,
            },
                new ProductModel(){
                Id = 3,Name="Nokia", Price =400,
            }
        });
        }
    }
}