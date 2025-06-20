using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ApiTask.Services
{
    public class DbConnection
    {
        private SqlConnection Connection;

        public DbConnection(string connectionUrl)
        {
            Connection = new SqlConnection(connectionUrl);
        }

        public async Task OpenConnection()
        {
            await Connection.OpenAsync();
        }

        public async Task CloseConnection()
        {
            if (Connection.State == ConnectionState.Open)
            {
                await Connection.CloseAsync();
            }
        }

        public async Task<List<List<string>>> GetData(string command, int columnsCount)
        {
            List<List<string>> data = new List<List<string>>();
            await GetDataImpl(command, data, columnsCount);
            return data;
        }

        private async Task GetDataImpl(string command, List<List<string>> data, int columnsCount)
        {
            SqlDataReader reader = await ExecuteCommand(command);
            await ReadData(reader, data, columnsCount);
            await reader.CloseAsync();
        }

        private async Task<SqlDataReader> ExecuteCommand(string command)
        {
            SqlCommand sqlCommand = new SqlCommand(command, Connection);
            return await sqlCommand.ExecuteReaderAsync();
        }

        private async Task ReadData(SqlDataReader reader, List<List<string>> data, int columnsCount)
        {
            if (reader.HasRows)
            {
                PrepareData(data, columnsCount);
                await ReadDataImpl(reader, data, columnsCount);
            }
        }

        private void PrepareData(List<List<string>> data, int columnsCount)
        {
            for (int i = 0; i < columnsCount; i++)
            {
                data.Add(new List<string>());
            }
        }

        private async Task ReadDataImpl(SqlDataReader reader, List<List<string>> data, int columnsCount)
        {
            while (await reader.ReadAsync())
            {
                for (int i = 0; i < columnsCount; i++)
                {
                    data[i].Add((string)reader.GetValue(i));
                }
            }
        }
    }
}
