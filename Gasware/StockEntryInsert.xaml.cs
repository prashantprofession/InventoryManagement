using Gasware.Common;
using Gasware.Database;
using Gasware.Models;
using Gasware.Repository;
using Gasware.Repository.Interfaces;
using System;
using System.Windows;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for StockEntryInsert.xaml
    /// </summary>
    public partial class StockEntryInsert : Window
    {
        private string _username;
        private readonly IUnityContainer _container;
        private readonly IStockEntryRepository _stockEntryRepository;
        private readonly IDailyReportRepository _dailyReportRepository;

        public StockEntryInsert()
        {
            InitializeComponent();
        }

        public StockEntryInsert(IUnityContainer container, string username)
        {
            InitializeComponent();
            _username = username;
            _container = container;
            _stockEntryRepository = _container.Resolve<IStockEntryRepository>();
            _dailyReportRepository = _container.Resolve<IDailyReportRepository>();
            dateReceived.SelectedDate = DateTime.Now;
            ClearFields();
        }

        private string ValidateInputFields()
        {
            string errorMessage = string.Empty;
            if (dateReceived.SelectedDate == null)
                errorMessage += "Invalid received date." + Environment.NewLine;
            if (!ValidationProvider.IsValidNumber(txtQuantity.Text))
                errorMessage += "Invalid Quantity." + Environment.NewLine;

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
            string errorMessage = ValidateInputFields();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Utilities.ErrorMessage(errorMessage);
                return;
            }
            CompanyRepository companyRepository = new CompanyRepository(_username);
            decimal cgstRate = companyRepository.GetTaxRate(1, "cgst");
            decimal sgstRate = companyRepository.GetTaxRate(1, "sgst");
            StockEntryModel stockEntryModel = new StockEntryModel()
            {
                ReceivedDate = dateReceived.SelectedDate.Value,
                Quantity = Int32.Parse(txtQuantity.Text),
                Product = (productCombo.SelectedItem as ProductModel),
                ReceivedBy = (receiversCombo.SelectedItem as DeliveryPersonModel),
                UnitRate = Decimal.Parse(txtUnitRate.Text),
                Balance = Double.Parse(txtBalance.Text),
                PaidAmount = Double.Parse(txtPaidAmount.Text),
                BilledAmount = Double.Parse(txtBilledAmount.Text),
                CgstPaid = CalculationProvider.GetPercentageValue(decimal.Parse(txtBilledAmount.Text),
                                                                  cgstRate),
                SgstPaid = CalculationProvider.GetPercentageValue(decimal.Parse(txtBilledAmount.Text),
                                                                  sgstRate)
            };
            _stockEntryRepository.Create(stockEntryModel);
            MessageBox.Show("Stock added successfully ", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
            ClearFields();
        }

        private void btnCloseAddProduct_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ClearFields()
        {
            txtQuantity.Text = "0";
            productCombo.SelectedItem = string.Empty;
            receiversCombo.SelectedItem = DateTime.Today;
            txtBilledAmount.Text = "0";
            txtBalance.Text = "0";
            txtPaidAmount.Text = "0";         
            txtUnitRate.Text = "0";
        }

        private void btnCloseAddStockIn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CalculatteBillAmount(object sender, RoutedEventArgs e)
        {
            if (txtQuantity.Text != "0" && txtQuantity.Text != string.Empty &&
                txtUnitRate.Text != "0" && txtUnitRate.Text != string.Empty )
            txtBilledAmount.Text = (Convert.ToDecimal(txtQuantity.Text) * 
                                   Convert.ToDecimal(txtUnitRate.Text)).ToString();
        }

        private void productSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            txtUnitRate.Text = (productCombo.SelectedItem as ProductModel)?.UnitRate.ToString() ?? "0";
        }

       
    }
}
