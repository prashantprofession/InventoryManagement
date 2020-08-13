using Gasware.Database;
using Gasware.Models;
using Gasware.Repository.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Gasware.Repository
{
    public class AddressRepository : IAddressRepository
    {
        private string _username;
        private string _maxAddressIdStatement = "SELECT max(addressid) from address";

        public AddressRepository(string username)
        {
            _username = username;
        }
        public AddressModel Get(int id)
        {

            string sql = "SELECT * FROM address where deactivatedate is null and addressid =" + id.ToString();
           // MySqlDataReader rdr = DbQueryExecuter.ReadDatabase(sql);

            MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
            MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
            AddressModel addressModel = new AddressModel();
            while (rdr.Read())
            {
                addressModel = GetAddressModelFromRawData(rdr);
            }
            mySqlConnectionModel.MySqlConnection.Close();
            return addressModel;
        }

        public List<AddressModel> GetAddresses()
        {
            string sql = "SELECT * FROM address where deactivatedate is null";
            // MySqlDataReader rdr = DbQueryExecuter.ReadDatabase(sql);

            MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
            MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
            List<AddressModel> addressModels = new List<AddressModel>();
            while (rdr.Read())
            {
                addressModels.Add(GetAddressModelFromRawData(rdr));
            }
            mySqlConnectionModel.MySqlConnection.Close();
            return addressModels;
        }

        private static AddressModel GetAddressModelFromRawData(MySqlDataReader rdr)
        {

            AddressModel addressModel = new AddressModel()
            {
                AddressId = rdr.GetInt32(0),
                Street = rdr.GetString(1),
                AddressLine1 = rdr.GetString(2),
                AddressLine2 = rdr.GetString(3),
                PinCode = rdr.GetString(4),
                City = rdr.GetString(6),
                State = rdr.GetString(7),
                Country = rdr.GetString(8)
            };

            return addressModel;
        }

        public int Create(AddressModel addressModel)
        {
            try
            {
                int nextAddressId = StaticIdProvider.GetNextId(_maxAddressIdStatement);
                string insertAddressSql = GetInsertQueryStatement(addressModel, nextAddressId);
                DbQueryExecuter.WriteToDatabase(insertAddressSql);
                return nextAddressId;
            }
            catch(Exception)
            {
                MessageBox.Show("Error occured while inserting Address details");
                return 0;
            }
        }

        public int Update(AddressModel address)
        {
            try
            {
                string updateAddressSql = GetUpdateQueryStatement(address, address.AddressId);
                DbQueryExecuter.WriteToDatabase(updateAddressSql);
                return address.AddressId;
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while updating Address details");
                return 0;
            }
        }

        public void Delete(int id)
        {
            string deleteQuery = "update address set deactivatedate ='" + 
                               Utilities.GetDatabaseDate(DateTime.Now) + 
                                 "' where addressid =" + id + ";";
            DbQueryExecuter.WriteToDatabase(deleteQuery);
        }

        private string GetInsertQueryStatement(AddressModel addressModel,int nextAddressId)
        {
            DateTime dateTime = DateTime.Now;
            return
                 "INSERT INTO address (" +
                 "addressid, street, addressline1, addressline2, pincode, " +
                 "city, state, country) VALUES (" +
                 nextAddressId + ",'" +
                 addressModel.Street + "','" +
                 addressModel.AddressLine1 + "','" +
                 addressModel.AddressLine2 + "','" +
                 addressModel.PinCode + "','" +
                 addressModel.City + "','" +
                 addressModel.State + "','" +
                 addressModel.Country + "'" +
                 ");";
        }

        private string GetUpdateQueryStatement(AddressModel addressModel, int addressId)
        {
            return
                 " Update address set" +
                 " street = '" + addressModel.Street +
                 "', addressline1 = '" + addressModel.AddressLine1 +
                 "', addressline2 = '" + addressModel.AddressLine2 +
                 "', pincode = '"      + addressModel.PinCode +
                 "', city = '"         + addressModel.City +
                 "', state = '"        + addressModel.State +
                 "', country = '"      + addressModel.Country +
                 "' where addressid =" + addressId + ";";

        }
    }
}
