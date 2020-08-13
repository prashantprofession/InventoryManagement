using Gasware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gasware.Repository.Interfaces
{
    public interface IProductRepository
    {
        ProductModel Get(int id);
        List<ProductModel> GetProducts();
        int Create(ProductModel productModel);
        void Update(ProductModel productModel);
        void Delete(int id);
    }
}
