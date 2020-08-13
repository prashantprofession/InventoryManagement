using Gasware.Database;
using Gasware.Models;
using Gasware.Repository.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Gasware.Repository
{
    public class VendorRepository : IVendorRepository
    {
        private string _username;
        private readonly AddressRepository _addressRepository;
        public VendorRepository(string username)
        {
            _username = username;
            _addressRepository = new AddressRepository(username);
        }
        public VendorModel Get(int id)
        {
            try
            {
                string sql = "SELECT * FROM supplier where supplierid=" + id.ToString();
                // MySqlDataReader rdr = DbQueryExecuter.ReadDatabase(sql);

                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
                VendorModel vendorModel = new VendorModel();
                while (rdr.Read())
                {
                    vendorModel = GetVendorFromRawData(rdr);
                }
                mySqlConnectionModel.MySqlConnection.Close();
                if (vendorModel.Address != null)
                    vendorModel.Address = _addressRepository.Get(vendorModel.Address.AddressId);
                return vendorModel;
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while getting dealer/supplier/vendor details");
                return null;
            }
        }

        public VendorModel GetVendorByName(string name)
        {
            try
            {
                string sql = "SELECT * FROM supplier where name='" + name + "';";
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
                VendorModel vendorModel = new VendorModel();
                while (rdr.Read())
                {
                    vendorModel = GetVendorFromRawData(rdr);
                }
                mySqlConnectionModel.MySqlConnection.Close();
                if (vendorModel.Address != null)
                    vendorModel.Address = _addressRepository.Get(vendorModel.Address.AddressId);
                return vendorModel;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while getting dealer/supplier/vendor details");
                return null;
            }
        }

        public List<VendorModel> GetVendors()
        {
            try
            {
                string sql = "SELECT * FROM supplier";
                List<VendorModel> vendorModels = new List<VendorModel>();
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
                VendorModel vendorModel = new VendorModel();
                while (rdr.Read())
                {
                    vendorModel = GetVendorFromRawData(rdr);
                    vendorModel.Address = _addressRepository.Get(vendorModel.Address.AddressId);
                    vendorModels.Add(vendorModel);
                }
                mySqlConnectionModel.MySqlConnection.Close();
                return vendorModels;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while getting all dealer/supplier/vendor details");
                return null;
            }
        }

        private static VendorModel GetVendorFromRawData(MySqlDataReader rdr)
        {
            return new VendorModel()
            {
                SupplierId = rdr.GetInt32(0),
                PhoneNumber = rdr.GetString(1),
                Name = rdr.GetString(2),
                Address = new AddressModel()
                {
                    AddressId = rdr.GetInt32(3)
                }
            };
        }

        public void Update(VendorModel vendorModel)
        {
            try
            {
                vendorModel.Address.AddressId = _addressRepository.Update(vendorModel.Address);
                string updateAddressSql = GetUpdateQueryStatement(vendorModel);
                DbQueryExecuter.WriteToDatabase(updateAddressSql);
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while updating dealer/supplier/vendor details");
            }
        }

        public int Create(VendorModel vendorModel)
        {

            try
            {
                if (GetVendorByName(vendorModel.Name).SupplierId != 0)
                {
                    Utilities.ErrorMessage("Duplicate vendor name. Retry with different name");
                    return 0;
                }
                string _nextSupplierIdQuery = "SELECT max(supplierid) from supplier";
                vendorModel.SupplierId = StaticIdProvider.GetNextId(_nextSupplierIdQuery);
                vendorModel.Address = new AddressModel()
                {
                    AddressId = _addressRepository.Create(vendorModel.Address)
                };
                string insertAddressSql = GetInsertQueryStatement(vendorModel);
                DbQueryExecuter.WriteToDatabase(insertAddressSql);
                return vendorModel.SupplierId;
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while adding dealer/supplier/vendor details");
                return 0;
            }
        }


        private string GetInsertQueryStatement(VendorModel vendorModel)
        {
            return
             "INSERT INTO supplier ( supplierid, name, phonenumber, addressid," +
             " createdby, createdate, modifiedby, modifieddate) VALUES (" +
             vendorModel.SupplierId.ToString() + ",'" +
             vendorModel.Name + "','" +
             vendorModel.PhoneNumber + "'," +
             vendorModel.Address.AddressId + ",'" +
             _username + "','" +
             Utilities.CurrentDbDate() + "','" +
             _username + "','" +
             Utilities.CurrentDbDate() + "');";
        }

        private string GetUpdateQueryStatement(VendorModel vendorModel)
        {
            return
             "update supplier set name = '" + vendorModel.Name +
             "', phonenumber = '" + vendorModel.PhoneNumber +
             "', modifiedby = '" + _username +
             "', modifieddate = '" + Utilities.GetDatabaseDate(DateTime.Now) +
             "' where supplierid = " + vendorModel.SupplierId + ";";
        }


        public void Delete(VendorModel vendorModel)
        {
            try
            {
                _addressRepository.Delete(vendorModel.Address.AddressId);

                string insertAddressSql = "delete from supplier where supplierid = " +
                            vendorModel.SupplierId.ToString() + ";";
                DbQueryExecuter.WriteToDatabase(insertAddressSql);
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while deleting dealer/vendor/supplier details");
            }
        }

    }
}
