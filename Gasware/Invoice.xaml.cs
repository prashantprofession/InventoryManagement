using Gasware.Common;
using Gasware.Database;
using Gasware.Models;
using Gasware.ReportModels;
using Gasware.Repository.Interfaces;
using Gasware.Service.Interfaces;
using Gasware.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for Invoice.xaml
    /// </summary>
    public partial class Invoice : Window
    {
        public Invoice()
        {
            InitializeComponent();
        }

        private readonly string _username;
        private readonly DateTime _selectedDate;
        private readonly IUnityContainer _container;
        private readonly IProductService _productService;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ICustomerService _customerService;
        private readonly IBillingService _billingService;
        private readonly IStockEntryRepository _stockEntryRepository;
        private TransactionsFilterInputModel filterModel;

        public Invoice(IUnityContainer container, string username, DateTime selectedDate)
        {
            InitializeComponent();
            _username = username;
            _selectedDate = selectedDate;
            _container = container;
            _productService = _container.Resolve<IProductService>();
            _invoiceRepository = _container.Resolve<IInvoiceRepository>();
            _customerService = _container.Resolve<ICustomerService>();
            _billingService = _container.Resolve<IBillingService>();
            _stockEntryRepository = _container.Resolve<IStockEntryRepository>();         
            customersCombo.ItemsSource = _customerService.GetCustomers();
            productsCombo.ItemsSource = _productService.GetProducts();
            UpdateFilterModel();
            RefreshGrid();
        }

        private void UpdateFilterModel()
        {
            filterModel = new TransactionsFilterInputModel();
            filterModel.ProductView = (productsCombo.SelectedItem as ProductViewModel);
            filterModel.CustomerView = (customersCombo.SelectedItem as CustomerViewModel);
            filterModel.FromDate = dpInputBillingDate.SelectedDate ?? DateTime.MinValue;
            filterModel.ToDate = dpBillingToDate.SelectedDate ?? DateTime.MaxValue;
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

        private void RefreshGrid()
        {
            if (filterModel == null)
            {
                filterModel = new TransactionsFilterInputModel()
                {
                    FromDate = DateTime.MinValue,
                    ToDate = DateTime.MaxValue
                };
            }
            List<InvoiceModel> invoiceModels = _invoiceRepository.GetInvoicesWithFilter(filterModel);
            invoiceDataGrid.ItemsSource = invoiceModels;
        }

        private void btn_billingCancel_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnInvoiceView_Clicked(object sender, RoutedEventArgs e)
        {
            InvoiceModel invoiceModel = (invoiceDataGrid.SelectedItem as InvoiceModel);
            InvoiceView invoiceView = new InvoiceView(invoiceModel.InvoiceId);
            invoiceView.AssignScreenData(invoiceModel);
            invoiceView.ShowDialog();
            RefreshGrid();
        }

        private void btnReprintInvoice_Clicked(object sender, RoutedEventArgs e)
        {
            InvoiceModel invoiceModel = (invoiceDataGrid.SelectedItem as InvoiceModel);
            InvoicePrint invoicePrint = new InvoicePrint(invoiceModel, "Duplicate");
            invoicePrint.ShowDialog();
            this.Close();
        }

        private void btn_Clear_Clicked(object sender, RoutedEventArgs e)
        {

            filterModel = new TransactionsFilterInputModel()
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                ProductView = null,
                CustomerView = null
            };
            dpInputBillingDate.SelectedDate = DateTime.Now;
            dpBillingToDate.SelectedDate = DateTime.Now;
            productsCombo.SelectedItem = null;
            customersCombo.SelectedItem = null;
            RefreshGrid();

        }

        private void btnExportToCsvClicked(object sender, RoutedEventArgs e)
        {
            List<InvoiceReportModel> invoiceModels = _invoiceRepository.GetInvoiceReportsWithFilter(filterModel);
            Gasware.Common.CommonFunctions commonFunctions = new Common.CommonFunctions();
            commonFunctions.ExportToCsvFile(invoiceModels);
        }

        private void btnPrintGridClicked(object sender, RoutedEventArgs e)
        {
            PrintFunction printFunction = new PrintFunction();
            printFunction.PrintGrid(invoiceDataGrid);
        }
    }
}
