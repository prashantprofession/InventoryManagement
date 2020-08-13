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
    /// Interaction logic for DeliveryPersonInsert.xaml
    /// </summary>
    public partial class DeliveryPersonInsert : Window
    {
        public DeliveryPersonInsert()
        {
            InitializeComponent();
        }

        private string _username;
        private readonly IUnityContainer _container;
        private readonly IDeliveryPersonService _deliveryPersonService;

        public DeliveryPersonInsert(IUnityContainer container, string username)
        {
            InitializeComponent();
            _username = username;
            _container = container;
            _deliveryPersonService = _container.Resolve<IDeliveryPersonService>();
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
            txtCustomerName.Text = string.Empty;
            txtPhoneNumber.Text = string.Empty;
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
            if (!ValidationProvider.IsValidPhoneNumber(txtPhoneNumber.Text))
                errorMessage += "Invalid Phone Number." + Environment.NewLine;
            if (string.IsNullOrEmpty(txtAddressLine1.Text))
                errorMessage += "Address line 1 cannot be empty." + Environment.NewLine;
            return errorMessage;
        }

        private void btnSaveDeliveryPerson_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = ValidateInputFields();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Utilities.ErrorMessage(errorMessage);
                return;
            }

            DeliveryPersonViewModel deliveryPersonViewModel = new DeliveryPersonViewModel()
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
                Name = txtCustomerName.Text,
                PhoneNumber = txtPhoneNumber.Text
            };
            _deliveryPersonService.Add(deliveryPersonViewModel);
            MessageBox.Show("Delivery person added successfully ", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
            ClearFields();
        }

        private void btnCloseDeliveryPerson_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
