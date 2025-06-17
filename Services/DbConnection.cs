using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace ApiTask.Services
{
    public class DbConnection
    {
        static SqlConnection Connection;

        public static async Task OpenConnection(string ConnectionUrl)
        {
            if (Connection == null)
            {
                Connection = new SqlConnection(ConnectionUrl);
            }
            await Connection.OpenAsync();
            Console.WriteLine("Подключение открыто");
        }

        public static async Task CloseConnection()
        {
            if (Connection.State == ConnectionState.Open)
            {
                await Connection.CloseAsync();
            }
        }
    }
}
