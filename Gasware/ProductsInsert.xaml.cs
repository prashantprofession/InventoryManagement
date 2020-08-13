using Gasware.Common;
using Gasware.Database;
using Gasware.Service.Interfaces;
using Gasware.ViewModels;
using System;
using System.Security.Authentication.ExtendedProtection;
using System.Windows;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for ProductsInsert.xaml
    /// </summary>
    public partial class ProductsInsert : Window
    {
        private string _username;
        public IUnityContainer _container { get; }
        private readonly IProductService _productService;

        public ProductsInsert()
        {
            InitializeComponent();
        }

        public ProductsInsert(IUnityContainer container, string username)
        {
            InitializeComponent();
            _username = username;
            _container = container;
            _productService = _container.Resolve<IProductService>();
            ClearFields();
        }

        private void btnSaveProduct_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = ValidateInputFields();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Utilities.ErrorMessage(errorMessage);
                return;
            }
            ProductViewModel productViewModel = new ProductViewModel()
            {
                Name = txtName.Text,
                Details = txtDetails.Text,
                Weight = Int32.Parse(txtWeight.Text.Trim()),
                UnitRate = Convert.ToDecimal(txtUnitRate.Text),
                CGstRate = Convert.ToDecimal(txtCgstRate.Text),
                SGstRate = Convert.ToDecimal(txtSgstRate.Text),
                IsBillable = chkboxIsBillable.IsChecked.Value,
                IsExpense = chkboxIsExpense.IsChecked.Value
            };

            _productService.Add(productViewModel);
            MessageBox.Show("Product added successfully ", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
            ClearFields();
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


        private void btnCloseAddProduct_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        private void ClearFields()
        {
            txtDetails.Text = string.Empty;
            txtWeight.Text = "0";
            txtName.Text = string.Empty;
            txtUnitRate.Text = "0";
            txtCgstRate.Text = "0";
            txtSgstRate.Text = "0";
            chkboxIsBillable.IsChecked = true;
            chkboxIsExpense.IsChecked = false;
        }


        private void btnCloseAddUser_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
