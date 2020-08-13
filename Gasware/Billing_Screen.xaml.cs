using Gasware.Billing;
using Gasware.Common;
using Gasware.Database;
using Gasware.Models;
using Gasware.ReportModels;
using Gasware.Repository;
using Gasware.Repository.Interfaces;
using Gasware.Service.Interfaces;
using Gasware.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for Billing_Screen.xaml
    /// </summary>
    public partial class Billing_Screen : Window
    {
        
        public Billing_Screen()
        {
            InitializeComponent();

        }
        private readonly string _username;
        private readonly DateTime _selectedDate;
        private readonly IUnityContainer _container;
        private readonly IProductService _productService;
        private readonly IDeliveryPersonService _deliveryPersonService;
        private readonly ICustomerService _customerService;
        private readonly IBillingService _billingService;
        private readonly IStockEntryRepository _stockEntryRepository;
        private TransactionsFilterInputModel filterModel;

        public Billing_Screen(IUnityContainer container, string username)
        {
            InitializeComponent();
            _username = username;
            _selectedDate = DateTime.Now;
            _container = container;
            _productService = _container.Resolve<IProductService>();
            _deliveryPersonService = _container.Resolve<IDeliveryPersonService>();
            _customerService = _container.Resolve<ICustomerService>();
            _billingService = _container.Resolve<IBillingService>();
            _stockEntryRepository = _container.Resolve<IStockEntryRepository>();
            UpdateFilterModel();
            RefreshGrid();
            deliveryPersonsCombo.ItemsSource = _deliveryPersonService.GetDeliveryPersons();
            customersCombo.ItemsSource = _customerService.GetCustomers();
            productsCombo.ItemsSource = _productService.GetProducts();
        }

        private void OkButtonClicked(object sender, RoutedEventArgs e)
        {
            DateTime selectedDate = dpInputBillingDate.SelectedDate ?? DateTime.Now;
            try
            {
                UpdateFilterModel();
                RefreshGrid();
                
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while processing billing details");
                return;
            }
        }

        private void UpdateFilterModel()
        {
            filterModel = new TransactionsFilterInputModel();
            filterModel.ProductView = (productsCombo.SelectedItem as ProductViewModel);
            filterModel.CustomerView = (customersCombo.SelectedItem as CustomerViewModel);
            filterModel.DeliveryPersonView = (deliveryPersonsCombo.SelectedItem as DeliveryPersonViewModel);
            filterModel.FromDate = dpInputBillingDate.SelectedDate ?? DateTime.MinValue;
            filterModel.ToDate = dpBillingToDate.SelectedDate ?? DateTime.MaxValue;
        }

        private void RefreshGrid()
        {
            if (filterModel == null)
            {
                filterModel = new TransactionsFilterInputModel()
                {
                    FromDate = DateTime.Now.AddDays(-1),
                    ToDate = DateTime.Now
                };
            }
            List<BillingViewModel> billingViewModels =
                          _billingService.GetFilteredBills(filterModel);
            billingDataGrid.ItemsSource = billingViewModels;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            List<StockEntryModel> stockEntries = _stockEntryRepository.GetStockEntries();
            StockEntryModel latestStock = stockEntries.LastOrDefault();
            BillingInsert billingInsert = new BillingInsert(_container, _username);
            billingInsert.txtStockRate.Text = latestStock.UnitRate.ToString();
            billingInsert.billingDate.SelectedDate = _selectedDate;
            billingInsert.productCombo.ItemsSource = _productService.GetProducts();
            billingInsert.deliveryPersonsCombo.ItemsSource = _deliveryPersonService.GetDeliveryPersons();
            billingInsert.customersCombo.ItemsSource = _customerService.GetCustomers();
            billingInsert.ShowDialog();
            RefreshGrid();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btn_billingCancel_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_BillingUpdate_Clicked(object sender, RoutedEventArgs e)
        {
            List<StockEntryModel> stockEntries = _stockEntryRepository.GetStockEntries();
            StockEntryModel latestStock = stockEntries.LastOrDefault();
            BillingViewModel billingViewModel = (billingDataGrid.SelectedItem as BillingViewModel);
            BillingUpdate billingUpdate = new BillingUpdate(_container, billingViewModel, _username);
            billingUpdate.txtStockRate.Text = latestStock.UnitRate.ToString();
            billingUpdate.productCombo.ItemsSource = _productService.GetProducts();
            billingUpdate.deliveryPersonsCombo.ItemsSource = _deliveryPersonService.GetDeliveryPersons();
            billingUpdate.customersCombo.ItemsSource = _customerService.GetCustomers();
            billingUpdate.AssignInterfaceValues(billingViewModel);
            billingUpdate.Title = "Updating Billing Id:" + billingViewModel.BillingId;
            billingUpdate.ShowDialog();
            RefreshGrid();
        }

        private void btn_Invoice_Clicked(object sender, RoutedEventArgs e)
        {
            string response = Utilities.YesNoMessage("Are you sure to generate Bill?");
            if (response.Equals("No"))
            {
                return;
            }

            BillingViewModel billingViewModel = (billingDataGrid.SelectedItem as BillingViewModel);
            InvoiceRepository invoiceRepository = new InvoiceRepository(_username);
            double totalBillAmount = (double)(billingViewModel.DeliveredFullCylinderQty *
                                billingViewModel.Rate);
            InvoiceModel invoice = new InvoiceModel()
            {
                Billing = _billingService.GetModelFromView(billingViewModel),
                AmountWithoutGst = CalculationProvider.GetCustomerInvoiceAmountWithoutGst(totalBillAmount,
                                                billingViewModel.Product.CGstRate,
                                                billingViewModel.Product.SGstRate),
                Customer = _customerService.GetDatabaseModel(billingViewModel.Customer),
                DeliveryPerson = _deliveryPersonService.GetDatabaseModel(billingViewModel.DeliveryPerson),
                Product = _productService.GetDatabaseModel(billingViewModel.Product),
                Cgst = CalculationProvider.GetCustomerInvoiceTax(totalBillAmount,
                        billingViewModel.Product.CGstRate,
                        billingViewModel.Product.CGstRate + billingViewModel.Product.SGstRate),
                InvoiceDate = DateTime.Now,
                Quantity = billingViewModel.DeliveredFullCylinderQty,
                RatePerQuantity =(double)billingViewModel.Rate,
                Sgst = CalculationProvider.GetCustomerInvoiceTax(totalBillAmount,
                        billingViewModel.Product.SGstRate,
                        billingViewModel.Product.CGstRate + billingViewModel.Product.SGstRate),
                TotalAmount= totalBillAmount
            };            
            int invoiceNumber = invoiceRepository.Create(invoice);
            InvoicePrint invoicePrint = new InvoicePrint(invoice, "Original");
            invoicePrint.ShowDialog();
            RefreshGrid();
            Gasware.Database.Utilities.SuccessMessage("Invoice number " + 
                                                       invoiceNumber.ToString() +
                                                      " generated successfully."); 
            

        }

        private void btn_BillingDelete_Clicked(object sender, RoutedEventArgs e)
        {
            BillingViewModel billingModel = (billingDataGrid.SelectedItem as BillingViewModel);
            var result = MessageBox.Show("Are you sure you want to delete the Billing with id " +
              billingModel.BillingId + "?",
               "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result.ToString() == "Yes")
            {
                _billingService.Delete(billingModel.BillingId);
                RefreshGrid();
                Utilities.SuccessMessage("Transaction deleted successfully.");
            }
        }

        private void btnExportToCsvClicked(object sender, RoutedEventArgs e)
        {
            List<BillingReportModel> billingReportModels = _billingService.GetBillingReportModels(filterModel);
            Gasware.Common.CommonFunctions commonFunctions = new Common.CommonFunctions();
            commonFunctions.ExportToCsvFile(billingReportModels);
        }

        private void btnPrintGridClicked(object sender, RoutedEventArgs e)
        {
            PrintFunction printFunction = new PrintFunction();
            printFunction.PrintGrid(billingDataGrid);
        }
    }
}
