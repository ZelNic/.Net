using System.Data;
using System.Data.SqlClient;

namespace Task_1
{
    public class DataBase
    {
        SqlConnection connectionString = new SqlConnection(@"Data Source=PC\SQLEXPRESS;Initial Catalog=Map;Integrated Security=True");

        public void OpenConnection()
        {
            if (connectionString.State == ConnectionState.Closed)
            {
                connectionString.Open();
            }
        }

        public void CloseConnection()
        {
            if (connectionString.State == ConnectionState.Open)
            {
                connectionString.Close();
            }
        }

        public SqlConnection GetConnection()
        {
            return connectionString;
        }
    }
}
