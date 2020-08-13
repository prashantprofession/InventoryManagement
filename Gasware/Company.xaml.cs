using Gasware.Common;
using Gasware.Database;
using Gasware.Models;
using Gasware.Repository.Interfaces;
using System;
using System.IO;
using System.Windows;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for Company.xaml
    /// </summary>
    public partial class Company : Window
    {
        private string _username;
        private readonly IUnityContainer _container;
        private static int _companyId = 0;
        private static int _addressId = 0;        
        private CompanyModel _companyModel = new CompanyModel();       
        private readonly ICompanyRepository _companyRepository;

        public Company(IUnityContainer container, string username)
        {
            InitializeComponent();
            _username = username;
            _container = container;
            _companyRepository = container.Resolve<ICompanyRepository>();
        }

        

        public void AssignScreenFields(CompanyModel companyModel)
        {
            _companyModel = companyModel;
            _companyId = companyModel.CompanyId;
            _addressId = _companyModel.Address.AddressId;
            txtname.Text = companyModel.Name ?? string.Empty;
            txtphonenumber.Text = companyModel.PhoneNumber ?? string.Empty;
            txtstate.Text = companyModel.Address?.State ?? string.Empty;
            txtaddressline1.Text = companyModel.Address?.AddressLine1 ?? string.Empty;
            txtaddressline2.Text = companyModel.Address?.AddressLine2 ?? string.Empty;
            txtcity.Text = companyModel.Address?.City ?? string.Empty;
            txtcountry.Text = companyModel.Address?.Country ?? "India";
            txtpincode.Text = companyModel.Address?.PinCode ?? string.Empty;
            txtstreet.Text = companyModel.Address?.Street ?? string.Empty;
            txtCgst.Text = companyModel.CentralGst.ToString();
            txtSgst.Text = companyModel.StateGst.ToString();
            txtReportsPath.Text = companyModel.ReportsPath;
            txtGstNumber.Text = companyModel.GstNumber;
            txtAccountNumber.Text = companyModel.AccountNumber;
            txtIFSCCode.Text = companyModel.IFSCCode;
            txtemailid.Text = companyModel.EmailId;
            txthsnno.Text = companyModel.HSNNumber;
            emailPasswordBox.Password = companyModel.Password;
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

        private CompanyModel GetCompanyModelFromUIVendor()
        {
            return new CompanyModel()
            {
                Address = GetAddressModelFromVendor(),
                Name = txtname.Text,
                PhoneNumber = txtphonenumber.Text,
                CompanyId = _companyId,
                CentralGst = decimal.Parse(txtCgst.Text),
                StateGst = decimal.Parse(txtSgst.Text),
                ReportsPath = txtReportsPath.Text,
                GstNumber = txtGstNumber.Text,
                AccountNumber = txtAccountNumber.Text,
                IFSCCode = txtIFSCCode.Text,
                EmailId = txtemailid.Text,
                HSNNumber = txthsnno.Text,
                Password = emailPasswordBox.Password.ToString()
            };
        }

        private void btnCompanySave_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = ValidateInputs();
            if(!string.IsNullOrEmpty(errorMessage))
            {
                Utilities.ErrorMessage(errorMessage);
                return;
            }
            _companyModel = GetCompanyModelFromUIVendor();

            if (_companyModel.CompanyId == 0 || _companyModel == null)
            {
                int vendorId = _companyRepository.Create(_companyModel);
                if (vendorId == 0)
                    return;
            }
            else
            {
                _companyRepository.Update(_companyModel);
            }
            MessageBox.Show("Company details saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private string ValidateInputs()
        {
            string errorMessage = string.Empty;
            if (!ValidationProvider.IsValidPhoneNumber(txtphonenumber.Text))
                errorMessage += "Invalid Phone number." + Environment.NewLine;
            if (string.IsNullOrEmpty(txtcity.Text))
                errorMessage += "City cannot be blank." + Environment.NewLine;
            if (string.IsNullOrEmpty(txtcountry.Text))
                errorMessage += "Country cannot be blank." + Environment.NewLine;
            if (!ValidationProvider.IsValidPinCode(txtpincode.Text))
                errorMessage += "Invalid Pin Code." + Environment.NewLine;

            if (!ValidationProvider.IsValidTaxRate(txtCgst.Text))
                errorMessage += "Invalid Cgst Tax Rate." + Environment.NewLine;

            if (Convert.ToDecimal(txtCgst.Text) <=0 ||
                Convert.ToDecimal(txtCgst.Text) >= 100)
                errorMessage += "C-Gst tax rate must be between 0 and 100" + Environment.NewLine; 

            if (Convert.ToDecimal(txtSgst.Text) <= 0 ||
                Convert.ToDecimal(txtSgst.Text) >= 100)
                errorMessage += "S-Gst tax rate must be between 0 and 100" + Environment.NewLine; 

            if (!ValidationProvider.IsValidTaxRate(txtSgst.Text))
                errorMessage += "Invalid Sgst Tax Rate." + Environment.NewLine;
            if (txtGstNumber.Text.Length < 15)
                errorMessage += "Invalid Gst License." + Environment.NewLine;

            if (!Directory.Exists(txtReportsPath.Text))
                errorMessage += "Invalid Reports directory. Mention the correct existing path.";
            else
              txtReportsPath.Text =  txtReportsPath.Text.Replace(@"\", @"\\");

            if (!string.IsNullOrEmpty(txtemailid.Text) && string.IsNullOrEmpty(emailPasswordBox.Password.ToString()))
                errorMessage += "Password cannot be empty for the give email id";
            return errorMessage;
        }

        private void btnCompanyClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
