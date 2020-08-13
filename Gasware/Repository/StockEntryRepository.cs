using Gasware.Database;
using Gasware.Models;
using Gasware.ReportModels;
using Gasware.Repository.Interfaces;
using Gasware.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Gasware.Repository
{
    public class StockEntryRepository : IStockEntryRepository
    {
        private string _username;
        public StockEntryRepository(string username)
        {
            _username = username;
        }
        public StockEntryModel Get(int id)
        {
            try
            {
                string sql = "SELECT * FROM stockentry where stockentryid=" + id.ToString();
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;

                StockEntryModel stockEntry = new StockEntryModel();
                while (rdr.Read())
                {
                    stockEntry = GetModelForRawData(rdr);
                }
                mySqlConnectionModel.MySqlConnection.Close();
                if (stockEntry.StockEntryId == 0)
                {
                    MessageBox.Show("No stock yet.Atleast one stock is needed to use it.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
                stockEntry.Product = GetProduct(stockEntry.Product.Productid);
                stockEntry.ReceivedBy = GetDeliveryPerson(stockEntry.ReceivedBy.DeliveryPersonId);
                return stockEntry;
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while getting stock details");
                return null;
            }
        }

        public StockEntryModel GetLatestStockForProduct(ProductViewModel productView)
        {
            try
            {
                string sql = "SELECT * FROM stockentry  where productid = " + productView.Productid +
                                            " order by stockentryid desc limit 1 ;";
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;

                StockEntryModel stockEntry = new StockEntryModel();
                while (rdr.Read())
                {
                    stockEntry = GetModelForRawData(rdr);
                }
                mySqlConnectionModel.MySqlConnection.Close();
                if (stockEntry.StockEntryId == 0)
                {
                    MessageBox.Show("No stock yet.Atleast one stock is needed to use it.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
                stockEntry.Product = GetProduct(stockEntry.Product.Productid);
                stockEntry.ReceivedBy = GetDeliveryPerson(stockEntry.ReceivedBy.DeliveryPersonId);
                return stockEntry;
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while getting stock details");
                return null;
            }
        }

        public List<StockEntryModel> GetStockEntries()
        {
            try
            {
                string sql = "SELECT * FROM stockentry";
                List<StockEntryModel> stockEntryModels = new List<StockEntryModel>();               
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
                StockEntryModel stock = new StockEntryModel();
                while (rdr.Read())
                {
                    stock = GetModelForRawData(rdr);
                    stockEntryModels.Add(stock);
                }

                mySqlConnectionModel.MySqlConnection.Close();
                foreach (StockEntryModel stockEntry in stockEntryModels)
                {
                    stockEntry.Product = GetProduct(stockEntry.Product.Productid);
                    stockEntry.ReceivedBy = GetDeliveryPerson(stockEntry.ReceivedBy.DeliveryPersonId);
                }
                return stockEntryModels;
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while getting stock details");
                return null;
            }
        }

        public List<StockEntryReportModel> GetStockEntryReportModels()
        {
            List<StockEntryReportModel> stockEntryReportModels = new List<StockEntryReportModel>();
            List<StockEntryModel> stockEntryModels = GetStockEntries();
            foreach (StockEntryModel  stockEntryModel in  stockEntryModels )
            {
                stockEntryReportModels.Add(new StockEntryReportModel()
                {
                    Balance= stockEntryModel.Balance,
                    BilledAmount = stockEntryModel.BilledAmount,
                    CgstPaid = stockEntryModel.CgstPaid,
                    PaidAmount = stockEntryModel.PaidAmount, 
                    Product = stockEntryModel.Product.Name,
                    Quantity = stockEntryModel.Quantity,
                    ReceivedBy = stockEntryModel.ReceivedBy.Name,
                    ReceivedDate = stockEntryModel.ReceivedDate,
                    SgstPaid = stockEntryModel.SgstPaid,
                    StockEntryId = stockEntryModel.StockEntryId,
                    UnitRate    = stockEntryModel.UnitRate                   
                });
            }
            return stockEntryReportModels;
        }
        public List<StockEntryModel> GetStockEntriesByDate(DateTime dateTime)
        {
            try
            {
                string sql = "SELECT * FROM stockentry where receiveddate = '" +
                    Utilities.GetDatabaseDate(dateTime) + "';" ;
                List<StockEntryModel> stockEntryModels = new List<StockEntryModel>();
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
                StockEntryModel stock = new StockEntryModel();
                while (rdr.Read())
                {
                    stock = GetModelForRawData(rdr);
                    stockEntryModels.Add(stock);
                }

                mySqlConnectionModel.MySqlConnection.Close();
                foreach (StockEntryModel stockEntry in stockEntryModels)
                {
                    stockEntry.Product = GetProduct(stockEntry.Product.Productid);
                    stockEntry.ReceivedBy = GetDeliveryPerson(stockEntry.ReceivedBy.DeliveryPersonId);
                }
                return stockEntryModels;
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while getting stock details");
                return new List<StockEntryModel>();
            }
        }


        private ProductModel GetProduct(int id)
        {
            try
            {
                ProductRepository productRepository = new ProductRepository(_username);
                return productRepository.Get(id);
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while getting product details");
                return null;
            }

        }

        private DeliveryPersonModel GetDeliveryPerson(int id)
        {
            DeliveryPersonRepository receivedByRepository = new DeliveryPersonRepository(_username);
            return receivedByRepository.Get(id);
        }

        private static StockEntryModel GetModelForRawData(MySqlDataReader rdr)
        {
            return new StockEntryModel()
            {
                StockEntryId = rdr.GetInt32(0),
                ReceivedDate = rdr.GetDateTime(1),
                Product = new ProductModel()
                {
                    Productid = rdr.GetInt32(2)
                },
                Quantity = rdr.GetInt32(3),
                ReceivedBy = new DeliveryPersonModel()
                {
                    DeliveryPersonId = rdr.GetInt32(4)
                },
                UnitRate = rdr.GetDecimal(9),
                PaidAmount = rdr.GetDouble(10),
                CgstPaid = rdr.GetDecimal(11), 
                SgstPaid = rdr.GetDecimal(12),
                Balance = rdr.GetDouble(13),
                BilledAmount = rdr.GetDouble(14)
            };
        }

        public void Update(StockEntryModel oldStockEntryModel, StockEntryModel newStockEntry)
        {
            StockEntryChangesModel stockChanges = new StockEntryChangesModel()
            {
                IsProductChanged = oldStockEntryModel.Product.Productid != newStockEntry.Product.Productid,
                IsQuantityChanged = oldStockEntryModel.Quantity != newStockEntry.Quantity,
                IsReceivedByChanged = oldStockEntryModel.ReceivedBy.DeliveryPersonId != newStockEntry.ReceivedBy.DeliveryPersonId,
                IsReceivedDateChanged = oldStockEntryModel.ReceivedDate != newStockEntry.ReceivedDate,
                NewProduct = newStockEntry.Product,
                OldProduct = oldStockEntryModel.Product,
                NewQuantity = newStockEntry.Quantity,
                OldQuantity = oldStockEntryModel.Quantity,
                NewReceivedBy = newStockEntry.ReceivedBy,
                OldReceivedBy = oldStockEntryModel.ReceivedBy,
                NewReceivedDate = newStockEntry.ReceivedDate,
                OldReceivedDate = oldStockEntryModel.ReceivedDate,
                StockEntryId = oldStockEntryModel.StockEntryId
            };
            string updateStockSql = GetUpdateQueryStatement(newStockEntry);
            DbQueryExecuter.WriteToDatabase(updateStockSql);
            DailyReportRepository dailyReportRepository = new DailyReportRepository(_username);
            int stockId = dailyReportRepository.UpdateDailyReportForStock(stockChanges);
            if (stockId == 0)
                return;
        }

        public void Update(StockEntryModel stockEntryModel)
        {
            try
            {
                string updateStockSql = GetUpdateQueryStatement(stockEntryModel);                 
                DbQueryExecuter.WriteToDatabase(updateStockSql);
                DailyReportRepository dailyReportRepository = new DailyReportRepository(_username);
                int stockId = dailyReportRepository.UpdateDailyReportForStock(stockEntryModel);
                if (stockId == 0)
                    return;
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while updating stock details");
            }
        }

        public int Create(StockEntryModel stock)
        {
            try
            {
                string _nextBillingIdQuery = "SELECT max(stockentryid) from stockentry";
                int nextId = StaticIdProvider.GetNextId(_nextBillingIdQuery);
                string insertStockSql = GetInsertQueryStatement(stock, nextId);
                DailyReportRepository dailyReportRepository = new DailyReportRepository(_username);
                int stockId = dailyReportRepository.UpdateDailyReportForStock(stock);
                if (stockId == 0)
                    return 0;
                DbQueryExecuter.WriteToDatabase(insertStockSql);
                return nextId;
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while adding stock details");
                return 0;
            }
        }

        public void Delete(int id)
        {
            try
            {
                StockEntryModel stockEntryModel = Get(id);
                DailyReportRepository dailyReportRepository = new DailyReportRepository(_username);
                int stockId = dailyReportRepository.DeleteDailyReportForStock(stockEntryModel);
                if (stockId == 0)
                    return;
                string deleteQuery = "delete from stockentry where stockentryid = " + id + ";";
                DbQueryExecuter.WriteToDatabase(deleteQuery);
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while deleting stock entry");
            }
        }


        private string GetInsertQueryStatement(StockEntryModel stockEntryModel, int nextId)
        {
            string dateTime = Utilities.GetDatabaseDate(DateTime.Now);
            return
                "INSERT INTO stockentry (stockentryid, receiveddate, productid, " +
                "quantity, receivedbyid, createdate, createdby, modifieddate, " +
                "modifiedby, unitrate, paidamount, cgstpaid, sgstpaid, balance," +
                " billedamount ) VALUES (" +
                nextId +
                ", '" + Utilities.GetDatabaseDate(stockEntryModel.ReceivedDate) +
                "'," + stockEntryModel.Product.Productid +
                "," + stockEntryModel.Quantity +
                "," + stockEntryModel.ReceivedBy.DeliveryPersonId +
                ",'" + dateTime +
                "','" + _username +
                "','" + dateTime +
                "','" + _username + "'," +
                stockEntryModel.UnitRate + "," +
                stockEntryModel.PaidAmount + "," +
                stockEntryModel.CgstPaid + "," +
                stockEntryModel.SgstPaid + "," +
                stockEntryModel.Balance + "," +
                stockEntryModel.BilledAmount + 
                ");";
        }

        private string GetUpdateQueryStatement(StockEntryModel stockEntryModel)
        {
            return
                "UPDATE stockentry SET " +
                "receiveddate = '" + Utilities.GetDatabaseDate(stockEntryModel.ReceivedDate) + "'," +
                "productid = " + stockEntryModel.Product.Productid + "," +
                "quantity = " + stockEntryModel.Quantity + "," +
                "receivedbyid = " + stockEntryModel.ReceivedBy.DeliveryPersonId + "," +
                "unitrate = " + stockEntryModel.UnitRate + "," +
                "paidamount = " + stockEntryModel.PaidAmount + "," +
                "cgstpaid = " + stockEntryModel.CgstPaid + "," +
                "sgstpaid = " + stockEntryModel.SgstPaid + "," +
                "balance = " + stockEntryModel.Balance + "," +
                "billedamount = " + stockEntryModel.BilledAmount + "," +
                "modifiedby = '" + _username + "'," +
                "modifieddate = '" + Utilities.GetDatabaseDate(stockEntryModel.ReceivedDate) + "' " +
                "WHERE stockentryid =" + stockEntryModel.StockEntryId + ";";
        }
    }
}
