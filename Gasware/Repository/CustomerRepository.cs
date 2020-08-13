using Gasware.Database;
using Gasware.Models;
using Gasware.Repository.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Gasware.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private string _username;
        private IAddressRepository _addressRepository;
        public CustomerRepository(string username)
        {
            _username = username;
            _addressRepository = new AddressRepository(_username);
        }
        public CustomerModel Get(int id)
        {
            try
            {
                string sql = "SELECT * FROM customer where deactivatedate is null and customerid=" + id.ToString();              
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;

                CustomerModel customerModel = new CustomerModel();
                while (rdr.Read())
                {
                    customerModel = GetModelForRawData(rdr);
                }
                mySqlConnectionModel.MySqlConnection.Close();

                if (customerModel.CustomerId == 0)
                {                    
                    return null;
                }

                customerModel.Address = _addressRepository.Get(customerModel.Address.AddressId);
                return customerModel;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while getting customer details");
                return null;
            }
        }

        public CustomerModel GetCustomerByName(string name)
        {
            try
            {
                string sql = "SELECT * FROM customer where deactivatedate is null and name='" + name + "';";
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;

                CustomerModel customerModel = new CustomerModel();
                while (rdr.Read())
                {
                    customerModel = GetModelForRawData(rdr);
                }
                mySqlConnectionModel.MySqlConnection.Close();

                if (customerModel.CustomerId == 0)
                {
                   // Utilities.ErrorMessage("No customer yet.Atleast one active customer is needed.");
                    return null;
                }

                customerModel.Address = _addressRepository.Get(customerModel.Address.AddressId);
                return customerModel;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while getting customer details");
                return null;
            }
        }

        public List<CustomerModel> GetCustomers()
        {
            try
            {
                string sql = "SELECT * FROM customer where deactivatedate is null";
                List<CustomerModel> customerModels = new List<CustomerModel>();
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;

                while (rdr.Read())
                {
                    customerModels.Add(GetModelForRawData(rdr));
                }
                mySqlConnectionModel.MySqlConnection.Close();
                foreach (CustomerModel local in customerModels)
                {
                    local.Address = _addressRepository.Get(local.Address.AddressId);
                }
                return customerModels;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while getting customer details");
                return null;
            }
        }


        private static CustomerModel GetModelForRawData(MySqlDataReader rdr)
        {
            return new CustomerModel()
            {
                CustomerId = rdr.GetInt32(0),
                Name = rdr.GetString(1),
                Address = new AddressModel()
                {
                    AddressId = rdr.GetInt32(2)
                },
                PhoneNumber = rdr.GetString(3),
                Location = rdr.GetString(4),
                Price = rdr.GetDouble(5),
                DiscountPercentage = rdr.GetInt32(6),
                DiscountFlat = rdr.GetInt32(7),
                DepositAmount = rdr.GetDouble(8),
                GstNumber = rdr.IsDBNull(14)? string.Empty : rdr.GetString(14),
                EmailId = rdr.IsDBNull(15) ? string.Empty : rdr.GetString(15),
            };
        }

        public void Update(CustomerModel customerModel)
        {
            try
            {
                CustomerModel existingCustomer = GetCustomerByName(customerModel.Name);
                if (existingCustomer != null && existingCustomer.CustomerId != customerModel.CustomerId)
                {
                     throw new InvalidOperationException("Duplicate customer name.Retry with different name");                    
                }
                string updateCustomerSql = GetUpdateQueryStatement(customerModel);
                _addressRepository.Update(customerModel.Address);
                DbQueryExecuter.WriteToDatabase(updateCustomerSql);
            }
            catch (Exception ex)
            {
                Utilities.ErrorMessage("Error occured while updating customer details." + Environment.NewLine +
                           ex.Message );
            }
        }

        public void Delete(CustomerModel customerModel)
        {
            try
            {
                _addressRepository.Delete(customerModel.Address.AddressId);
                string deleteCustomerSql = "Update customer set deactivatedate = '" +
                                           Utilities.GetDatabaseDate(DateTime.Now) +
                                          "' where customerid = " + customerModel.CustomerId + ";";
                DbQueryExecuter.WriteToDatabase(deleteCustomerSql);
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while deleting customer details");
            }
        }

        public int Create(CustomerModel customerModel)
        {
            try
            {
                if (GetCustomerByName(customerModel.Name) != null)
                {
                    Utilities.ErrorMessage("Duplicate customer name.Retry with different name");
                    return 0;
                }
                string _nextBillingIdQuery = "SELECT max(customerid) from customer";
                int nextId = StaticIdProvider.GetNextId(_nextBillingIdQuery);
                customerModel.Address.AddressId = _addressRepository.Create(customerModel.Address);
                string insertAddressSql = GetInsertQueryStatement(customerModel, nextId);
                DbQueryExecuter.WriteToDatabase(insertAddressSql);
                return nextId;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while adding customer details");
                return 0;
            }
        }


        private string GetInsertQueryStatement(CustomerModel customerModel, int nextId)
        {
            string dateTime = Gasware.Database.Utilities.GetDatabaseDate(DateTime.Now);
            return
               "INSERT INTO customer(CustomerId,Name,AddressId,PhoneNumber,Location,Price," +
               "DiscountPercentage,DiscountFlat,DepositAmount,CreateDate," +
               "CreatedBy,ModifiedDate,ModifiedBy, GstNumber, EmailId) VALUES (" +
               nextId + ",'" +
               customerModel.Name + "'," +
               customerModel.Address.AddressId + ",'" +
               customerModel.PhoneNumber + "','" +
               customerModel.Location + "'," +
               customerModel.Price + "," +
               customerModel.DiscountPercentage + "," +
               customerModel.DiscountFlat + "," +
               customerModel.DepositAmount + ",'" +
               dateTime + "','" +
               customerModel.CreatedBy + "','" +
               dateTime + "','" +
               customerModel.ModifiedBy + "','" +
               customerModel.GstNumber + "','" +
               customerModel.EmailId + "');";
        }

        private string GetUpdateQueryStatement(CustomerModel customerModel)
        {
            return
            "UPDATE customer SET" +
            " Name = '" + customerModel.Name +
            "', PhoneNumber ='" + customerModel.PhoneNumber +
            "', Location ='" + customerModel.Location +
            "', Price =" + customerModel.Price +
            ", DiscountPercentage =" + customerModel.DiscountPercentage +
            ", DiscountFlat =" + customerModel.DiscountFlat +
            ", DepositAmount =" + customerModel.DepositAmount +
            ", ModifiedDate ='" + Gasware.Database.Utilities.GetDatabaseDate(DateTime.Now) +
            "', ModifiedBy ='" + _username +
            "', GstNumber ='" + customerModel.GstNumber + 
            "', EmailId = '" + customerModel.EmailId +
            "' WHERE CustomerId =" + customerModel.CustomerId + ";";
        }
    }
}
