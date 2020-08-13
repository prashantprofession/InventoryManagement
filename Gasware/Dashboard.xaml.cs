using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Gasware.Models;
using Gasware.ViewModels;
using Unity;
using Gasware.Service.Interfaces;
using Gasware.Repository.Interfaces;
using Gasware.Repository;
using System.Windows.Threading;
using Gasware.Database;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        public string username;

        private readonly IUnityContainer _container;
        private readonly IVendorService _vendorService;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICustomerService _customerService;
        private readonly IStockEntryRepository _stockEntryRepository;
        private readonly IProductService _productService;
        private readonly IUserRespository _userRespository;
        private readonly IDeliveryPersonService _deliveryPersonService;
        private readonly LicenseViewModule _licenseDetails;
        private readonly IDailyReportRepository _dailyReportRepository;
       
        public Dashboard(string username, IUnityContainer container)
        {
            InitializeComponent();
            this.username = username;            
            _container = container;
            _licenseDetails = new LicenseViewModule(username);            
            btnBilling.Visibility = _licenseDetails.Billing;
            vendor.Visibility = _licenseDetails.Vendor;
            customers.Visibility = _licenseDetails.Customer;
            btnCompany.Visibility = _licenseDetails.Company;
            btnStockIn.Visibility = _licenseDetails.StockEntry;
            products.Visibility = _licenseDetails.Product;
            users.Visibility = _licenseDetails.User;
            deliveryperson.Visibility = _licenseDetails.DeliveryPerson;
            balanceList.ItemsSource = _licenseDetails.StockDetails;
            btnLicense.Visibility = _licenseDetails.LicenseButton;
            lblLicense.Content = _licenseDetails.LicenseExpiryString;
            lblUserName.Content =  username;   
            _vendorService = _container.Resolve<IVendorService>();
            _companyRepository = _container.Resolve<ICompanyRepository>();
            _customerService = _container.Resolve<ICustomerService>();
            _stockEntryRepository = _container.Resolve<IStockEntryRepository>();
            _productService = _container.Resolve<IProductService>();
            _userRespository = _container.Resolve<IUserRespository>();
            _deliveryPersonService = _container.Resolve<IDeliveryPersonService>();
            _dailyReportRepository = _container.Resolve<IDailyReportRepository>();
           // btnLogOut.Content += " Log Out";
            RefreshDashboardButtons();

        }


        private void vendor_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                Vendor vendorScreen = new Vendor(_container, username);
                vendorScreen.ShowDialog();
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while processing vendor details");
                return;
            }
        }

        private void btnEmailTemplate_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                EmailTemplateScreen emailTemplateScreen = new EmailTemplateScreen(_container, username);
                emailTemplateScreen.ShowDialog();
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while processing email template details");
                return;
            }
        }


        private void billing_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Billing_Screen billing_Screen = new Billing_Screen(_container, username);
                billing_Screen.ShowDialog();
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while processing billing details");
                return;
            }
        }



        private void customers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<CustomerViewModel> customerViewModels = _customerService.GetCustomers();
                Customer_Screen customer_Screen = new Customer_Screen(_container, username);
                customer_Screen.customerDataGrid.ItemsSource = customerViewModels;
                customer_Screen.ShowDialog();
                RefreshDashboardButtons();
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while processing customer details");
                return;
            }
        }

        private void deliveryperson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<DeliveryPersonViewModel> deliveryPersonViews = _deliveryPersonService.GetDeliveryPersons();
                DeliveryPersonScreen deliveryPersonScreen = new DeliveryPersonScreen(_container, username);
                deliveryPersonScreen.deliveryPersonGrid.ItemsSource = deliveryPersonViews;
                deliveryPersonScreen.ShowDialog();
                RefreshDashboardButtons();
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while processing delivery person details");
                return;
            }
        }

        private void users_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Users usersScreen = new Users(_container, username);
                usersScreen.usersDataGrid.ItemsSource = _userRespository.GetUsers();
                usersScreen.ShowDialog();
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while processing user details");
            }
        }

        private void RefreshDashboardButtons()
        {
            btnBilling.IsEnabled = _licenseDetails.EnableBillButton();
            btnDailyReport.IsEnabled = _licenseDetails.EnableDailyReportButton();
            btnStockIn.IsEnabled = _licenseDetails.EnableStockEntryButton();
        }

        private void products_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Products productscreen = new Products(_container, username);
                productscreen.productsDataGrid.ItemsSource = _productService.GetProducts();                
                productscreen.ShowDialog();
                RefreshDashboardButtons();
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while processing product details");
                return;
            }

        }

        private void btnCompany_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CompanyModel companyModel = _companyRepository.Get(1);
                Company companyScreen = new Company(_container, username);
                companyScreen.AssignScreenFields(companyModel);
                companyScreen.ShowDialog();
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while processing company details");
                return;
            }
        }

        private void stockin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StockEntry stockEntryScreen = new StockEntry(_container, username);
                stockEntryScreen.stocksDataGrid.ItemsSource = _stockEntryRepository.GetStockEntries();
                stockEntryScreen.ShowDialog();
                RefreshDashboardButtons();
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while processing stock details");
            }

        }

        private void refreshbalances_Click(object sender, RoutedEventArgs e)
        {            
            balanceList.ItemsSource = _licenseDetails.GetStockDetails();
            RefreshDashboardButtons();
            Utilities.SuccessMessage("Balance details updated successfully..");
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dailyReport_Click(object sender, RoutedEventArgs e)
        {
            DailyReportDateInput dailyReport = new DailyReportDateInput(username);
            dailyReport.Show();
        }

        private void btnLicense_Click(object sender, RoutedEventArgs e)
        {
            LicenseScreen licenseScreen = new LicenseScreen(username);
            string licenseString = _companyRepository.GetLicense(1);
            if (!string.IsNullOrEmpty(licenseString))
            {
                licenseScreen.licenseCombo.Text = licenseString;
                licenseScreen.ShowDialog();
            }
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void invoice_Click(object sender, RoutedEventArgs e)
        {
            Invoice invoice = new Invoice(_container, username, DateTime.Now);
            invoice.ShowDialog();

        }

        private void reports_Click(object sender, RoutedEventArgs e)
        {
            Reports reports = new Reports(_container, username);
            reports.ShowDialog();
        }

        private void forecastCustomers_Click(object sender, RoutedEventArgs e)
        {
            ForecastCustomers forecastCustomers = new ForecastCustomers(_container, username);
            forecastCustomers.ShowDialog();
        }

        private void dbOperations_Click(object sender, RoutedEventArgs e)
        {
            DatabaseBackup database = new DatabaseBackup();
            database.ShowDialog();
        }
    }
}
