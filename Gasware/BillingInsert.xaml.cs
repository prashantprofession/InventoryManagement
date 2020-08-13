using Gasware.Common;
using Gasware.Database;
using Gasware.Service.Interfaces;
using Gasware.ViewModels;
using System;
using System.Windows;
using Unity;
using Gasware.Repository.Interfaces;
using Gasware.Models;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for BillingInsert.xaml
    /// </summary>
    public partial class BillingInsert : Window
    {
        private readonly string _username;
        private readonly IUnityContainer _container;
        private readonly IBillingService _billingService;
        private readonly IStockEntryRepository _stockEntryRepository;

        public BillingInsert()
        {
            InitializeComponent();
        }

        public BillingInsert(IUnityContainer container, string username)
        {
            InitializeComponent();
            _username = username;
            _container = container;
            _billingService = _container.Resolve<IBillingService>();
            _stockEntryRepository= _container.Resolve<IStockEntryRepository>();
            ClearFields();
        }

        private void btnSaveBilling_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = IsValidateInput();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Utilities.ErrorMessage(errorMessage);
                return;
            }

            BillingViewModel billingModel = new BillingViewModel()
            {
                Product = (productCombo.SelectedItem as ProductViewModel) ,
                DeliveryPerson = (deliveryPersonsCombo.SelectedItem as DeliveryPersonViewModel),
                Customer = (customersCombo.SelectedItem as CustomerViewModel),
                BilledAmount = Convert.ToDouble(txtBillAmount.Text),
                DeliveredFullCylinderQty = Convert.ToInt32(txtDeliveredQuantity.Text),
                PaidAmount = Convert.ToDouble(txtRecivedAmount.Text),
                ReceivedEmptyCylinderQty = Convert.ToInt32(txtRecivedQuantity.Text),
                BillingDate = billingDate.SelectedDate ?? DateTime.Now,
                Rate = Convert.ToDecimal(txtRate.Text),
                Details = txtDetails.Text
            };
            int billId = _billingService.Add(billingModel);
            if (billId == 0)
                MessageBox.Show("Transaction not successfull.", "Information",
                MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Transaction added successfully.", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
            ClearFields();
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

        private void ClearFields()
        {
            productCombo.SelectedItem = null;
            deliveryPersonsCombo.SelectedItem = null;
            customersCombo.SelectedItem = null;
            txtBillAmount.Text = "0";
            txtDeliveredQuantity.Text = "0";
            txtRecivedAmount.Text = "0";
            txtRecivedQuantity.Text = "0";
            txtRate.Text = "0";
            txtDetails.Text = string.Empty;
        }
     

        private void btnCloseAddBilling_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
            if (customersCombo.SelectedItem != null)
            {
                CustomerViewModel customerViewModel = (customersCombo.SelectedItem as CustomerViewModel);
                if (customerViewModel.DiscountFlat != 0)
                {
                    txtRate.Text = (Convert.ToDecimal(txtRate.Text) -
                                    (decimal)customerViewModel.DiscountFlat).ToString();
                }
                else if (customerViewModel.DiscountPercentage != 0)
                {
                    txtRate.Text = (Convert.ToDecimal(txtRate.Text) -
                                    CalculationProvider.GetPercentageValue(Convert.ToDecimal(txtRate.Text),
                                   (decimal)customerViewModel.DiscountPercentage)).ToString();
                }
            }
        }
    }
}
