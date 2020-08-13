using Gasware.Database;
using Gasware.Models;
using Gasware.Repository.Interfaces;
using Gasware.Service;
using Gasware.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Gasware.Repository
{
    public class BillingRepository : IBillingRepository
    {
        
        private string _username;
        public BillingRepository(string username)
        {
            _username = username;
        }
        public BillingModel GetBilling(int id)
        {
            string sql = "SELECT * FROM billings where billingid =" + id.ToString();            
            MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
            MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;            
            BillingModel billing = new BillingModel();
            while (rdr.Read())
            {
                billing = GetBillingModelFromRawData(rdr);
            }            
            mySqlConnectionModel.MySqlConnection.Close();
            billing = AssignOtherModels(billing);
            return billing;
        }

        private BillingModel AssignOtherModels(BillingModel billingModel)
        {
            CustomerServiceProvider customerRepository = new CustomerServiceProvider("admin");
            DeliveryPersonService deliveryPersonRepository = new DeliveryPersonService("admin");
            ProductServiceProvider productRepository = new ProductServiceProvider("admin");
            billingModel.Customer = customerRepository.Get(billingModel.Customer.CustomerId);
            billingModel.DeliveryPerson = deliveryPersonRepository.Get(billingModel.DeliveryPerson.DeliveryPersonId);
            billingModel.Product = productRepository.Get(billingModel.Product.Productid);
            return billingModel;
        }

        public List<BillingModel> GetAllBillings()
        {
            string sql = "SELECT * FROM billings";
            MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
            MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
            
            List<BillingModel> billingModels = new List<BillingModel>();
            while (rdr.Read())
            {
                billingModels.Add(GetBillingModelFromRawData(rdr));
            }           
            mySqlConnectionModel.MySqlConnection.Close();    
            return billingModels;
        }

        public List<BillingModel> GetBillingsForDate(DateTime dateTime)
        {
            string dbFromDate = Utilities.GetDatabaseDate(dateTime.AddDays(-1));
            string dbToDate = Utilities.GetDatabaseDate(dateTime);
            string sql = "SELECT * FROM billings where billdate >='" + 
                         dbFromDate + "' and billdate <= '" + dbToDate + "';" ;            
            MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
            MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
            
            List<BillingModel> billingModels = new List<BillingModel>();
            while (rdr.Read())
            {
                billingModels.Add(GetBillingModelFromRawData(rdr));
            }
            mySqlConnectionModel.MySqlConnection.Close();
            return billingModels;
        }

        private static BillingModel GetBillingModelFromRawData(MySqlDataReader rdr)
        {

            return new BillingModel()
            {
                BillingId = rdr.GetInt32(0),
                Customer = new CustomerViewModel()
                {
                    CustomerId = rdr.GetInt32(1)
                },
                DeliveryPerson = new DeliveryPersonViewModel()
                {
                    DeliveryPersonId = rdr.GetInt32(2)
                },
                DeliveredFullCylinderQty = rdr.GetInt32(3),
                BilledAmount = rdr.GetDouble(4),
                Product = new ProductViewModel()
                {
                    Productid = rdr.GetInt32(5)
                },
                PaidAmount = rdr.GetDouble(6),
                ReceivedEmptyCylinderQty = rdr.GetInt32(7),
                BillingDate = rdr.GetDateTime(8),
                Invoice = new InvoiceModel()
                {
                    InvoiceId = rdr.GetInt32(13)
                },
                Rate = rdr.GetDecimal(14),
                Details = rdr.IsDBNull(15) ? string.Empty : rdr.GetString(15)
            };

        }

        public int Create(BillingModel billingModel)
        {
            try
            {
                string _nextBillingIdQuery = "SELECT max(billingid) from billings";
                int nextBillingId = StaticIdProvider.GetNextId(_nextBillingIdQuery);
                string insertSql = GetInsertQueryStatement(billingModel, nextBillingId);
                DailyReportRepository dailyReportRepository = new DailyReportRepository(_username);
                int dailyReportId = dailyReportRepository.AddDailyReportForBill(billingModel);
                if (dailyReportId == 0)
                    return 0;
                DbQueryExecuter.WriteToDatabase(insertSql);
                return nextBillingId;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while creating billing details");
                return 0;
            }
        }

        public int Update(BillingModel billingModel)
        {
            try
            {
                string updateAddressSql = GetUpdateQueryStatement(billingModel, billingModel.BillingId);
                DailyReportRepository _dailyReportRepository = new DailyReportRepository(_username);
                int dailyReportId = _dailyReportRepository.UpdateDailyReportForBill(billingModel);
                if (dailyReportId == 0)
                    return 0;
                DbQueryExecuter.WriteToDatabase(updateAddressSql);
                return billingModel.BillingId;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while updadting billing details");
                return 0;
            }
        }

        public void Delete(int billingId)
        {
            try
            {
                string deleteSql = GetDeleteQueryStatement(billingId);
                BillingModel billingModel = GetBilling(billingId);
                DailyReportRepository _dailyReportRepository = new DailyReportRepository(_username);
                int dailyReportId = _dailyReportRepository.DeleteDailyReportForBill(billingModel);
                if (dailyReportId == 0)
                    return;
                DbQueryExecuter.WriteToDatabase(deleteSql);
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while deleting billing details");
            }
        }

        private string GetInsertQueryStatement(BillingModel billingModel, int nextAddressId)
        {
            return
                 "INSERT INTO billings (" +
                 "billingid, customerid, deliverypersonid, deliveredfullcylinderqty, billedamount, " +
                 "productid, paidamount, receivedemptycylinderqty, billdate, createdate," +
                 "createdby, modifieddate, modifiedby, invoiceid, rate, details " +
                 ") VALUES (" +
                 nextAddressId + "," +
                 billingModel.Customer.CustomerId + "," +
                 billingModel.DeliveryPerson.DeliveryPersonId + "," +
                 billingModel.DeliveredFullCylinderQty + "," +
                 billingModel.BilledAmount + "," +
                 billingModel.Product.Productid + "," +
                 billingModel.PaidAmount + "," +
                 billingModel.ReceivedEmptyCylinderQty + ",'" +
                 Utilities.GetDatabaseDate(billingModel.BillingDate) + "','" +
                 Utilities.CurrentDBTimeStamp() + "','" +
                 _username + "','" +
                 Utilities.CurrentDBTimeStamp() + "','" +
                 _username + "',0," +
                 billingModel.Rate + ",'" +
                 billingModel.Details + "');";
        }

        private string GetUpdateQueryStatement(BillingModel billingModel, int billingid)
        {
            int invoiceId = billingModel.Invoice.InvoiceId;
            return
                 " Update billings set" +
                 " customerid = " + billingModel.Customer.CustomerId +
                 ", deliverypersonid = " + billingModel.DeliveryPerson.DeliveryPersonId +
                 ", deliveredfullcylinderqty = " + billingModel.DeliveredFullCylinderQty +
                 ", productid = " + billingModel.Product.Productid +
                 ", paidamount = " + billingModel.BilledAmount +
                 ", receivedemptycylinderqty = " + billingModel.ReceivedEmptyCylinderQty +
                 ", billdate = '" + Utilities.GetDatabaseDate(billingModel.BillingDate) + 
                 "', modifieddate ='" + Utilities.CurrentDBTimeStamp() +
                 "', modifiedby = '" + _username +
                 "', rate = " + billingModel.Rate +
                 ", invoiceid = " + invoiceId +
                 ", details = '" + billingModel.Details +
                 "' where billingid =" + billingid + ";";
        }

        private string GetDeleteQueryStatement(int billingid)
        {
            return "Delete from billings where billingid =" + billingid + ";";

        }
    }
}
