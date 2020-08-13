using Gasware.Common;
using Gasware.Models;
using Gasware.Repository;
using Gasware.Service.Interfaces;
using Gasware.ViewModels;
using System.Collections.Generic;
using System.Windows;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for Products.xaml
    /// </summary>
    public partial class Products : Window
    {
        public Products()
        {
            InitializeComponent();
        }

        private string _username;
        private readonly IUnityContainer _container;
        private readonly IProductService _productService;

        public Products(IUnityContainer container, string username)
        {
            InitializeComponent();
            _username = username;
            _container = container;
            _productService = _container.Resolve<IProductService>();
        }



        private void btnNewInsertProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductsInsert productsInsert = new ProductsInsert(_container, _username);
            productsInsert.ShowDialog();
            productsDataGrid.ItemsSource =  _productService.GetProducts();            
        }

        private void btnProductUpdate_Clicked(object sender, RoutedEventArgs e)
        {
            ProductViewModel productViewModel = (productsDataGrid.SelectedItem as ProductViewModel);
            ProductsUpdate productsUpdate = new ProductsUpdate(_container, productViewModel, _username);
            productsUpdate.Title = "Updating Product :" + productViewModel.Productid;
            productsUpdate.AssignInterfaceValues();
            productsUpdate.ShowDialog();            
            productsDataGrid.ItemsSource= _productService.GetProducts();
        }

        private void btnProductDelete_Clicked(object sender, RoutedEventArgs e)
        {
            ProductViewModel productViewModel = (productsDataGrid.SelectedItem as ProductViewModel);
            var result = MessageBox.Show("Are you sure you want to delete the Product with id " +
              productViewModel.Productid + "?",
               "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result.ToString() == "Yes")
            {
                _productService.Delete(productViewModel);
                productsDataGrid.ItemsSource = _productService.GetProducts();
                MessageBox.Show("Product deleted successfully.", "Success",
                   MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnExportToCsvClicked(object sender, RoutedEventArgs e)
        {
            List<ProductViewModel> productViewModels = _productService.GetProducts();
            Gasware.Common.CommonFunctions commonFunctions = new Common.CommonFunctions();
            commonFunctions.ExportToCsvFile(productViewModels);
        }

        private void btnPrintGridClicked(object sender, RoutedEventArgs e)
        {
            PrintFunction printFunction = new PrintFunction();
            printFunction.PrintGrid(productsDataGrid);
        }
     


        private void btnCloseInsertProduct_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }        
    }
}
