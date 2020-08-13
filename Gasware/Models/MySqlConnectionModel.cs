using MySql.Data.MySqlClient;

namespace Gasware.Models
{
    public class MySqlConnectionModel
    {
        public MySqlDataReader MySqlDataReader { get; set; }
        public MySqlConnection MySqlConnection { get; set; }
    }
}
