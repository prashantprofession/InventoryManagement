using Gasware.Models;
using Gasware.Repository;
using Gasware.Repository.Interfaces;
using Gasware.Service.Interfaces;
using Gasware.ViewModels;
using System.Collections.Generic;
using Unity;

namespace Gasware.Service
{
    public class ProductServiceProvider : IProductService
    {
        private readonly string _username;
        private IProductRepository _productRepository;

        public ProductServiceProvider(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public ProductServiceProvider(string username)
        {
            _username = username;
        }

        public ProductServiceProvider(IUnityContainer container, string username)
        {
            _username = username;
            _productRepository = container.Resolve<IProductRepository>();
            
        }
        public ProductViewModel Get(int id)
        {
            if (_productRepository == null)
            {
                _productRepository = new ProductRepository("admin");
            }
            ProductModel productModel = _productRepository.Get(id);
            if (productModel == null)
                return null;
            return GetViewModel(productModel);
        }

        public List<ProductViewModel> GetProducts()
        {
            if (_productRepository == null)
            {
                _productRepository = new ProductRepository("admin");
            }
            List<ProductModel> productModels = _productRepository.GetProducts();
            List<ProductViewModel> productViewModels = new List<ProductViewModel>();
            foreach (ProductModel productModel in productModels)
            {
                productViewModels.Add(GetViewModel(productModel));
            }
            return productViewModels;
        }

        public ProductViewModel GetViewModel(ProductModel productModel)
        {
            ProductViewModel viewModel = new ProductViewModel()
            {
                Productid = productModel.Productid,
                Details = productModel.Details,
                Name = productModel.Name,
                Weight = productModel.Weight,
                UnitRate = productModel.UnitRate,
                CGstRate = productModel.CGstRate,
                SGstRate = productModel.SGstRate,
                IsBillable = productModel.IsBillable,
                IsBillableItem = productModel.IsBillable ? "Yes" : "No",                
                IsExpense = productModel.IsExpense,
                IsExpenseItem = productModel.IsExpense ? "Yes" : "No"
            };
            return viewModel;
        }

        public ProductModel GetDatabaseModel(ProductViewModel productViewModel)
        {
            ProductModel productModel = new ProductModel()
            {
                Productid = productViewModel.Productid,
                Details = productViewModel.Details,
                Name = productViewModel.Name,
                Weight = productViewModel.Weight,
                UnitRate = productViewModel.UnitRate,
                CGstRate = productViewModel.CGstRate,
                SGstRate = productViewModel.SGstRate,
                IsBillable = productViewModel.IsBillable,
                IsExpense = productViewModel.IsExpense
            };
            return productModel;
        }


        public void Add(ProductViewModel productViewModel)
        {
            if (_productRepository == null)
            {
                _productRepository = new ProductRepository("admin");
            }
            ProductModel productModel = GetDatabaseModel(productViewModel);
            _productRepository.Create(productModel);
        }

        public void Update(ProductViewModel productViewModel)
        {
            if (_productRepository == null)
            {
                _productRepository = new ProductRepository("admin");
            }
            ProductModel productModel = GetDatabaseModel(productViewModel);
            _productRepository.Update(productModel);
        }

        public void Delete(ProductViewModel productViewModel)
        {
            if (_productRepository == null)
            {
                _productRepository = new ProductRepository("admin");
            }
            _productRepository.Delete(productViewModel.Productid);
        }
    }
}
