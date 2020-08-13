using Gasware.Database;
using Gasware.Models;
using Gasware.Repository.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Gasware.Repository
{
    public class DeliveryPersonRepository : IDeliveryPersonRepository
    {
        private string _username;
        private IAddressRepository _addressRepository;
        public DeliveryPersonRepository(string username)
        {
            _username = username;
            _addressRepository = new AddressRepository(_username);
        }
        public DeliveryPersonModel Get(int id)
        {
            try
            {
                string sql = "SELECT * FROM deliveryperson where deactivatedate is null and deliverypersonid=" + id.ToString();
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
                DeliveryPersonModel deliveryPersonModel = new DeliveryPersonModel();
                while (rdr.Read())
                {
                    deliveryPersonModel = GetModelForRawData(rdr);
                }
                mySqlConnectionModel.MySqlConnection.Close();
                if (deliveryPersonModel.DeliveryPersonId == 0)
                {
                    MessageBox.Show("No delivery person yet.Atleast one active delivery person is needed to use it.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
                deliveryPersonModel.Address = _addressRepository.Get(deliveryPersonModel.Address.AddressId);
                return deliveryPersonModel;
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while getting delivery person details");
                return null;
            }
        }

        public DeliveryPersonModel GetDeliveryPersonByName(string name)
        {
            try
            {
                string sql = "SELECT * FROM deliveryperson where deactivatedate is null and name='" + name + "';";
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
                DeliveryPersonModel deliveryPersonModel = new DeliveryPersonModel();
                while (rdr.Read())
                {
                    deliveryPersonModel = GetModelForRawData(rdr);
                }
                mySqlConnectionModel.MySqlConnection.Close();
                if (deliveryPersonModel.DeliveryPersonId == 0)
                {
                    //Utilities.ErrorMessage("No delivery person yet.Atleast one active delivery person is needed to use it.");
                    return null;
                }
                deliveryPersonModel.Address = _addressRepository.Get(deliveryPersonModel.Address.AddressId);
                return deliveryPersonModel;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while getting delivery person details");
                return null;
            }
        }
        public List<DeliveryPersonModel> GetDeliveryPersons()
        {
            try
            {
                string sql = "SELECT * FROM deliveryperson where deactivatedate is null";
                List<DeliveryPersonModel> deliveryPersonModels = new List<DeliveryPersonModel>();
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;

                DeliveryPersonModel deliveryPersonModel = new DeliveryPersonModel();
                while (rdr.Read())
                {
                    deliveryPersonModel = GetModelForRawData(rdr);
                    deliveryPersonModel.Address = _addressRepository.Get(deliveryPersonModel.Address.AddressId);
                    deliveryPersonModels.Add(deliveryPersonModel);
                }
                mySqlConnectionModel.MySqlConnection.Close();

                return deliveryPersonModels;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while gettign delivery persons details");
                return null;
            }
        }

        private static DeliveryPersonModel GetModelForRawData(MySqlDataReader rdr)
        {
            return new DeliveryPersonModel()
            {
                DeliveryPersonId = rdr.GetInt32(0),
                Name = rdr.GetString(1),
                PhoneNumber = rdr.GetString(2),
                Address = new AddressModel()
                {
                    AddressId = rdr.GetInt32(4)
                }
            };
        }

        public void Update(DeliveryPersonModel deliveryPersonModel)
        {
            try
            {
                if (GetDeliveryPersonByName(deliveryPersonModel.Name) != null)
                {
                    Utilities.ErrorMessage("Duplicate delivery person name.Retry with different name");
                    return ;
                }
                string updateDeliveryPersonSql = GetUpdateQueryStatement(deliveryPersonModel);
                _addressRepository.Update(deliveryPersonModel.Address);
                DbQueryExecuter.WriteToDatabase(updateDeliveryPersonSql);
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while updating delivery person's details");
            }
        }

        public int Create(DeliveryPersonModel deliveryPersonModel)
        {
            try
            {
                if (GetDeliveryPersonByName(deliveryPersonModel.Name) != null)
                {
                    //Utilities.ErrorMessage("Duplicate delivery person name.Retry with different name");
                    return 0;
                }
                string _nextBillingIdQuery = "SELECT max(deliverypersonid) from deliveryperson";
                int nextId = StaticIdProvider.GetNextId(_nextBillingIdQuery);
                deliveryPersonModel.Address.AddressId = _addressRepository.Create(deliveryPersonModel.Address);
                string insertDeliveryPersonSql = GetInsertQueryStatement(deliveryPersonModel, nextId);
                DbQueryExecuter.WriteToDatabase(insertDeliveryPersonSql);
                return nextId;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while adding delivery person's details");
                return 0;
            }
        }


        private string GetInsertQueryStatement(DeliveryPersonModel deliveryPersonModel, int nextId)
        {
            return
               "INSERT INTO deliveryperson(deliverypersonid, name, phonenumber, createdate, addressid) VALUES (" +
               nextId + ",'" +
               deliveryPersonModel.Name + "','" +
               deliveryPersonModel.PhoneNumber + "','" +
               Gasware.Database.Utilities.GetDatabaseDate(DateTime.Now) + "'," +
               deliveryPersonModel.Address.AddressId + ");";
        }

        private string GetUpdateQueryStatement(DeliveryPersonModel deliveryPersonModel)
        {
            return
                "UPDATE deliveryperson SET " +
                "   Name = '" + deliveryPersonModel.Name +
                "', PhoneNumber ='" + deliveryPersonModel.PhoneNumber +
                "' WHERE deliverypersonid =" + deliveryPersonModel.DeliveryPersonId + ";";
        }

        public void Delete(DeliveryPersonModel deliveryPersonModel)
        {
            try
            {
                _addressRepository.Delete(deliveryPersonModel.Address.AddressId);
                string deleteQuery = "Update deliveryperson set deactivatedate = '" +
                                      Utilities.GetDatabaseDate(DateTime.Now) +
                                     "' where deliverypersonid = " + deliveryPersonModel.DeliveryPersonId + ";";
                DbQueryExecuter.WriteToDatabase(deleteQuery);
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while deleting delivery person details");
            }

        }
    }
}
