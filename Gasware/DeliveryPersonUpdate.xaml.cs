using Gasware.Common;
using Gasware.Database;
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
    /// Interaction logic for DeliveryPersonUpdate.xaml
    /// </summary>
    public partial class DeliveryPersonUpdate : Window
    {
        public DeliveryPersonUpdate()
        {
            InitializeComponent();
        }

        private string _username;
        private readonly IUnityContainer _container;
        private DeliveryPersonViewModel _deliveryPersonModel;
        private readonly IDeliveryPersonService _deliveryPersonService;
        public DeliveryPersonUpdate(IUnityContainer container, DeliveryPersonViewModel deliveryPersonModel, string username)
        {
            InitializeComponent();
            _deliveryPersonModel = deliveryPersonModel;
            _username = username;
            _container = container;
            _deliveryPersonService = _container.Resolve<IDeliveryPersonService>();
        }

        public void AssignInterfaceValues()
        {
            txtAddressLine1.Text = _deliveryPersonModel.Address.AddressLine1;
            txtAddressLine2.Text = _deliveryPersonModel.Address.AddressLine2;
            txtCity.Text = _deliveryPersonModel.Address.City;
            txtCountry.Text = _deliveryPersonModel.Address.Country;
            txtPinCode.Text = _deliveryPersonModel.Address.PinCode;
            txtState.Text = _deliveryPersonModel.Address.State;
            txtStreet.Text = _deliveryPersonModel.Address.Street;
            txtCustomerName.Text = _deliveryPersonModel.Name;
            txtPhoneNumber.Text = _deliveryPersonModel.PhoneNumber;
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

        private void UpdateModelWithData()
        {
            string errorMessage = ValidateInputFields();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Utilities.ErrorMessage(errorMessage);
                return;
            }
            _deliveryPersonModel = new DeliveryPersonViewModel()
            {
                DeliveryPersonId = _deliveryPersonModel.DeliveryPersonId,
                Address = new Models.AddressModel()
                {
                    AddressId = _deliveryPersonModel.Address.AddressId,
                    AddressLine1 = txtAddressLine1.Text,
                    AddressLine2 = txtAddressLine2.Text,
                    City = txtCity.Text,
                    Country = txtCountry.Text,
                    PinCode = txtPinCode.Text,
                    State = txtState.Text,
                    Street = txtStreet.Text
                },
                Name = txtCustomerName.Text,
                PhoneNumber = txtPhoneNumber.Text,
            };
        }

        private void btnSaveDeliveryPerson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateModelWithData();
              //  DeliveryPersonService deliveryPersonService = new DeliveryPersonService(_username);
                _deliveryPersonService.Update(_deliveryPersonModel);
                MessageBox.Show("Delivery person details updated successfully.", "Success",
                   MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while saving customer data.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnCloseEditDeliveryPerson_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
