using Gasware.Database;
using Gasware.Models;
using Gasware.ReportModels;
using Gasware.Repository.Interfaces;
using Gasware.Service;
using Gasware.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;


namespace Gasware.Repository
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly string _username;
        public InvoiceRepository(string username)
        {
            _username = username;
        }
        public int Create(InvoiceModel invoice)
        {
            try
            {
                string _nextBillingIdQuery = "SELECT max(invoiceid) from invoice";
                int nextId = StaticIdProvider.GetNextId(_nextBillingIdQuery);
                string insertInvoiceSql = GetInsertQueryStatement(invoice, nextId);
                DbQueryExecuter.WriteToDatabase(insertInvoiceSql);
                BillingRepository billingRepository = new BillingRepository(_username);
                //InvoiceModel invoiceModel = Get(nextId);
                invoice.InvoiceId = nextId;
                BillingModel billing = billingRepository.GetBilling(invoice.Billing.BillingId);
                billing.Invoice = invoice;
                billingRepository.Update(billing);
                return nextId;
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while adding invoice details");
                return 0;
            }
        }

        public void Delete(int id)
        {
            try
            {
                string deleteQuery = "Update invoice set deactivatedate = '" + Utilities.GetDatabaseDate(DateTime.Now) +
                                     "' where invoiceid=" + id + ";";
                DbQueryExecuter.WriteToDatabase(deleteQuery);
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while deleting product details");
            }
        }

        public InvoiceModel Get(int id)
        {
            try
            {
                string sql = "SELECT * FROM invoice where  deactivatedate is null and invoiceid=" + id.ToString();
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;

                InvoiceModel invoice = new InvoiceModel();
                while (rdr.Read())
                {
                    invoice = GetModelForRawData(rdr);
                }

                mySqlConnectionModel.MySqlConnection.Close();
                if (invoice.InvoiceId == 0)
                {
                    return null;
                }
                return invoice;
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while getting invoice details");
                return null;
            }
        }

        public List<InvoiceModel> GetInvoices()
        {
            try
            {

                string sql = "SELECT * FROM invoice where deactivatedate is null";
                List<InvoiceModel> invoiceModels = new List<InvoiceModel>();
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
                InvoiceModel invoiceModel = new InvoiceModel();
                while (rdr.Read())
                {
                    invoiceModel = GetModelForRawData(rdr);
                    invoiceModels.Add(invoiceModel);
                }
                mySqlConnectionModel.MySqlConnection.Close();
                if (invoiceModels.Count > 0)
                {
                    invoiceModels = SetCustomerAndProducts(invoiceModels);
                }
                return invoiceModels;
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while getting invoice details");
                return null;
            }
        }

        private List<InvoiceModel> SetCustomerAndProducts(List<InvoiceModel> invoiceModels)
        {
            ProductServiceProvider productService = new ProductServiceProvider("admin");
            CustomerServiceProvider customerService = new CustomerServiceProvider("admin");
            DeliveryPersonService deliveryPersonService = new DeliveryPersonService("admin");
            BillingRepository billingRepository = new BillingRepository("admin");
            foreach (InvoiceModel invoice in invoiceModels)
            {
                BillingModel billing = billingRepository.GetBilling(invoice.Billing.BillingId);
                invoice.Product = productService.GetDatabaseModel(billing.Product);
                invoice.Customer = customerService.GetDatabaseModel(billing.Customer);
                invoice.Billing = billing;
            }
            return invoiceModels;
        }

        public List<InvoiceModel> GetInvoicesWithFilter(TransactionsFilterInputModel filterModel)
        {
            List<InvoiceModel> invoiceModels = GetInvoices();
            DateTime toDate = filterModel.ToDate != DateTime.MaxValue ?
                                        filterModel.ToDate.AddDays(1) :
                                        filterModel.ToDate;            
            invoiceModels = invoiceModels.Where(x => x.InvoiceDate >= filterModel.FromDate &&
                                                     x.InvoiceDate <= toDate).ToList();
            if (filterModel.ProductView != null)
            {
                invoiceModels = invoiceModels.Where(x => x.Product.Productid ==
                                    filterModel.ProductView.Productid).ToList();
            }

            if (filterModel.CustomerView != null)
            {
                invoiceModels = invoiceModels.Where(x => x.Customer.CustomerId ==
                                    filterModel.CustomerView.CustomerId).ToList();
            }

            return invoiceModels;
        }

        public List<InvoiceReportModel> GetInvoiceReportsWithFilter(TransactionsFilterInputModel filterModel)
        {
            List<InvoiceModel> invoiceModels = GetInvoicesWithFilter(filterModel);
            List<InvoiceReportModel> invoiceReports = new List<InvoiceReportModel>();
            foreach(InvoiceModel invoiceModel in invoiceModels)
            {
                invoiceReports.Add(new InvoiceReportModel()
                {
                    AmountWithoutGst = invoiceModel.AmountWithoutGst,
                    BillNumber = invoiceModel.Billing.BillingId,
                    Cgst = invoiceModel.Cgst,
                    Customer = invoiceModel.Customer.Name,
                    DeliveryPerson = invoiceModel.Billing.DeliveryPerson.Name,
                    InvoiceDate = invoiceModel.InvoiceDate,
                    InvoiceId = invoiceModel.InvoiceId,
                    Product = invoiceModel.Product.Name,
                    Quantity = invoiceModel.Quantity,
                    RatePerQuantity = invoiceModel.RatePerQuantity,
                    Sgst = invoiceModel.Sgst,
                    TotalAmount = invoiceModel.TotalAmount

                });
            }
            return invoiceReports;
        }

        public List<InvoiceModel> GetInvoicesForDateRange(DateTime fromDate, DateTime toDate)
        {
            try
            {

                string sql = "SELECT * FROM invoice where deactivatedate is null and invoicedate >= '" +
                             Utilities.GetDatabaseShortDate(fromDate) + "' and invoicedate <= '" +
                             Utilities.GetDatabaseShortDate(toDate) + "';";
                List<InvoiceModel> invoiceModels = new List<InvoiceModel>();
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
                InvoiceModel invoiceModel = new InvoiceModel();
                while (rdr.Read())
                {
                    invoiceModel = GetModelForRawData(rdr);
                    invoiceModels.Add(invoiceModel);
                }
                mySqlConnectionModel.MySqlConnection.Close();
                if (invoiceModels.Count > 0)
                {
                    invoiceModels = SetCustomerAndProducts(invoiceModels);
                }
                return invoiceModels;
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while getting invoice details");
                return null;
            }
        }
        public void Update(InvoiceModel invoice)
        {
            try
            {
                string updateInvoiceSql = GetUpdateQueryStatement(invoice);
                DbQueryExecuter.WriteToDatabase(updateInvoiceSql);
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while updating product details");
            }
        }

        private static InvoiceModel GetModelForRawData(MySqlDataReader rdr)
        {
            return new InvoiceModel()
            {
                InvoiceId = rdr.GetInt32(0),
                IsBillRequired = rdr.GetInt32(0) == 0,
                Quantity = rdr.GetInt32(1),
                RatePerQuantity = rdr.GetInt32(2),
                Billing = new BillingModel()
                {
                    BillingId = rdr.GetInt32(3)
                },
                Customer = new CustomerModel()
                {
                    CustomerId = rdr.GetInt32(4)
                },
                Product = new ProductModel()
                {
                    Productid = rdr.GetInt32(5)
                },
                AmountWithoutGst = rdr.GetDouble(6),
                Cgst = rdr.GetDouble(7),
                Sgst = rdr.GetDouble(8),
                TotalAmount = rdr.GetDouble(9),
                InvoiceDate = rdr.GetDateTime(10)
                
            };
        }

        private string GetInsertQueryStatement(InvoiceModel invoice, int nextId)
        {
            return "INSERT INTO invoice(invoiceid, quantity, rateperqty, billid, " +
                   "customerid, productid, amountwogst, cgst, sgst, totalamount, " +
                   "invoicedate, createdby, modifiedby, createdate, modifieddate) VALUES( " +
                   nextId + "," +
                   invoice.Quantity + "," +
                   invoice.RatePerQuantity + "," +
                   invoice.Billing.BillingId + "," +
                   invoice.Customer.CustomerId + "," +
                   invoice.Product.Productid + "," +
                   invoice.AmountWithoutGst + "," +
                   invoice.Cgst + "," +
                   invoice.Sgst + "," +
                   invoice.TotalAmount + ",'" +
                   Utilities.GetDatabaseDate(invoice.InvoiceDate) + "','" +
                   _username + "','" +
                   _username + "','" +
                   Utilities.CurrentDBTimeStamp() + "','" +
                   Utilities.CurrentDBTimeStamp() + "');";
        }


        private string GetUpdateQueryStatement(InvoiceModel invoice)
        {
            return
                "UPDATE invoice SET " +
                "rateperqty = " + invoice.RatePerQuantity + "," +
                "billid =" + invoice.Billing.BillingId + "," +
                "customerid =" + invoice.Customer.CustomerId + "," +
                "productid =" + invoice.Product.Productid + "," +
                "amountwogst =" + invoice.AmountWithoutGst + "," +
                "cgst =" + invoice.Cgst + "," +
                "sgst =" + invoice.Sgst + "," +
                "totalamount =" + invoice.TotalAmount + "," +
                "invoicedate =" + Utilities.GetDatabaseDate(invoice.InvoiceDate) + "," +
                "modifiedby = '" +  _username + "'," +
                "modifieddate ='" + Utilities.CurrentDbDate() + "' where " +
                "invoiceid = " + invoice.InvoiceId + ";";
        }
    }
}
