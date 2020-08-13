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
    /// Interaction logic for UpdateCustomerScreen.xaml
    /// </summary>
    public partial class UpdateCustomerScreen : Window
    {
        public UpdateCustomerScreen()
        {
            InitializeComponent();
        }
        private string _username;
        private CustomerViewModel _customerViewModel;
        private readonly IUnityContainer _container;
        private readonly ICustomerService _customerService;

        public UpdateCustomerScreen(IUnityContainer container,
                                    CustomerViewModel customerViewModel,
                                    string username)
        {
            InitializeComponent();
            _username = username;
            _customerViewModel = customerViewModel;
            _container = container;
            _customerService = _container.Resolve<ICustomerService>();
        }

        public void AssignInterfaceValues()
        {
            txtAddressLine1.Text = _customerViewModel.Address.AddressLine1;
            txtAddressLine2.Text = _customerViewModel.Address.AddressLine2;
            txtCity.Text = _customerViewModel.Address.City;
            txtCountry.Text = _customerViewModel.Address.Country ?? "India";
            txtPinCode.Text = _customerViewModel.Address.PinCode;
            txtState.Text = _customerViewModel.Address.State;
            txtStreet.Text = _customerViewModel.Address.Street;
            txtDepositAmount.Text = _customerViewModel.DepositAmount.ToString();
            txtDiscountFlat.Text = _customerViewModel.DiscountFlat.ToString();
            txtDiscountPercentage.Text = _customerViewModel.DiscountPercentage.ToString();
            txtLocation.Text = _customerViewModel.Location;
            txtCustomerName.Text = _customerViewModel.Name;
            txtPhoneNumber.Text = _customerViewModel.PhoneNumber;
            txtPrice.Text = _customerViewModel.Price.ToString();
            txtCustomerGSTNo.Text = _customerViewModel.GstNumber;
            txtEmailId.Text = _customerViewModel.EmailId;
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

        private void UpdateModelWithData()
        {
            _customerViewModel = new CustomerViewModel()
            {
                CustomerId = _customerViewModel.CustomerId,
                Address = new Models.AddressModel()
                {
                    AddressId = _customerViewModel.Address.AddressId,
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
        }

        private void btnCustomerUpdateSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string errorMessage = ValidateInputFields();
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Utilities.ErrorMessage(errorMessage);
                    return;
                }
                UpdateModelWithData();                
                _customerService.Update(_customerViewModel);
                MessageBox.Show("Customer details updated successfully.", "Success",
                   MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while saving customer data.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnCloseUpdateCustomer_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
