using Gasware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

namespace Gasware
{
    /// <summary>
    /// Interaction logic for InvoiceView.xaml
    /// </summary>
    public partial class InvoiceView : Window
    {
        public InvoiceView(int id)
        {
            InitializeComponent();
            lblInvoiceTitleContent.Content = "Invoice ID :" + id.ToString();
        }

        public void AssignScreenData(InvoiceModel invoiceModel)
        {
            txtCustomer.Text = invoiceModel.Customer.Name;
            txtAmount.Text = invoiceModel.AmountWithoutGst.ToString();
            txtBillingId.Text = invoiceModel.Billing.BillingId.ToString();
            txtCgst.Text = invoiceModel.Cgst.ToString();
            txtSgst.Text = invoiceModel.Sgst.ToString();
            txtTotalAmount.Text = invoiceModel.TotalAmount.ToString();
            txtProduct.Text = invoiceModel.Product.Name;
            txtQuantity.Text = invoiceModel.Quantity.ToString();
            dpInvoiceDate.SelectedDate = invoiceModel.InvoiceDate;
            txtRatePerQty.Text = invoiceModel.RatePerQuantity.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
