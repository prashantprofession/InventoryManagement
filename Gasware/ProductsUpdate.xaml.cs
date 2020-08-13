using Gasware.Common;
using Gasware.Database;
using Gasware.Models;
using Gasware.Repository;
using Gasware.Service.Interfaces;
using Gasware.ViewModels;
using System;
using System.Windows;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for ProductsUpdate.xaml
    /// </summary>
    public partial class ProductsUpdate : Window
    {
        public ProductsUpdate()
        {
            InitializeComponent();
        }

        private string _username;
        private ProductViewModel _productViewModel;
        private readonly IUnityContainer _container;
        private readonly IProductService _productService;

        public ProductsUpdate(IUnityContainer container, ProductViewModel productViewModel, string username)
        {
            InitializeComponent();
            _username = username;
            _productViewModel = productViewModel;
            _container = container;
            _productService = _container.Resolve<IProductService>();
        }


        private string ValidateInputFields()
        {

            string errorMessage = string.Empty;
            if (chkboxIsBillable.IsChecked.Value && chkboxIsExpense.IsChecked.Value)
                errorMessage += "Billable product cannot be expenses. Product should either be billable or expense." + Environment.NewLine;
            if (chkboxIsBillable.IsChecked.Value)
            {
                if (!ValidationProvider.IsValidNumber(txtWeight.Text))
                    errorMessage += "Invalid product weight." + Environment.NewLine;
                if (!ValidationProvider.IsValidNumber(txtUnitRate.Text))
                    errorMessage += "Invalid product unit rate." + Environment.NewLine;
                if (!ValidationProvider.IsValidTaxRate(txtCgstRate.Text))
                    errorMessage += "Invalid Cgst Tax Rate." + Environment.NewLine;
                if (Convert.ToDecimal(txtCgstRate.Text) <= 0 ||
                    Convert.ToDecimal(txtCgstRate.Text) >= 100)
                    errorMessage += "C-Gst tax rate must be between 0 and 100" + Environment.NewLine;
                if (!ValidationProvider.IsValidTaxRate(txtSgstRate.Text))
                    errorMessage += "Invalid Sgst Tax Rate." + Environment.NewLine;
                if (Convert.ToDecimal(txtSgstRate.Text) <= 0 ||
                    Convert.ToDecimal(txtSgstRate.Text) >= 100)
                    errorMessage += "S-Gst tax rate must be between 0 and 100" + Environment.NewLine;
            }
            return errorMessage;
        }


        private void btnCloseUpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        public void AssignInterfaceValues()
        {
            txtDetails.Text = _productViewModel.Details;
            txtName.Text = _productViewModel.Name;
            txtWeight.Text = _productViewModel.Weight.ToString();
            txtUnitRate.Text = _productViewModel.UnitRate.ToString();
            txtCgstRate.Text = _productViewModel.CGstRate.ToString();
            txtSgstRate.Text = _productViewModel.SGstRate.ToString();
            chkboxIsBillable.IsChecked = _productViewModel.IsBillable;
            chkboxIsExpense.IsChecked = _productViewModel.IsExpense;
        }

        private void UpdateModelWithData()
        {
            _productViewModel = new ProductViewModel()
            {
                Productid = _productViewModel.Productid,
                Weight = Convert.ToInt32(Math.Round(Convert.ToDouble(txtWeight.Text))),
                Details = txtDetails.Text,
                Name = txtName.Text,
                UnitRate = Decimal.Parse(txtUnitRate.Text),
                CGstRate = Decimal.Parse(txtCgstRate.Text),
                SGstRate = Decimal.Parse(txtSgstRate.Text),
                IsBillable = chkboxIsBillable.IsChecked.Value,
                IsExpense = chkboxIsExpense.IsChecked.Value
            };           
        }

        private void btnSaveProductUpdate_Click(object sender, RoutedEventArgs e)
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
                _productService.Update(_productViewModel);
                Utilities.SuccessMessage("Product details saved successfully.");
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while saving product data.");
            }

        }

    }
}
