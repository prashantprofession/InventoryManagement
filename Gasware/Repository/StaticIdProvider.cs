using MySql.Data.MySqlClient;
using System;

namespace Gasware.Repository
{
    public static class StaticIdProvider
    {
        public static int GetNextId(string sqlStatement)
        {
            DatabaseConnection databaseConnection = new DatabaseConnection();
            MySqlConnection connection = databaseConnection.GetMySqlConnection();

            connection.Open();
            var cmd = new MySqlCommand(sqlStatement, connection);

            string maxCountString = cmd.ExecuteScalar()?.ToString();
            connection.Close();
            if (string.IsNullOrEmpty(maxCountString))
                return 1;
            else
                return Convert.ToInt32(maxCountString) + 1;
        }
    }
}
