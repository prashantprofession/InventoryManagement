using Gasware.Common;
using Gasware.Database;
using Gasware.Models;
using Gasware.Repository;
using System;
using System.Windows;
using System.Windows.Controls;


namespace Gasware
{
    /// <summary>
    /// Interaction logic for InvoicePrint.xaml
    /// </summary>
    public partial class InvoicePrint : Window
    {
        private InvoiceModel _invoiceModel;
        public InvoicePrint(InvoiceModel invoice, string billType)
        {
            InitializeComponent();
            AssignInvoiceValues(invoice, billType);
            _invoiceModel = invoice;
        }

        private void AssignInvoiceValues(InvoiceModel invoice, string invoiceType)
        {
            CompanyRepository companyRepository = new CompanyRepository("admin");
            CompanyModel companyModel = companyRepository.Get(1);
            txtCgstRate.Text = Math.Round(invoice.Product.CGstRate, 2).ToString() + "%";
            txtSgstRate.Text = Math.Round(invoice.Product.SGstRate, 2).ToString() + "%";
            txtCgstAmount.Text = invoice.Cgst.ToString();
            txtSgstAmount.Text = invoice.Sgst.ToString();
            txtInvoiceId.Text = invoice.InvoiceId.ToString().PadLeft(5,'0');
            txtCompanyAddress.Text = companyModel.Address.AddressLine1 + "," +
                                     companyModel.Address.AddressLine2;
            txtCompanyGstNumber.Text = companyModel.GstNumber;
            txtCompanyName.Text = companyModel.Name;
            txtCompanyPhone.Text = companyModel.PhoneNumber;
            txtCompanyPinCode.Text = companyModel.Address.PinCode;
            txtCompanyTownOrCity.Text = companyModel.Address.City;
            txtCustomerName.Text = invoice.Customer.Name;
            txtDateIssued.Text = invoice.InvoiceDate.ToString();
            txtHSNNo.Text =  string.IsNullOrEmpty(companyModel.HSNNumber) ? "27111900" : companyModel.HSNNumber;
            txtParticulars.Text = invoice.Product.Name;
            txtQuantity.Text = invoice.Quantity.ToString();
            txtRate.Text = invoice.RatePerQuantity.ToString();
            txtSubtotal.Text = invoice.AmountWithoutGst.ToString();
            txtType.Text = invoiceType;
            txtTotalBillAmount.Text = invoice.TotalAmount.ToString();
            txtCustomerGst.Text = string.IsNullOrEmpty(invoice.Customer.GstNumber) ? "xxAAAAAxxxxAxAx" : invoice.Customer.GstNumber;
        }

        private void printButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                PrintDialog myPrintDialog = new PrintDialog();
                CommonFunctions commonFunction = new CommonFunctions();                
                if (myPrintDialog.ShowDialog() == true)
                {
                    myPrintDialog.PrintVisual(print, "Printing Invoice");
                    Utilities.SuccessMessage("Invoice Printed successfully.");
                }
            }
            finally
            {
                this.IsEnabled = true;
            }          
        }

        private void sendEmailClicked(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            var result = Utilities.YesNoMessage("Are you sure you want to send an email to customer?");
            if (result.ToString() == "No")
            {
                this.IsEnabled = true;
                return;
            }
            EmailSendProvider emailSendProvider = new EmailSendProvider();
            emailSendProvider.SendEmail(_invoiceModel, "Duplicate");
            this.IsEnabled = true;
        }
    }
}
