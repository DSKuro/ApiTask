using Microsoft.Data.SqlClient;
using System.Collections.Generic;
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
        }

        public static async Task CloseConnection()
        {
            if (Connection.State == ConnectionState.Open)
            {
                await Connection.CloseAsync();
            }
        }

        public static async Task<List<string>> GetData(string command) 
        {
            List<string> codes = new List<string>();
            SqlDataReader reader = await ExecuteCommand(command);
            await ReadData(reader, codes);
            return codes;
        }

        private static async Task<SqlDataReader> ExecuteCommand(string command)
        {
            SqlCommand sqlCommand = new SqlCommand(command, Connection);
            return await sqlCommand.ExecuteReaderAsync();
        }

        private static async Task ReadData(SqlDataReader reader, List<string> codes)
        {
            if (reader.HasRows)
            {
                string column = reader.GetName(0);

                while (await reader.ReadAsync())
                {
                    codes.Add((string)reader.GetValue(0));
                }
            }
        }
    }
}
