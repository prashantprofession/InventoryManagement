using Gasware.Database;
using Gasware.Models;
using Gasware.ReportModels;
using Gasware.Repository.Interfaces;
using Gasware.Service;
using Gasware.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Windows;

namespace Gasware.Repository
{
    public class DailyReportRepository : IDailyReportRepository
    {

        private string _username;
        public DailyReportRepository(string username)
        {
            _username = username;
        }


        private static DailyReportModel GetDailyReportModelFromRawData(MySqlDataReader rdr)
        {

            return new DailyReportModel()
            {
                DailyReportId = rdr.GetInt32(0),
                SoldQuantity = rdr.GetInt32(1),
                SoldAmount = rdr.GetDouble(2),
                ReceivedQuantity = rdr.GetInt32(3),
                ReceivedAmount = rdr.GetDouble(4),
                TransactionDate = rdr.GetDateTime(5),
                Balance = rdr.GetDouble(6),
                Product = new ProductViewModel()
                {
                    Productid = rdr.GetInt32(7)
                }
            };

        }


        private string GetInsertQueryStatement(DailyReportModel dailyReportModel, int nextid)
        {
            return
                 "INSERT INTO dailyreport (" +
                 "dailyreportid, soldqty, soldamount, receivedqty, receivedamount, " +
                 "productid, transactiondate, balance, createdate, createdby, " +
                 "modifieddate, modifiedby " +
                 ") VALUES (" +
                 nextid + "," +
                 dailyReportModel.SoldQuantity + "," +
                 dailyReportModel.SoldAmount + "," +
                 dailyReportModel.ReceivedQuantity + "," +
                 dailyReportModel.ReceivedAmount + "," +
                 dailyReportModel.Product.Productid + ",'" +
                 Utilities.GetDatabaseDate(dailyReportModel.TransactionDate) + "'," +
                 dailyReportModel.Balance + ",'" +
                 Utilities.CurrentDBTimeStamp() + "','" +
                 _username + "','" +
                 Utilities.CurrentDBTimeStamp() + "','" +
                 _username + "');";
        }

        private string GetUpdateQueryStatement(DailyReportModel dailyReportModel)
        {
            return
                 " Update dailyreport set" +
                 " productid = " + dailyReportModel.Product.Productid +
                 ", soldqty = " + dailyReportModel.SoldQuantity +
                 ", soldamount = " + dailyReportModel.SoldAmount +
                 ", receivedqty = " + dailyReportModel.ReceivedQuantity +
                 ", receivedamount = " + dailyReportModel.ReceivedAmount +
                 ", balance = '" + dailyReportModel.Balance +
                 "', modifieddate ='" + Utilities.CurrentDBTimeStamp() +
                 "', modifiedby = '" + _username + "' where dailyreportid =" + dailyReportModel.DailyReportId + ";";
        }

        private string GetDeleteQueryStatement(int id)
        {
            return "Delete from dailyreport where dailyreportid =" + id + ";";
        }



        public int Create(DailyReportModel dailyReportModel)
        {
            try
            {
                string _nextDailyReportIdQuery = "SELECT max(dailyreportid) from dailyreport";
                int nextDailyReportId = StaticIdProvider.GetNextId(_nextDailyReportIdQuery);
                string insertSql = GetInsertQueryStatement(dailyReportModel, nextDailyReportId);
                DbQueryExecuter.WriteToDatabase(insertSql);
                return nextDailyReportId;
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while creating daily report details");
                return 0;
            }
        }

        public void Delete(int id)
        {
            try
            {
                string deleteSql = GetDeleteQueryStatement(id);
                DbQueryExecuter.WriteToDatabase(deleteSql);
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while deleting billing details");
            }
        }

        public List<DailyReportModel> GetAllReports()
        {
            string sql = "SELECT * FROM dailyreport";
            MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
            MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;

            List<DailyReportModel> dailyReportModels = new List<DailyReportModel>();
            while (rdr.Read())
            {
                dailyReportModels.Add(GetDailyReportModelFromRawData(rdr));
            }
            dailyReportModels = AssignProductModels(dailyReportModels);
            mySqlConnectionModel.MySqlConnection.Close();
            return dailyReportModels;
        }

        private List<DailyReportModel> AssignProductModels(List<DailyReportModel> dailyReportModels)
        {
            ProductServiceProvider productServiceProvider = new ProductServiceProvider(_username);
            foreach (DailyReportModel dailyReportModel in dailyReportModels)
            {
                dailyReportModel.Product = productServiceProvider.Get(dailyReportModel.Product.Productid);
            }
            return dailyReportModels;
        }



        public DailyReportModel GetDailyReport(int id)
        {
            string sql = "SELECT * FROM dailyreport where dailyreportid =" + id.ToString();
            MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
            MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
            DailyReportModel dailyReportModel = new DailyReportModel();
            while (rdr.Read())
            {
                dailyReportModel = GetDailyReportModelFromRawData(rdr);
            }
            mySqlConnectionModel.MySqlConnection.Close();
            return dailyReportModel;
        }

        public DailyReportModel GetDailyReportByDate(DateTime dateTime, ProductModel product)
        {
            string sql = "SELECT * FROM dailyreport where productid = " + product.Productid +
                " and transactiondate ='" + Utilities.GetDatabaseShortDate(dateTime) + "';";
            MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
            MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
            DailyReportModel dailyReportModel = new DailyReportModel();
            while (rdr.Read())
            {
                dailyReportModel = GetDailyReportModelFromRawData(rdr);
            }
            mySqlConnectionModel.MySqlConnection.Close();
            return dailyReportModel;
        }


        public List<DailyReportModel> GetReportsForDateRange(DateTime fromDate, DateTime toDate, ProductViewModel product)
        {
            string dbFromDate = Utilities.GetDatabaseShortDate(fromDate);
            string dbToDate = Utilities.GetDatabaseShortDate(toDate);
            string additionWhereCondition = product == null ? string.Empty : " and productid = " + product.Productid.ToString();
            string sql = "SELECT * FROM dailyreport where " +
                "transactiondate >='" + dbFromDate + "' " +
                additionWhereCondition +
                " and transactiondate <= '" + dbToDate + "';";
            MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
            MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
            List<DailyReportModel> dailyReportModels = new List<DailyReportModel>();
            while (rdr.Read())
            {
                dailyReportModels.Add(GetDailyReportModelFromRawData(rdr));
            }
            dailyReportModels = AssignProductModels(dailyReportModels);
            mySqlConnectionModel.MySqlConnection.Close();
            return dailyReportModels;
        }

        public List<DailyTransactionReportModel> GetPrintModels(DateTime fromDate, DateTime toDate, ProductViewModel product)
        {
            List<DailyReportModel> dbModels = GetReportsForDateRange(fromDate, toDate, product);
            List<DailyTransactionReportModel> printModels = new List<DailyTransactionReportModel>();
            foreach (DailyReportModel dbModel in dbModels)
            {
                printModels.Add(new DailyTransactionReportModel() 
                { 
                    Balance = dbModel.Balance,
                    DailyReportId = dbModel.DailyReportId,
                    Product= dbModel.Product.Name,
                    ReceivedAmount = dbModel.ReceivedAmount,
                    ReceivedQuantity = dbModel.ReceivedQuantity,
                    SoldAmount = dbModel.SoldAmount,
                    SoldQuantity = dbModel.SoldQuantity,
                    TransactionDate = dbModel.TransactionDate
                });
            }
            return printModels;
        }

        public int Update(DailyReportModel dailyReportModel)
        {
            try
            {
                string updateAddressSql = GetUpdateQueryStatement(dailyReportModel);
                DbQueryExecuter.WriteToDatabase(updateAddressSql);
                return dailyReportModel.DailyReportId;
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while updadting daily report details");
                return 0;
            }
        }

        private DailyReportModel GetDailyReportModelByProductAndDate(int productid, DateTime date)
        {
            string sql = "SELECT * FROM dailyreport where transactiondate = '" +
                         Utilities.GetDatabaseShortDate(date) + "' and productid = " +
                         productid + ";";
            MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
            MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
            DailyReportModel dailyReportModel = new DailyReportModel();
            while (rdr.Read())
            {
                dailyReportModel = GetDailyReportModelFromRawData(rdr);
            }
            mySqlConnectionModel.MySqlConnection.Close();
            return dailyReportModel;
        }

        public int AddDailyReportForBill(BillingModel billingViewModel)
        {
            DailyReportModel prevDailyReportModel = GetDailyReportModelByProductAndDate(billingViewModel.Product.Productid, billingViewModel.BillingDate.AddDays(-1));
            DailyReportModel dailyReportModel = GetDailyReportModelByProductAndDate(billingViewModel.Product.Productid, billingViewModel.BillingDate);
            // dailyReportModel.Balance = prevDailyReportModel.Balance - billingViewModel.DeliveredFullCylinderQty;            

            if (dailyReportModel.DailyReportId == 0 || dailyReportModel == null)
            {
                if (prevDailyReportModel.DailyReportId == 0 || prevDailyReportModel == null)
                {
                    var result = MessageBox.Show("Product " + billingViewModel.Product.Name +
                              " is out of stock.Continuing might corrupt the data." +
                              " Are you sure you want to continue?", "Question",
                              MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result.ToString().Equals("Yes"))
                    {
                        dailyReportModel = AssignDailyReportDetailsForBillingModel(dailyReportModel, billingViewModel);
                        //dailyReportModel.Balance -= billingViewModel.DeliveredFullCylinderQty;
                        //dailyReportModel.SoldAmount = billingViewModel.BilledAmount;
                        //dailyReportModel.SoldQuantity = billingViewModel.DeliveredFullCylinderQty;
                        //dailyReportModel.ReceivedAmount = billingViewModel.PaidAmount;
                        //dailyReportModel.ReceivedQuantity = billingViewModel.ReceivedEmptyCylinderQty;
                        //dailyReportModel.Product = billingViewModel.Product;
                        //dailyReportModel.TransactionDate = billingViewModel.BillingDate;
                        return Create(dailyReportModel);
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    dailyReportModel = AssignDailyReportDetailsForBillingModel(dailyReportModel, billingViewModel);
                    dailyReportModel.Balance = prevDailyReportModel.Balance - billingViewModel.DeliveredFullCylinderQty;
                    //dailyReportModel.SoldAmount = billingViewModel.BilledAmount;
                    //dailyReportModel.SoldQuantity = billingViewModel.DeliveredFullCylinderQty;
                    //dailyReportModel.ReceivedAmount = billingViewModel.PaidAmount;
                    //dailyReportModel.ReceivedQuantity = billingViewModel.ReceivedEmptyCylinderQty;
                    //dailyReportModel.Product = billingViewModel.Product;
                    //dailyReportModel.TransactionDate = billingViewModel.BillingDate;
                    return Create(dailyReportModel);
                }
            }
            else
            {
                dailyReportModel = AssignDailyReportDetailsForBillingModel(dailyReportModel, billingViewModel);
                dailyReportModel.Balance -= billingViewModel.DeliveredFullCylinderQty;
                //dailyReportModel.SoldAmount += billingViewModel.BilledAmount;
                //dailyReportModel.SoldQuantity += billingViewModel.DeliveredFullCylinderQty;
                //dailyReportModel.ReceivedAmount += billingViewModel.PaidAmount;
                //dailyReportModel.ReceivedQuantity += billingViewModel.ReceivedEmptyCylinderQty;
                //dailyReportModel.Product = billingViewModel.Product;
                //dailyReportModel.TransactionDate = billingViewModel.BillingDate;
                return Update(dailyReportModel);
            }
        }

        private static DailyReportModel AssignDailyReportDetailsForBillingModel(
            DailyReportModel dailyReportModel, BillingModel billingViewModel)
        {
            dailyReportModel.SoldAmount += billingViewModel.BilledAmount;
            dailyReportModel.SoldQuantity += billingViewModel.DeliveredFullCylinderQty;
            dailyReportModel.ReceivedAmount += billingViewModel.PaidAmount;
            dailyReportModel.ReceivedQuantity += billingViewModel.ReceivedEmptyCylinderQty;
            dailyReportModel.Product = billingViewModel.Product;
            dailyReportModel.TransactionDate = billingViewModel.BillingDate;
            return dailyReportModel;
        }

        public int UpdateDailyReportForBill(BillingModel billingViewModel)
        {
            DailyReportModel prevDailyReportModel = GetDailyReportModelByProductAndDate(billingViewModel.Product.Productid, billingViewModel.BillingDate.AddDays(-1));
            DailyReportModel dailyReportModel = GetDailyReportModelByProductAndDate(billingViewModel.Product.Productid, billingViewModel.BillingDate);

            BillingRepository billingRepository = new BillingRepository(_username);
            List<BillingModel> todaysBillings = billingRepository.GetBillingsForDate(
                        billingViewModel.BillingDate).Where(x =>
                        x.Product == billingViewModel.Product).ToList();
            dailyReportModel.Product = billingViewModel.Product;

            dailyReportModel.TransactionDate = billingViewModel.BillingDate;
            dailyReportModel.Balance = prevDailyReportModel.Balance;
            foreach (BillingModel billing in todaysBillings)
            {
                dailyReportModel = AssignDailyReportDetailsForBillingModel(dailyReportModel, billing);
                dailyReportModel.Balance -= billingViewModel.DeliveredFullCylinderQty;
                //dailyReportModel.SoldAmount += billingViewModel.BilledAmount;
                //dailyReportModel.SoldQuantity += billingViewModel.DeliveredFullCylinderQty;
                //dailyReportModel.ReceivedAmount += billingViewModel.PaidAmount;
                //dailyReportModel.ReceivedQuantity += billingViewModel.ReceivedEmptyCylinderQty;              
            }

            StockEntryRepository stockEntryRepository = new StockEntryRepository(_username);
            List<StockEntryModel> stocks = stockEntryRepository.GetStockEntries().Where(x =>
                            x.ReceivedDate == billingViewModel.BillingDate &&
                            x.Product.Productid == billingViewModel.Product.Productid).ToList();
            foreach (StockEntryModel stockEntry in stocks)
            {
                dailyReportModel.Balance += stockEntry.Quantity;
            }
            return Update(dailyReportModel);
        }

        public int DeleteDailyReportForBill(BillingModel billingViewModel)
        {
            DailyReportModel prevDailyReportModel = GetDailyReportModelByProductAndDate(billingViewModel.Product.Productid, billingViewModel.BillingDate.AddDays(-1));
            DailyReportModel dailyReportModel = GetDailyReportModelByProductAndDate(billingViewModel.Product.Productid, billingViewModel.BillingDate);
            //            dailyReportModel.Balance = prevDailyReportModel.Balance - billingViewModel.DeliveredFullCylinderQty;
            dailyReportModel.Balance -= billingViewModel.DeliveredFullCylinderQty;
            if (dailyReportModel.DailyReportId == 0 || dailyReportModel == null)
            {
                var result = MessageBox.Show("Product " + billingViewModel.Product.Name +
                          "is out of stock.Continuing might corrupt the data." +
                          " Are you sure you want to continue?", "Question",
                          MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result.ToString().Equals("Yes"))
                {
                    dailyReportModel.SoldAmount -= billingViewModel.BilledAmount;
                    dailyReportModel.SoldQuantity -= billingViewModel.DeliveredFullCylinderQty;
                    dailyReportModel.ReceivedAmount -= billingViewModel.PaidAmount;
                    dailyReportModel.ReceivedQuantity -= billingViewModel.ReceivedEmptyCylinderQty;
                    dailyReportModel.Product = billingViewModel.Product;
                    dailyReportModel.TransactionDate = billingViewModel.BillingDate;
                    return Update(dailyReportModel);
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                dailyReportModel.SoldAmount -= billingViewModel.BilledAmount;
                dailyReportModel.SoldQuantity -= billingViewModel.DeliveredFullCylinderQty;
                dailyReportModel.ReceivedAmount -= billingViewModel.PaidAmount;
                dailyReportModel.ReceivedQuantity -= billingViewModel.ReceivedEmptyCylinderQty;
                return Update(dailyReportModel);
            }
        }

        public int AddDailyReportForStock(StockEntryModel stockEntryModel)
        {
            DailyReportModel prevDailyReportModel = GetDailyReportModelByProductAndDate(
                stockEntryModel.Product.Productid, stockEntryModel.ReceivedDate.AddDays(-1));
            DailyReportModel dailyReportModel = GetDailyReportModelByProductAndDate(
                stockEntryModel.Product.Productid, stockEntryModel.ReceivedDate);
            ProductServiceProvider productService = new ProductServiceProvider(_username);
            if (dailyReportModel.DailyReportId < 2)
            {
                DailyReportModel newDailyReportModel = new DailyReportModel()
                {
                    DailyReportId = 0,
                    SoldAmount = 0,
                    SoldQuantity = 0,
                    ReceivedAmount = 0,
                    ReceivedQuantity = 0,
                    Balance = stockEntryModel.Quantity,
                    Product = productService.GetViewModel(stockEntryModel.Product),
                    TransactionDate = DateTime.Now
                };
                return Create(newDailyReportModel);
            }
            else
            {
                dailyReportModel.Balance += stockEntryModel.Quantity;
                Update(dailyReportModel);
                return dailyReportModel.DailyReportId;
            }
        }

        public int DeleteDailyReportForStock(StockEntryModel stockEntryModel)
        {
            DailyReportModel prevDailyReportModel = GetDailyReportModelByProductAndDate(
                stockEntryModel.Product.Productid, stockEntryModel.ReceivedDate.AddDays(-1));
            DailyReportModel dailyReportModel = GetDailyReportModelByProductAndDate(
                stockEntryModel.Product.Productid, stockEntryModel.ReceivedDate);
            ProductServiceProvider productService = new ProductServiceProvider(_username);
            if (dailyReportModel.DailyReportId == 1)
            {
                DailyReportModel newDailyReportModel = new DailyReportModel()
                {
                    SoldAmount = 0,
                    SoldQuantity = 0,
                    ReceivedAmount = 0,
                    ReceivedQuantity = 0,
                    Balance = stockEntryModel.Quantity,
                    Product = productService.GetViewModel(stockEntryModel.Product),
                    TransactionDate = DateTime.Now
                };
                Delete(newDailyReportModel.DailyReportId);
                return newDailyReportModel.DailyReportId;
            }
            else
            {
                dailyReportModel.Balance -= stockEntryModel.Quantity;
                Update(dailyReportModel);
                return dailyReportModel.DailyReportId;
            }
        }

        public int UpdateDailyReportForStock(StockEntryChangesModel stockEntryChanges)
        {
            ProductServiceProvider productService = new ProductServiceProvider(_username);
            DailyReportModel existingDailyReportModel = new DailyReportModel();
            if (stockEntryChanges.IsReceivedDateChanged)
            {
                int noOfDayChanged = (int)stockEntryChanges.OldReceivedDate.Subtract(
                                    stockEntryChanges.NewReceivedDate).TotalDays;
                List<DailyReportModel> dailyReportModelToUpdates = new List<DailyReportModel>();
                if (noOfDayChanged < 0)
                {
                    dailyReportModelToUpdates = GetReportsForDateRange(stockEntryChanges.OldReceivedDate,
                                            stockEntryChanges.NewReceivedDate, null);
                    foreach (DailyReportModel dailyReportModel in dailyReportModelToUpdates)
                    {
                        dailyReportModel.Balance -= stockEntryChanges.NewQuantity;
                        Update(dailyReportModel);
                    }
                }
                else
                {
                    dailyReportModelToUpdates = GetReportsForDateRange(stockEntryChanges.NewReceivedDate,
                                            stockEntryChanges.OldReceivedDate, null);
                    foreach (DailyReportModel dailyReportModel in dailyReportModelToUpdates)
                    {
                        dailyReportModel.Balance += stockEntryChanges.NewQuantity;
                        Update(dailyReportModel);
                    }
                }
                DailyReportModel newDailyReportModel = GetDailyReportByDate(stockEntryChanges.NewReceivedDate,
                                                                            stockEntryChanges.NewProduct);
                newDailyReportModel.TransactionDate = stockEntryChanges.NewReceivedDate;
                newDailyReportModel.Balance = stockEntryChanges.NewQuantity;
                newDailyReportModel.Product = productService.GetViewModel(
                                            stockEntryChanges.NewProduct);
                return newDailyReportModel.DailyReportId == 0 ? Create(newDailyReportModel) :
                                                                Update(newDailyReportModel);
                //if (newDailyReportModel.DailyReportId == 0)
                //{
                //    //GetDailyReportModelForStockUpdate(stockEntryChanges, productService);
                //    newDailyReportModel.TransactionDate = stockEntryChanges.NewReceivedDate;
                //    newDailyReportModel.Balance = stockEntryChanges.NewQuantity;
                //    newDailyReportModel.Product = productService.GetViewModel(
                //                                stockEntryChanges.NewProduct);
                //    return Create(newDailyReportModel);
                //}
                //else
                //{

                //}

            }
            else
            {
                existingDailyReportModel = GetDailyReportModelForStockUpdate(stockEntryChanges, productService);
                return Update(existingDailyReportModel);
            }
        }




        private DailyReportModel GetDailyReportModelForStockUpdate(
                        StockEntryChangesModel stockEntryChanges,
                        ProductServiceProvider productService)
        {
            DailyReportModel existingDailyReportModel = GetDailyReportModelByProductAndDate(
                           stockEntryChanges.OldProduct.Productid, stockEntryChanges.OldReceivedDate);            
            if (stockEntryChanges.IsProductChanged)
            {
                existingDailyReportModel.Product = productService.GetViewModel(
                                                stockEntryChanges.NewProduct);
            }
            if (stockEntryChanges.IsQuantityChanged)
            {
                existingDailyReportModel.Balance += stockEntryChanges.NewQuantity -
                                                    stockEntryChanges.OldQuantity;
            }
            if (stockEntryChanges.IsReceivedDateChanged)
            {
                existingDailyReportModel.TransactionDate = stockEntryChanges.NewReceivedDate;
            }
            return existingDailyReportModel;
        }

        public int UpdateDailyReportForStock(StockEntryModel stockEntryModel)
        {
            DailyReportModel prevDailyReportModel = GetDailyReportModelByProductAndDate(
                stockEntryModel.Product.Productid, stockEntryModel.ReceivedDate.AddDays(-1));
            DailyReportModel dailyReportModel = GetDailyReportModelByProductAndDate(
                stockEntryModel.Product.Productid, stockEntryModel.ReceivedDate);

            StockEntryRepository stockRepository = new StockEntryRepository(_username);
            List<StockEntryModel> todaysStocks = stockRepository.GetStockEntriesByDate(
                                stockEntryModel.ReceivedDate);
            ProductServiceProvider productServiceProvider = new ProductServiceProvider(_username);

            dailyReportModel.Product = productServiceProvider.GetViewModel(stockEntryModel.Product);
            dailyReportModel.TransactionDate = stockEntryModel.ReceivedDate;
            dailyReportModel.Balance = prevDailyReportModel.Balance + stockEntryModel.Quantity;
            foreach (StockEntryModel stock in todaysStocks)
            {
                dailyReportModel.Balance += stock.Quantity;
            }

            BillingRepository billingRepository = new BillingRepository(_username);
            List<BillingModel> billingModels = billingRepository.GetBillingsForDate(stockEntryModel.ReceivedDate);
            foreach (BillingModel billing in billingModels)
            {
                dailyReportModel.Balance -= billing.DeliveredFullCylinderQty;
            }
            if (dailyReportModel.DailyReportId == 0)
                return Create(dailyReportModel);
            return Update(dailyReportModel);

        }
    }
}
