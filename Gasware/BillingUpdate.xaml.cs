using Gasware.Common;
using Gasware.Database;
using Gasware.Models;
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
    /// Interaction logic for BillingUpdate.xaml
    /// </summary>
    public partial class BillingUpdate : Window
    {
        public BillingUpdate()
        {
            InitializeComponent();
        }

        private readonly string _username;
        private readonly IUnityContainer _container;
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private readonly IDeliveryPersonService _deliveryPersonService;
        private readonly IBillingService _billingService;
        private readonly IStockEntryRepository _stockEntryRepository;
        private BillingViewModel _billingViewModel;
        public BillingUpdate(IUnityContainer container,
                             BillingViewModel billingViewModel, string username)
        {
            InitializeComponent();
            _username = username;
            _container = container;
            _billingViewModel = billingViewModel;
            _productService = _container.Resolve<IProductService>();
            _customerService = _container.Resolve<ICustomerService>();
            _deliveryPersonService = _container.Resolve<IDeliveryPersonService>();
            _billingService = _container.Resolve<IBillingService>();
            _stockEntryRepository = _container.Resolve<IStockEntryRepository>();
        }

        private void UpdateModelWithUIData()
        {
            _billingViewModel = new BillingViewModel()
            {
                BillingId = _billingViewModel.BillingId,
                Product = (productCombo.SelectedItem as ProductViewModel),
                DeliveryPerson = (deliveryPersonsCombo.SelectedItem as DeliveryPersonViewModel),
                Customer = (customersCombo.SelectedItem as CustomerViewModel),
                BilledAmount = Convert.ToDouble(txtBillAmount.Text),
                DeliveredFullCylinderQty = Convert.ToInt32(txtDeliveredQuantity.Text),
                PaidAmount = Convert.ToDouble(txtRecivedAmount.Text),
                ReceivedEmptyCylinderQty = Convert.ToInt32(txtRecivedQuantity.Text),
                BillingDate = _billingViewModel.BillingDate,
                Invoice = new InvoiceModel() { InvoiceId = 0 },
                Details = txtDetails.Text
            };
        }

        public void AssignInterfaceValues(BillingViewModel billingViewModel)
        {
            _billingViewModel = billingViewModel;
            txtBillAmount.Text = billingViewModel.BilledAmount.ToString();
            txtDeliveredQuantity.Text = billingViewModel.DeliveredFullCylinderQty.ToString();
            txtRecivedAmount.Text = billingViewModel.PaidAmount.ToString();
            txtRecivedQuantity.Text = billingViewModel.ReceivedEmptyCylinderQty.ToString();
            txtDetails.Text = billingViewModel.Details;
            txtRate.Text = billingViewModel.Rate.ToString();
            List<ProductViewModel> products = _productService.GetProducts();
            List<DeliveryPersonViewModel> deliveryPeople = _deliveryPersonService.GetDeliveryPersons();
            List<CustomerViewModel> customers = _customerService.GetCustomers();
            productCombo.ItemsSource = products;
            deliveryPersonsCombo.ItemsSource = deliveryPeople;
            customersCombo.ItemsSource = customers;
            productCombo.SelectedIndex = products.FindLastIndex(x =>
                         x.Productid == billingViewModel.Product.Productid);
            deliveryPersonsCombo.SelectedIndex = deliveryPeople.FindLastIndex(x =>
                         x.DeliveryPersonId == billingViewModel.DeliveryPerson.DeliveryPersonId);
            customersCombo.SelectedIndex = customers.FindLastIndex(x =>
                         x.CustomerId == billingViewModel.Customer.CustomerId);
        }

        private void btnEditSaveBilling_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string errorMessage = IsValidateInput();
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Utilities.ErrorMessage(errorMessage);
                    return;
                }
                UpdateModelWithUIData();
                _billingService.Update(_billingViewModel);
                Utilities.SuccessMessage("Transaction updated successfully.");
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while saving billing data.");
            }
        }

        private string IsValidateInput()
        {
            string errorMessage = string.Empty;

            if (productCombo.SelectedItem == null)
                errorMessage += "Invalid Product.Product is not selected";
            else
            {
                ProductViewModel product = (productCombo.SelectedItem as ProductViewModel);
                if (product.IsExpense)
                {
                    if (string.IsNullOrEmpty(txtDetails.Text) || txtDetails.Text.Length < 4)
                        errorMessage += "Expense details are incomplete.";
                    if (!ValidationProvider.IsValidNumber(txtRecivedAmount.Text))
                        errorMessage += "Invalid Received amount.It should be number";
                    if (!ValidationProvider.IsValidNumber(txtBillAmount.Text))
                        errorMessage += "Invalid Bill Amount.It should be number";
                    if (!ValidationProvider.IsValidNumber(txtRate.Text))
                        errorMessage += "Invalid Rate.It should be number";
                    if (deliveryPersonsCombo.SelectedItem == null)
                        errorMessage += "Invalid Delivery Person.Delivery person is not selected";
                }
                else
                {
                    if (!ValidationProvider.IsValidNumber(txtRecivedAmount.Text))
                        errorMessage += "Invalid Received amount.It should be number";

                    if (!ValidationProvider.IsValidNumber(txtDeliveredQuantity.Text))
                        errorMessage += "Invalid Delivered Quantity.It should be number";

                    if (!ValidationProvider.IsValidNumber(txtRecivedQuantity.Text))
                        errorMessage += "Invalid Received Quantity.It should be number";

                    if (!ValidationProvider.IsValidNumber(txtBillAmount.Text))
                        errorMessage += "Invalid Bill Amount.It should be number";

                    if (!ValidationProvider.IsValidNumber(txtRate.Text))
                        errorMessage += "Invalid Rate.It should be number";

                    if (customersCombo.SelectedItem == null)
                        errorMessage += "Invalid Customer.Customer is not selected";

                    if (deliveryPersonsCombo.SelectedItem == null)
                        errorMessage += "Invalid Delivery Person.Delivery person is not selected";
                }

            }
            return errorMessage;
        }




        private void btnCloseEditBilling_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void productCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ProductViewModel productView = (productCombo.SelectedItem as ProductViewModel);
            txtRate.Text = "0";
            if (productView != null && productView.IsBillable)
            {
                StockEntryModel stockEntryModel = _stockEntryRepository.GetLatestStockForProduct(productView);
                txtRate.Text = stockEntryModel.UnitRate.ToString();
                txtStockRate.Text = txtRate.Text;
            }
            if (productView != null && productView.IsExpense)
            {
                txtDeliveredQuantity.Text = "1";
                txtDeliveredQuantity.IsEnabled = false;
                customersCombo.IsEnabled = false;
                txtStockRate.IsEnabled = false;
                txtRecivedQuantity.IsEnabled = false;
            }
        }

        private void customersCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void CalculatteBillAmount(object sender, RoutedEventArgs e)
        {
            ProductViewModel productView = (productCombo.SelectedItem as ProductViewModel);
            if (productView != null && productView.IsBillable)
            {
                if (txtDeliveredQuantity.Text != "0" && txtDeliveredQuantity.Text != null &&
                    txtRate.Text != "0" && txtRate.Text != null)
                    txtBillAmount.Text = (decimal.Parse(txtDeliveredQuantity.Text) *
                                     decimal.Parse(txtRate.Text)).ToString();
            }
            else if (productView != null && productView.IsExpense)
            {
                txtBillAmount.Text = txtRate.Text;
            }
        }
    }
}
