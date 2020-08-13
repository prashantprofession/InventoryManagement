using Gasware.Common;
using Gasware.Database;
using Gasware.Service.Interfaces;
using Gasware.ViewModels;
using System;
using System.Windows;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for InserCustomerScreen.xaml
    /// </summary>
    public partial class InserCustomerScreen : Window
    {
        public InserCustomerScreen()
        {
            InitializeComponent();
        }

        private string _username;
        private readonly IUnityContainer _container;
        private readonly ICustomerService _customerService;

        public InserCustomerScreen(IUnityContainer container, string username)
        {
            InitializeComponent(); 
            _username = username;
            _container = container;
            _customerService = _container.Resolve<ICustomerService>();
        }

        private void ClearFields()
        {
            txtAddressLine1.Text = string.Empty;
            txtAddressLine2.Text = string.Empty;
            txtCity.Text = string.Empty;
            txtCountry.Text = string.Empty;
            txtPinCode.Text = string.Empty;
            txtState.Text = string.Empty;
            txtStreet.Text = string.Empty;
            txtDepositAmount.Text = string.Empty;
            txtDiscountFlat.Text = string.Empty;
            txtDiscountPercentage.Text = string.Empty;
            txtLocation.Text = string.Empty;
            txtCustomerName.Text = string.Empty;
            txtPhoneNumber.Text = string.Empty;
            txtPrice.Text = string.Empty;
            txtCustomerGSTNo.Text = string.Empty;
            txtEmailId.Text = string.Empty;
        }

        private string ValidateInputFields()
        {
            string errorMessage = string.Empty;
            if (txtCity.Text.Length < 3 || string.IsNullOrEmpty(txtCity.Text))
                errorMessage += "Invalid City name." + Environment.NewLine;
            if (txtCountry.Text.Length < 3 || string.IsNullOrEmpty(txtCountry.Text))
                errorMessage += "Invalid Country." + Environment.NewLine;
            if (!ValidationProvider.IsValidPinCode(txtPinCode.Text))
                errorMessage += "Invalid Pin Code." + Environment.NewLine;
            if (!ValidationProvider.IsValidNumber(txtDepositAmount.Text))
                errorMessage += "Invalid Deposit Amount." + Environment.NewLine;
            if (!ValidationProvider.IsValidNumber(txtDiscountFlat.Text))
                errorMessage += "Invalid Discount Flat Rate." + Environment.NewLine;
            if (!ValidationProvider.IsValidNumber(txtDiscountPercentage.Text))
                errorMessage += "Invalid Discount Percentage." + Environment.NewLine;
            if (Convert.ToInt32(txtDiscountPercentage.Text) < 0 ||
                Convert.ToInt32(txtDiscountPercentage.Text) > 100)
                errorMessage += "Discount percentage value must be between 0 and 100 ";
            if (!ValidationProvider.IsValidPhoneNumber(txtPhoneNumber.Text))
                errorMessage += "Invalid Phone Number." + Environment.NewLine;
            if (!ValidationProvider.IsValidNumber(txtPrice.Text))
                errorMessage += "Invalid Price." + Environment.NewLine;
            return errorMessage;
        }

        private void btnSaveCustomer_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = ValidateInputFields();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Utilities.ErrorMessage(errorMessage);
                return;
            }    
            CustomerViewModel customerView = new CustomerViewModel()
            {
                Address = new Models.AddressModel()
                {
                    AddressLine1 = txtAddressLine1.Text,
                    AddressLine2 = txtAddressLine2.Text,
                    City = txtCity.Text,
                    Country = txtCountry.Text,
                    PinCode = txtPinCode.Text,
                    State = txtState.Text,
                    Street = txtStreet.Text
                },
                DepositAmount = Convert.ToDouble(txtDepositAmount.Text),
                DiscountFlat = Convert.ToInt32(txtDiscountFlat.Text),
                DiscountPercentage = Convert.ToInt32(txtDiscountPercentage.Text),
                Location = txtLocation.Text,
                Name = txtCustomerName.Text,
                PhoneNumber = txtPhoneNumber.Text,
                Price = Convert.ToDouble(txtPrice.Text),
                GstNumber = txtCustomerGSTNo.Text,
                EmailId = txtEmailId.Text

            };
            _customerService.Add(customerView);
            MessageBox.Show("Customer added successfully ", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
            ClearFields();
        }

        private void btnCloseAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
