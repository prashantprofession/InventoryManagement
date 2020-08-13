using Gasware.Common;
using Gasware.Database;
using Gasware.Models;
using Gasware.Service.Interfaces;
using Gasware.ViewModels;
using System;
using System.Windows;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for VendorChange.xaml
    /// </summary>
    public partial class VendorChange : Window
    {

        private string _username;
        private readonly IUnityContainer _container;
        private static int _vendorId = 0;
        private static int _addressId = 0;
        private VendorViewModel _vendorModel = new VendorViewModel();

        private readonly IVendorService _vendorService;

        public VendorChange(IUnityContainer container, string username, int vendorId)
        {
            InitializeComponent();
            _username = username;
            _container = container;
            _vendorId = vendorId;
            _vendorService = _container.Resolve<IVendorService>();
            if (vendorId == 0)
                ClearFields();
            else
            {
                _vendorModel = _vendorService.Get(vendorId);
                AssignScreenFields(_vendorModel);
            }
            
        }
        public VendorChange()
        {
            InitializeComponent();
        }

        public void AssignScreenFields(VendorViewModel vendorModel)
        {
            _vendorModel = vendorModel;
            _vendorId = _vendorModel.SupplierId;
            _addressId = _vendorModel.Address == null ? 1 : _vendorModel.Address.AddressId;
            txtvendorname.Text = vendorModel.Name ?? string.Empty;
            txtphonenumber.Text = vendorModel.PhoneNumber ?? string.Empty;
            txtstate.Text = vendorModel.Address?.State ?? string.Empty;
            txtaddressline1.Text = vendorModel.Address?.AddressLine1 ?? string.Empty;
            txtaddressline2.Text = vendorModel.Address?.AddressLine2 ?? string.Empty;
            txtcity.Text = vendorModel.Address?.City ?? string.Empty;
            txtcountry.Text = vendorModel.Address?.Country ?? string.Empty;
            txtpincode.Text = vendorModel.Address?.PinCode ?? string.Empty;
            txtstreet.Text = vendorModel.Address?.Street ?? string.Empty;
        }

        public void ClearFields()
        {
            _addressId = 0;
            txtvendorname.Text = string.Empty;
            txtphonenumber.Text = "0";
            txtstate.Text = "Karnataka";
            txtaddressline1.Text = string.Empty;
            txtaddressline2.Text = string.Empty;
            txtcity.Text = string.Empty;
            txtcountry.Text = "India";
            txtpincode.Text = "0";
            txtstreet.Text = string.Empty;

        }

        private AddressModel GetAddressModelFromVendor()
        {
            return new AddressModel()
            {
                AddressId = _addressId,
                AddressLine1 = txtaddressline1.Text,
                AddressLine2 = txtaddressline2.Text,
                City = txtcity.Text,
                Country = txtcountry.Text,
                PinCode = txtpincode.Text,
                State = txtstate.Text,
                Street = txtstreet.Text
            };
        }

        private VendorViewModel GetVendorModelFromUIVendor()
        {
            return new VendorViewModel()
            {
                Address = GetAddressModelFromVendor(),
                Name = txtvendorname.Text,
                PhoneNumber = txtphonenumber.Text,
                SupplierId = _vendorId,
            };
        }

        private string ValidateInputFields()
        {
            string errorMessage = string.Empty;
            if (txtvendorname.Text.Length < 3 || string.IsNullOrEmpty(txtvendorname.Text))
                errorMessage += "Vendor name should have atleast 3 characters";
            if (!ValidationProvider.IsValidPhoneNumber(txtphonenumber.Text))
                errorMessage += "Invalid Phone number";
            if (txtcity.Text.Length < 3 || string.IsNullOrEmpty(txtcity.Text))
                errorMessage += "Invalid City name." + Environment.NewLine;
            if (txtcountry.Text.Length < 3 || string.IsNullOrEmpty(txtcountry.Text))
                errorMessage += "Invalid Country." + Environment.NewLine;
            if (!ValidationProvider.IsValidPinCode(txtpincode.Text))
                errorMessage += "Invalid Pin Code." + Environment.NewLine;
            if (!ValidationProvider.IsValidPhoneNumber(txtphonenumber.Text))
                errorMessage += "Invalid Phone Number." + Environment.NewLine;
            if (string.IsNullOrEmpty(txtaddressline1.Text))
                errorMessage += "Address line 1 cannot be empty." + Environment.NewLine;
            return errorMessage;
        }

        private void btnVendorSave_Click(object sender, RoutedEventArgs e)
        {

            string errorMessage = ValidateInputFields();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Utilities.ErrorMessage(errorMessage);
                return;
            }

            _vendorModel = GetVendorModelFromUIVendor();
            if (_vendorModel.SupplierId == 0 || _vendorModel == null)
            {
                int vendorId = _vendorService.Create(_vendorModel);
                if (vendorId == 0)
                {
                    Utilities.ErrorMessage("Error occured while adding vendor details");
                    return;
                }
                ClearFields();
            }
            else
            {
                _vendorService.Update(_vendorModel);
            }
            Utilities.SuccessMessage("Dealer/Supplier/Vendor details saved successfully.");
        }

        private void btnVendorClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
