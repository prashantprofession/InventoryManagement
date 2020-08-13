using Gasware.Models;
using Gasware.ViewModels;
using System.Collections.Generic;

namespace Gasware.Service.Interfaces
{
    public interface IProductService
    {
        ProductViewModel Get(int id);
        List<ProductViewModel> GetProducts();
        void Update(ProductViewModel productViewModel);
        void Add(ProductViewModel productViewModel);
        void Delete(ProductViewModel productViewModel);

        ProductViewModel GetViewModel(ProductModel productModel);
        ProductModel GetDatabaseModel(ProductViewModel productViewModel);
    }
}
