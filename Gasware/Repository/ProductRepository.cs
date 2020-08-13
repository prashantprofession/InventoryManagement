using Gasware.Database;
using Gasware.Models;
using Gasware.Repository.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Gasware.Repository
{
    public class ProductRepository : IProductRepository
    {
        private string _username;
        public ProductRepository(string username)
        {
            _username = username;
        }
        public ProductModel Get(int id)
        {
            try
            {
                string sql = "SELECT * FROM products where deactivatedate is null and productid=" + id.ToString();                
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;

                ProductModel product = new ProductModel();
                while (rdr.Read())
                {
                    product = GetModelForRawData(rdr);
                }

                mySqlConnectionModel.MySqlConnection.Close();
                if (product.Productid == 0)
                {
                    Utilities.ErrorMessage("No product yet.Atleast one active product is needed to use it.");
                    return null;
                }
                return product;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while getting product details");
                return null;
            }
        }

        public ProductModel GetProductByName(string name)
        {
            try
            {
                string sql = "SELECT * FROM products where deactivatedate is null and name='" + 
                    name + "';";
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;

                ProductModel product = new ProductModel();
                while (rdr.Read())
                {
                    product = GetModelForRawData(rdr);
                }

                mySqlConnectionModel.MySqlConnection.Close();
                if (product.Productid == 0)
                {
                  //  Utilities.ErrorMessage("No product yet.Atleast one active product is needed to use it.");
                    return null;
                }
                return product;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while getting product details");
                return null;
            }
        }

        public List<ProductModel> GetProducts()
        {
            try
            {
                string sql = "SELECT * FROM products where deactivatedate is null";
                List<ProductModel> productModels = new List<ProductModel>();
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;

                ProductModel productModel = new ProductModel();
                while (rdr.Read())
                {
                    productModel = GetModelForRawData(rdr);
                    productModels.Add(productModel);
                }

                mySqlConnectionModel.MySqlConnection.Close();
                return productModels;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while getting product details");
                return null;
            }
        }

        private static ProductModel GetModelForRawData(MySqlDataReader rdr)
        {
            return new ProductModel()
            {
                Productid = rdr.GetInt32(0),
                Weight = rdr.IsDBNull(1) ? 0 : rdr.GetInt32(1),
                Name = rdr.IsDBNull(2) ? string.Empty :  rdr.GetString(2),
                Details = rdr.IsDBNull(3) ? string.Empty : rdr.GetString(3),
                UnitRate = rdr.IsDBNull(5) ? 0 : rdr.GetDecimal(5),
                CGstRate = rdr.IsDBNull(6) ? 0 : rdr.GetDecimal(6),
                SGstRate = rdr.IsDBNull(7) ? 0 : rdr.GetDecimal(7),
                IsBillable = rdr.IsDBNull(8) ? true: rdr.GetBoolean(8),
                IsExpense = rdr.IsDBNull(9) ? false : rdr.GetBoolean(9)
            };
        }

        public void Update(ProductModel productModel)
        {
            try
            {
                string updateProductSql = GetUpdateQueryStatement(productModel);
                DbQueryExecuter.WriteToDatabase(updateProductSql);
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while updating product details");
            }
        }

        public int Create(ProductModel productModel)
        {
            try
            {
                if (GetProductByName(productModel.Name) != null)
                {
                    Utilities.ErrorMessage("Duplicate product name. Retry with different name.");
                    return 0;
                }
                string _nextBillingIdQuery = "SELECT max(productid) from products";
                int nextId = StaticIdProvider.GetNextId(_nextBillingIdQuery);
                string insertAddressSql = GetInsertQueryStatement(productModel, nextId);
                DbQueryExecuter.WriteToDatabase(insertAddressSql);
                return nextId;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while adding product details");
                return 0;
            }
        }

        public void Delete(int id)
        {
            try
            {
                string deleteQuery = "Update products set deactivatedate = '" + Utilities.GetDatabaseDate(DateTime.Now) +
                    "' where productid=" + id + ";";
                DbQueryExecuter.WriteToDatabase(deleteQuery);
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while deleting product details");
            }
        }


        private string GetInsertQueryStatement(ProductModel productModel, int nextId)
        {
            return
                "INSERT INTO products (productid, weight, name, details, unitrate," +
                "cgstrate, sgstrate, isbillable, isexpense) VALUES (" + nextId +
                ", " + productModel.Weight +
                ",'" + productModel.Name +
                "','" + productModel.Details +
                "'," + productModel.UnitRate +
                "," + productModel.CGstRate +
                "," + productModel.SGstRate +
                "," + Convert.ToInt16(productModel.IsBillable) +
                "," + Convert.ToInt16(productModel.IsExpense) +
                ");";
        }

        private string GetUpdateQueryStatement(ProductModel productModel)
        {
            return
                "UPDATE products SET weight = " + productModel.Weight +
                ", name ='" + productModel.Name +
                "', details = '" + productModel.Details +
                "', unitrate =" + productModel.UnitRate +
                ", cgstrate = " + productModel.CGstRate +
                ", sgstrate =" + productModel.SGstRate +
                ", isbillable = " + Convert.ToInt16(productModel.IsBillable) +
                ", isexpense = " + Convert.ToInt16(productModel.IsExpense) +
                " WHERE productid =" + productModel.Productid + ";";
        }
    }
}
