using Gasware.Database;
using Gasware.Models;
using Gasware.Repository.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Gasware.Repository
{
    public class UserRepository : IUserRespository
    {
        private string _username;
        public UserRepository(string username)
        {
            _username = username;
        }

        public UserModel Get(int id)
        {
            try
            {
                string sql = "SELECT * FROM users where deactivatedate is null and userid=" + id.ToString();
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
                
                UserModel user = new UserModel();
                while (rdr.Read())
                {
                    user = GetModelForRawData(rdr);
                }
                mySqlConnectionModel.MySqlConnection.Close();
                if (user.UserId == 0)
                {
                    Utilities.ErrorMessage("No user yet.Atleast one user is needed to use it.");
                    return null;
                }
                return user;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while getting user details");
                return null;
            }
        }

        public UserModel GetUserByName(string username)
        {
            try
            {
                string sql = "SELECT * from users where deactivatedate is null and name='" + username + "'";
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;

                UserModel user = new UserModel();
                while (rdr.Read())
                {
                    user = GetModelForRawData(rdr);
                }
                mySqlConnectionModel.MySqlConnection.Close();
                if (user.UserId == 0)
                {
                    //Utilities.ErrorMessage("No user yet.Atleast one user is needed to use it.");
                    return null;
                }
                return user;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while getting user details");
                return null;
            }
        }
        public List<UserModel> GetUsers()
        {
            try
            {
                string sql = "SELECT * FROM users where deactivatedate is null and name <> 'superuser'";
                List<UserModel> userModels = new List<UserModel>();
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;

                UserModel userModel = new UserModel();
                while (rdr.Read())
                {
                    userModel = GetModelForRawData(rdr);
                    userModels.Add(userModel);
                }
                mySqlConnectionModel.MySqlConnection.Close();
                return userModels;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while getting all user details");
                return null;
            }
        }

        private static UserModel GetModelForRawData(MySqlDataReader rdr)
        {
            return new UserModel()
            {
                UserId = rdr.GetInt32(0),
                Name = rdr.GetString(1),
                Password = rdr.GetString(2)
            };
        }

        public void Update(UserModel userModel)
        {
            try
            {
                if (GetUserByName(userModel.Name) != null)
                {
                    Utilities.ErrorMessage("Duplicate user name. Retry with different name.");
                    return;
                }
                string updateUserSql = GetUpdateQueryStatement(userModel);
                DbQueryExecuter.WriteToDatabase(updateUserSql);
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while updating user details");
            }
        }

        public int Create(UserModel user)
        {
            try
            {
                if (GetUserByName(user.Name) != null)
                {
                    Utilities.ErrorMessage("Duplicate user name. Retry with different name.");
                    return 0;
                }
                string _nextBillingIdQuery = "SELECT max(userid) from users";
                int nextId = StaticIdProvider.GetNextId(_nextBillingIdQuery);
                string insertUserSql = GetInsertQueryStatement(user, nextId);
                DbQueryExecuter.WriteToDatabase(insertUserSql);
                return nextId;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while adding user details");
                return 0;
            }
        }


        private string GetInsertQueryStatement(UserModel userModel, int nextId)
        {
            return
               "INSERT INTO users(userid, name, password) VALUES (" +
               nextId + ",'" + userModel.Name + "','" +
               Password.GetEncodedPassword(userModel.Password) + "');";
        }

        private string GetUpdateQueryStatement(UserModel userModel)
        {
            return
                "UPDATE users SET " +
                "   Name = '" + userModel.Name +
                "', Password ='" + Password.GetEncodedPassword(userModel.Password) +
                "' where userid =" + userModel.UserId;
        }

        public void Delete(UserModel userModel)
        {
            try
            {
                string deleteQuery = "Update users set deactivatedate = '" +
                                      Utilities.GetDatabaseDate(DateTime.Now) +
                                     "' where userid = " + userModel.UserId + ";";
                DbQueryExecuter.WriteToDatabase(deleteQuery);
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while deleting user details");
            }

        }
    }
}
