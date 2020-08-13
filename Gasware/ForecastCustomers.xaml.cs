
using Gasware.Common;
using Gasware.Models;
using Gasware.ReportModels;
using Gasware.Repository.Interfaces;
using Gasware.Service.Interfaces;
using Gasware.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for ForecastCustomers.xaml
    /// </summary>
    public partial class ForecastCustomers : Window
    {
        private readonly string _username;
        private readonly IBillingService _billingService;
        private readonly IStockEntryRepository _stockEntryRepository;
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
      //  private List<ForecastCustomerModel> _forecastCustomerList = new List<ForecastCustomerModel>();

        public ForecastCustomers(IUnityContainer container, string username)
        {
            InitializeComponent();
            _username = username;
            _billingService = container.Resolve<IBillingService>();
            _stockEntryRepository = container.Resolve<IStockEntryRepository>();
            _productService = container.Resolve<IProductService>();
            _customerService = container.Resolve<ICustomerService>();
            productsCombo.ItemsSource = _productService.GetProducts();
            customersCombo.ItemsSource = _customerService.GetCustomers();
            forecastDataGrid.ItemsSource = GetFullCustomerList();
        }

        private void GetDetailsClicked(object sender, RoutedEventArgs e)
        {
            ProductViewModel productView = (productsCombo.SelectedItem as ProductViewModel);
            CustomerViewModel customerView = (customersCombo.SelectedItem as CustomerViewModel);
            List<ForecastCustomerModel> forecastCustomers = GetFullCustomerList();
            if (productView != null)
            {
                forecastCustomers = forecastCustomers.Where(x => x.ProductId == productView.Productid).ToList();
            }
            if (customerView != null)
            {
                forecastCustomers = forecastCustomers.Where(x => x.CustomerId == 
                                                customerView.CustomerId).ToList();

            }
            forecastDataGrid.ItemsSource = forecastCustomers;
        }

        private List<ForecastCustomerModel> GetFullCustomerList()
        {
            List<BillingViewModel> billingViews = _billingService.GetBillings();
            StockEntryModel latestStock = new StockEntryModel();
            int index = 0;
            List<ForecastCustomerModel> forecastCustomerList = new List<ForecastCustomerModel>();
            foreach (BillingViewModel billing in billingViews)
            {
                ForecastCustomerModel existingForecastCustomer = forecastCustomerList.FirstOrDefault(x => 
                                        x.CustomerId == billing.Customer.CustomerId && 
                                        x.ProductId == billing.Product.Productid);                
                if (existingForecastCustomer == null)
                {
                    latestStock = _stockEntryRepository.GetLatestStockForProduct(billing.Product);
                    forecastCustomerList.Add(CreateNewForecastCustomerModel(billing, latestStock));
                }
                else
                {
                    index = forecastCustomerList.IndexOf(existingForecastCustomer);
                    if (existingForecastCustomer.Quantity < billing.DeliveredFullCylinderQty)
                    {
                        existingForecastCustomer.Quantity = billing.DeliveredFullCylinderQty;                        
                    }                    
                    int dateDiffValue = billing.BillingDate.Subtract(existingForecastCustomer.LastSelectedDate).Days;
                    if (dateDiffValue < existingForecastCustomer.MininumNumberOfDays)
                    {
                        existingForecastCustomer.MininumNumberOfDays = dateDiffValue;                        
                    }
                    existingForecastCustomer.LastSelectedDate = billing.BillingDate;
                    forecastCustomerList[index] = existingForecastCustomer; 
                }
            }

            List<ForecastCustomerModel> tempModels = new List<ForecastCustomerModel>();
            //tempModels = _forecastCustomerList;

            foreach (ForecastCustomerModel forecastCustomer in forecastCustomerList)
            {
                if (forecastCustomer.MininumNumberOfDays == 0 ||
                    forecastCustomer.LastSelectedDate.AddDays(
                    forecastCustomer.MininumNumberOfDays).ToShortDateString() == 
                    DateTime.Now.AddDays(1).ToShortDateString())
                {
                    tempModels.Add(forecastCustomer);
                }                
            }
            return tempModels;
        }

        private ForecastCustomerModel CreateNewForecastCustomerModel(BillingViewModel billing,
                                                                     StockEntryModel latestStock)
        {
            decimal customerRate = GetCustomersDiscountedRate(
                                    latestStock.UnitRate,
                                    billing.Customer.DiscountFlat,
                                    billing.Customer.DiscountPercentage);
                  return
                        new ForecastCustomerModel()
                        {
                            Customer = billing.Customer.Name,
                            CustomerId = billing.Customer.CustomerId,
                            CustomerRate = customerRate,
                            PhoneNumber = billing.Customer.PhoneNumber,
                            UnitRate = latestStock.UnitRate,
                            Product = billing.Product.Name,
                            ProductId = billing.Product.Productid,
                            Quantity = billing.DeliveredFullCylinderQty,
                            TotalCustomerPrice = billing.DeliveredFullCylinderQty * customerRate,
                            TotalLatestPrice = billing.DeliveredFullCylinderQty * latestStock.UnitRate,
                            MininumNumberOfDays = 1
                        };

        }

        private decimal GetCustomersDiscountedRate(decimal latestRate, int flatDiscountAmount, decimal discountPercentage)
        {
            if (flatDiscountAmount != 0)
            {
                return (latestRate - flatDiscountAmount);
            }
            else if (discountPercentage != 0)
            {
                return latestRate - ((discountPercentage / 100) * latestRate);
            }
            return latestRate;
        }

        private void btn_ForecastCancel_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnExportToCsvClicked(object sender, RoutedEventArgs e)
        {
            List<ForecastCustomerModel> forecastCustomers = GetFullCustomerList();
            Gasware.Common.CommonFunctions commonFunctions = new Common.CommonFunctions();
            commonFunctions.ExportToCsvFile(forecastCustomers);
        }

        private void btnPrintGridClicked(object sender, RoutedEventArgs e)
        {
            PrintFunction printFunction = new PrintFunction();
            printFunction.PrintGrid(forecastDataGrid);
        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
