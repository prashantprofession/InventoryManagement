using Gasware.Common;
using Gasware.Database;
using Gasware.Models;
using Gasware.Repository;
using Gasware.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for StockEntryUpdate.xaml
    /// </summary>
    public partial class StockEntryUpdate : Window
    {
        public StockEntryUpdate()
        {
            InitializeComponent();
        }

        private string _username;
        private readonly IUnityContainer _container;
        private StockEntryModel _stockEntryModel;
        private IProductRepository _productServiceProvider;
        private IDeliveryPersonRepository _deliveryPersonService;
        private IStockEntryRepository _stockEntryRepository;
        private IDailyReportRepository _dailyReportRepository;

        public StockEntryUpdate(IUnityContainer container,
                                StockEntryModel stockEntryModel,
                                string username)
        {
            InitializeComponent();
            _username = username;
            _stockEntryModel = stockEntryModel;
            _container = container;
            _productServiceProvider = _container.Resolve<IProductRepository>();
            _deliveryPersonService = _container.Resolve<IDeliveryPersonRepository>();
            _stockEntryRepository = _container.Resolve<IStockEntryRepository>();
            _dailyReportRepository = _container.Resolve<IDailyReportRepository>();
        }



        public void AssignInterfaceValues()
        {
            txtQuantity.Text = _stockEntryModel.Quantity.ToString();
            dateReceived.SelectedDate = _stockEntryModel.ReceivedDate;
            List<ProductModel> productModels = _productServiceProvider.GetProducts();
            List<DeliveryPersonModel> receivers = _deliveryPersonService.GetDeliveryPersons();

            productCombo.ItemsSource = _productServiceProvider.GetProducts();
            receiversCombo.ItemsSource = _deliveryPersonService.GetDeliveryPersons();
            productCombo.SelectedIndex = productModels.FindLastIndex(x =>
                        x.Productid == _stockEntryModel.Product.Productid);
            receiversCombo.SelectedIndex = receivers.FindLastIndex(x =>
                         x.DeliveryPersonId == _stockEntryModel.ReceivedBy.DeliveryPersonId);
            txtUnitRate.Text = _stockEntryModel.UnitRate.ToString();
            txtBalance.Text = _stockEntryModel.Balance.ToString();
            txtPaidAmount.Text = _stockEntryModel.PaidAmount.ToString();
            txtBilledAmount.Text = _stockEntryModel.BilledAmount.ToString();
        }

        private void UpdateModelWithData()
        {
            CompanyRepository companyRepository = new CompanyRepository(_username);
            decimal cgstRate = companyRepository.GetTaxRate(1, "cgst");
            decimal sgstRate = companyRepository.GetTaxRate(1, "sgst");
            _stockEntryModel = new StockEntryModel()
            {
                StockEntryId = _stockEntryModel.StockEntryId,
                ReceivedDate = (DateTime)dateReceived.SelectedDate,
                Quantity = Int32.Parse(txtQuantity.Text),
                Product = (productCombo.SelectedItem as ProductModel),
                ReceivedBy = (receiversCombo.SelectedItem as DeliveryPersonModel),
                UnitRate = Decimal.Parse(txtUnitRate.Text),
                Balance = Double.Parse(txtBalance.Text),
                PaidAmount = Double.Parse(txtPaidAmount.Text),
                CgstPaid = CalculationProvider.GetPercentageValue(decimal.Parse(txtBilledAmount.Text),
                                                                  cgstRate),
                SgstPaid = CalculationProvider.GetPercentageValue(decimal.Parse(txtBilledAmount.Text),
                                                                  sgstRate),
                BilledAmount = double.Parse(txtBilledAmount.Text)
            };
        }

        private string ValidateInputFields()
        {
            string errorMessage = string.Empty;
            if (dateReceived.SelectedDate == null)
                errorMessage += "Invalid received date." + Environment.NewLine;
            if (!ValidationProvider.IsValidNumber(txtQuantity.Text))
                errorMessage += "Invalid Weight." + Environment.NewLine;

            if (productCombo.SelectedItem == null)
                errorMessage += "Invalid Product name." + Environment.NewLine;

            if (receiversCombo.SelectedItem == null)
                errorMessage += "Invalid Receiver's name." + Environment.NewLine;

            if (!ValidationProvider.IsValidNumber(txtUnitRate.Text))
                errorMessage += "Invalid Unit Rate." + Environment.NewLine;

            if (!ValidationProvider.IsValidNumber(txtBalance.Text))
                errorMessage += "Invalid Balance Amount." + Environment.NewLine;

            if (!ValidationProvider.IsValidNumber(txtPaidAmount.Text))
                errorMessage += "Invalid Paid Amount." + Environment.NewLine;

            if (!ValidationProvider.IsValidNumber(txtUnitRate.Text))
                errorMessage += "Invalid Billed Amount." + Environment.NewLine;

            return errorMessage;
        }


        private void btnSaveStockIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string errorMessage = ValidateInputFields();
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Utilities.ErrorMessage(errorMessage);
                    return;
                }
                StockEntryModel oldStockEntryModel = _stockEntryModel;
                UpdateModelWithData();
                _stockEntryRepository.Update(oldStockEntryModel, _stockEntryModel);
                MessageBox.Show("Stock saved successfully.", "Success",
                   MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while saving stock data.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void btnCloseAddStockIn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void productCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            txtUnitRate.Text = (productCombo.SelectedItem as ProductModel).UnitRate.ToString();
            txtBilledAmount.Text = (Convert.ToDecimal( txtUnitRate.Text ) * 
                                   Convert.ToDecimal( txtQuantity.Text)).ToString();

        }

        private void CalculatteBillAmount(object sender, RoutedEventArgs e)
        {
            txtBilledAmount.Text = (Convert.ToDecimal(txtUnitRate.Text) *
                                   Convert.ToDecimal(txtQuantity.Text)).ToString();
        }

        private void productSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            txtUnitRate.Text = (productCombo.SelectedItem as ProductModel).UnitRate.ToString();
            txtBilledAmount.Text = (Convert.ToDecimal(txtUnitRate.Text) *
                                   Convert.ToDecimal(txtQuantity.Text)).ToString();
        }
    }
}
