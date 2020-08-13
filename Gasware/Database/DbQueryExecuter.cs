using Gasware.Models;
using MySql.Data.MySqlClient;
using System.Data;


namespace Gasware
{
    public static class DbQueryExecuter
    {    
        public static void WriteToDatabase(string sqlQuery)
        {
            DatabaseConnection databaseConnection = new DatabaseConnection();
            MySqlConnection mySqlconnection = databaseConnection.GetMySqlConnection();
            mySqlconnection.Open();
            var cmd = new MySqlCommand(sqlQuery, mySqlconnection);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            mySqlconnection.Close();
        }

        public static MySqlConnectionModel ReadDatabase(string sql)
        {
            DatabaseConnection databaseConnection = new DatabaseConnection();
            MySqlConnection mySqlconnection = databaseConnection.GetMySqlConnection();
            mySqlconnection.Open();
            var cmd = new MySqlCommand(sql, mySqlconnection);
            MySqlConnectionModel mySqlConnectionModel = new MySqlConnectionModel()
            {
                MySqlConnection = mySqlconnection,
                MySqlDataReader = cmd.ExecuteReader()
            };
            return mySqlConnectionModel;            
        }

        public static string GetSingleStringForQuery(string sql)
        {
            MySqlConnectionModel mySqlConnectionModel = ReadDatabase(sql);
            MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
            string returnString = string.Empty;
            while (rdr.Read())
            {
                returnString = rdr.GetString(0);
            }
            mySqlConnectionModel.MySqlConnection.Close();
            return returnString;
        }
        
    }
}
