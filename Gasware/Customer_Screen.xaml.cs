using Gasware.Common;
using Gasware.Database;
using Gasware.ReportModels;
using Gasware.Repository;
using Gasware.Service;
using Gasware.Service.Interfaces;
using Gasware.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for Customer_Screen.xaml
    /// </summary>
    public partial class Customer_Screen : Window
    {
        public Customer_Screen()
        {
            InitializeComponent();
        }
        private string _username;
        private IUnityContainer _container;
        private readonly ICustomerService _customerService;

        public Customer_Screen(IUnityContainer container, string username)
        {
            InitializeComponent();
            _username = username;
            _container = container;
            _customerService = _container.Resolve<ICustomerService>();
        }

       
        private void btnInsertCustomer_Click(object sender, RoutedEventArgs e)
        {
            InserCustomerScreen inserCustomerScreen = new InserCustomerScreen(_container, _username);         
            inserCustomerScreen.ShowDialog();            
            this.customerDataGrid.ItemsSource = _customerService.GetCustomers();
        }


        private void btnCloseCustomer_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
  

        private void btnCustomerUpdate_Clicked(object sender, RoutedEventArgs e)
        {            
            CustomerViewModel customerViewModel = (customerDataGrid.SelectedItem as CustomerViewModel);
            UpdateCustomerScreen updateCustomer = new UpdateCustomerScreen(_container, customerViewModel, _username);
            updateCustomer.Title = "Updating Customer " + customerViewModel.CustomerId;
            updateCustomer.AssignInterfaceValues();
            updateCustomer.ShowDialog();
            this.customerDataGrid.ItemsSource = _customerService.GetCustomers();
        }

        private void btnCustomerDelete_Clicked(object sender, RoutedEventArgs e)
        {
            CustomerViewModel customerView = (customerDataGrid.SelectedItem as CustomerViewModel);
             var result = MessageBox.Show("Are you sure you want to delete the customer with id " +
               customerView.CustomerId + "?", 
                "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if ( result.ToString() == "Yes")
            {                
                _customerService.Delete(customerView);
                this.customerDataGrid.ItemsSource = _customerService.GetCustomers();
                MessageBox.Show("Customer deleted successfully.", "Success",
                   MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnExportToCsvClicked(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            List<CustomerReportModel> customerModels = _customerService.GetReportModelCustomers();
            CommonFunctions commonFunctions = new CommonFunctions();
            commonFunctions.ExportToCsvFile(customerModels);
            this.IsEnabled = true;
        }

        private void btnPrintGridClicked(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            PrintFunction printFunction = new PrintFunction();
            printFunction.PrintGrid(customerDataGrid);
            this.IsEnabled = true;
            
        }
    }
}
