using MySql.Data.MySqlClient;

namespace Gasware
{
    public class DatabaseConnection
    {
        public const string MySqlUserId = "root";
        public const string MySqlPassword = "mysql";
        public const string MySqlDatabase = "gasgallery";
        public const string MySqlServer = "localhost";
        public MySqlConnection GetMySqlConnection()
        {
            string connetionString = $@"server={MySqlServer};database={MySqlDatabase};"+
                                     $@"uid={MySqlUserId};pwd={MySqlPassword};";
            return new MySqlConnection(connetionString);
        }
    }
}
