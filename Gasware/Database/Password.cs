using MySql.Data.MySqlClient;
using System;
using System.Security.Cryptography;

namespace Gasware
{
    public static class Password
    {
        public static string GetEncodedPassword(string passwordString)
        {
            string randomchar = new Random().Next(100, 999).ToString();
            passwordString = randomchar + passwordString;
            byte[] encData_byte = new byte[passwordString.Length];
            encData_byte = System.Text.Encoding.UTF8.GetBytes(passwordString);
            string encodedData = Convert.ToBase64String(encData_byte);
            return encodedData;
        }

        public static string GetDecodedPassword(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            result = result.Substring(3);
            return result;
        }

        public static bool IsValidPassword(MySqlConnection connection, string username, string password)
        {
            connection.Open();
            var stm = "SELECT password from users where name='" + username + "'";
            var cmd = new MySqlCommand(stm, connection);

            // string encodedPassword = Password.GetEncodedPassword(password);

            string passwordstring = cmd.ExecuteScalar()?.ToString();
            connection.Close();
            if (string.IsNullOrEmpty(passwordstring))
            {
                return false;
            }
            else
            {
                string decodedPassword = Password.GetDecodedPassword(passwordstring);

                if (!string.IsNullOrEmpty(decodedPassword) && decodedPassword == password)
                    return true;
                return false;
            }
        }
    }
}
