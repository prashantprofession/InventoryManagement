using Gasware.Database;
using Gasware.Models;
using Gasware.Repository.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Gasware.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private string _username;
        private readonly IAddressRepository _addressRepository;

        public CompanyRepository(string username)
        {
            _username = username;
            _addressRepository = new AddressRepository(username);
        }
        public CompanyModel Get(int id)
        {
            try
            {
                
                string sql = "SELECT * FROM company where companyid=" + id.ToString();
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
                CompanyModel companyModel = new CompanyModel();
                while (rdr.Read())
                {
                    companyModel = GetVendorFromRawData(rdr);
                }
                mySqlConnectionModel.MySqlConnection.Close();
                if (companyModel.Address == null)
                {
                    MessageBox.Show("Address details are mandatory to continue.Fill the same.", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
                    return new CompanyModel();
                }
                companyModel.Address = _addressRepository.Get(companyModel.Address.AddressId);
                return companyModel;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while getting company details");
                return null;
            }
        }

        public decimal GetTaxRate(int id, string type)
        {
            try
            {
                string sql = "SELECT " + type + " FROM company where companyid=" + 
                               id.ToString();
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
                decimal taxRate = 0;
                while (rdr.Read())
                {
                    taxRate = rdr.GetDecimal(0);
                }
                mySqlConnectionModel.MySqlConnection.Close();                
                return taxRate;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while getting company details");
                return 0;
            }
        }

        public List<CompanyModel> GetCompanies()
        {
            try
            {
                string sql = "SELECT * FROM company";
                List<CompanyModel> companyModels = new List<CompanyModel>();
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;                
                CompanyModel companyModel = new CompanyModel();
                while (rdr.Read())
                {
                    companyModel = GetVendorFromRawData(rdr);
                    companyModel.Address = _addressRepository.Get(companyModel.Address.AddressId);
                    companyModels.Add(companyModel);
                }
                mySqlConnectionModel.MySqlConnection.Close();
                return companyModels;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while getting company details");
                return null;
            }
        }

        private static CompanyModel GetVendorFromRawData(MySqlDataReader rdr)
        {
            return new CompanyModel()
            {
                CompanyId = rdr.GetInt32(0),
                Name = rdr.GetString(1),
                Address = new AddressModel()
                {
                    AddressId = rdr.GetInt32(2)
                },
                PhoneNumber = rdr.GetString(3),
                CreatedDate = rdr.GetDateTime(4),
                License = rdr.GetString(8),
                CentralGst = rdr.GetDecimal(9),
                StateGst = rdr.GetDecimal(10),
                ReportsPath = rdr.GetString(11),
                GstNumber = rdr.GetString(12),
                AccountNumber = rdr.IsDBNull(13) ? string.Empty : rdr.GetString(13),
                IFSCCode = rdr.IsDBNull(14) ? string.Empty : rdr.GetString(14),
                EmailId = rdr.IsDBNull(15) ? string.Empty : rdr.GetString(15),
                HSNNumber = rdr.IsDBNull(16) ? string.Empty : rdr.GetString(16),
                Password = rdr.IsDBNull(17) ? string.Empty : Password.GetDecodedPassword(rdr.GetString(17))
            };              
        }

        public void Update(CompanyModel companyModel)
        {
            try
            {
                companyModel.Address.AddressId = _addressRepository.Update(companyModel.Address);
                string updateCompanySql = GetUpdateQueryStatement(companyModel);
                DbQueryExecuter.WriteToDatabase(updateCompanySql);
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while updating company address details");
            }
        }

        public int Create(CompanyModel companyModel)
        {

            try
            {
                companyModel.Address = new AddressModel()
                {
                    AddressId = _addressRepository.Create(companyModel.Address)
                };
                int nextId = StaticIdProvider.GetNextId("select max(companyid) from company");
                string insertCompanySql = GetInsertQueryStatement(companyModel, nextId);
                DbQueryExecuter.WriteToDatabase(insertCompanySql);
                return 1;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while creating company address details");
                return 0;
            }
        }

        public string UpdateLicense(int id, string license)
        {
            string encodedLicense = Password.GetEncodedPassword(license);
            string updateQuery = "update company set license = '" + encodedLicense + 
                                 "' where companyid=" + id + ";";
            DbQueryExecuter.WriteToDatabase(updateQuery);
            return GetLicenseExpiryDate(id).ToString("dd-MM-yyyy");
        }

        public DateTime GetLicenseExpiryDate(int id)
        {
            int trialPeriod = 60; //30 days
            int yearlyDays = 366;
            CompanyModel company = Get(id);

            if (company?.License == null)
                return DateTime.Now.AddDays(trialPeriod);

            string decodedLicense = Password.GetDecodedPassword(company.License);
            switch (decodedLicense.ToUpper())
            {
                case "TRIAL":
                    return company.CreatedDate.AddDays(trialPeriod);                    
                case "1 YEAR":
                    return company.CreatedDate.AddDays(yearlyDays * 1);
                case "3 YEARS":
                    return company.CreatedDate.AddDays(yearlyDays * 3);
                case "5 YEARS":
                    return company.CreatedDate.AddDays(yearlyDays * 5);
                case "LIFETIME":
                    return company.CreatedDate.AddDays(yearlyDays * 50);
                default:
                    return DateTime.Now;
            }
        }

        public string GetLicense(int id)
        {
            string selectLicenseQuery = "select license from company where companyid=" + id + ";";
            DbQueryExecuter.WriteToDatabase(selectLicenseQuery);
            string license = string.Empty;

            MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(selectLicenseQuery);
            MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;            
            while (rdr.Read())
            {
                license = rdr.GetString(0);
            }
            mySqlConnectionModel.MySqlConnection.Close();
            if (string.IsNullOrEmpty(license))
            {
                MessageBox.Show("Company details are not updated.You have to set company details to continue.", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
                return string.Empty;
            }            
            return Password.GetDecodedPassword(license);
        }

        public void Delete(CompanyModel companyModel)
        {
            try
            {
                _addressRepository.Delete(companyModel.Address.AddressId);
                string deleteSql = "delete from company where companyid=" + companyModel.CompanyId + ";";
                DbQueryExecuter.WriteToDatabase(deleteSql);
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while deleting company details");
            }
        }


        private string GetInsertQueryStatement(CompanyModel companyModel, int nextId)
        {
            string dateTime = Utilities.GetDatabaseDate(DateTime.Now);
            string encodedTrial = Password.GetEncodedPassword("Trial");
            return
             "INSERT INTO company ( companyid, name, phonenumber, addressid," +
             " createdby, createdate, modifiedby, modifieddate, license, cgst," +
             " sgst, reportspath, gstnumber, bankaccountnumber, bankifsccode," +
             " emailid, hsnno, emailpassword ) VALUES (" +
             nextId + ",'" +
             companyModel.Name + "','" +
             companyModel.PhoneNumber + "'," +
             companyModel.Address.AddressId + ",'" +
             _username + "','" +
             dateTime + "','" +
             _username + "','" +
             dateTime + "','" +
             encodedTrial + "'," +
             companyModel.CentralGst + "," +
             companyModel.StateGst + ",'" +
             companyModel.ReportsPath + ",'" +
             companyModel.GstNumber + "'" +
             companyModel.AccountNumber + "','" +
             companyModel.IFSCCode + "','" +
             companyModel.EmailId + "','" +
             companyModel.HSNNumber + "','" +
             Password.GetEncodedPassword(companyModel.Password) +
             "');";
        }

        private string GetUpdateQueryStatement(CompanyModel companyModel)
        {
            return
             "update company set name = '" + companyModel.Name +
             "', phonenumber = '" + companyModel.PhoneNumber +
             "', cgst = " + companyModel.CentralGst +
             ", sgst = " + companyModel.StateGst +
             ", modifiedby = '" + _username +
             "', modifieddate = '" + Utilities.GetDatabaseDate(DateTime.Now) +
             "', reportspath = '" + companyModel.ReportsPath +
             "', gstnumber = '" + companyModel.GstNumber +
             "', bankaccountnumber ='" + companyModel.AccountNumber +
             "', bankifsccode ='" + companyModel.IFSCCode + 
             "', emailid ='" + companyModel.EmailId + 
             "', hsnno ='" + companyModel.HSNNumber +
             "', emailpassword ='" + Password.GetEncodedPassword(companyModel.HSNNumber) +
             "' where companyid =" + companyModel.CompanyId + ";";
        }
    }
}
