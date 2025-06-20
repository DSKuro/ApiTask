using System.Collections.Generic;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace ApiTask.Services.ViewModelSubServices
{
    public static class DbDataProcess
    {
        public static async Task<SortingTreeMemento> GetParameters(DbConnection connection, string command)
        {
            List<List<string>> data = await connection.GetData
                (ReadConfiguration.GetValueByKeyFromConfiguration(command), 1);
            return new SortingTreeMemento(data[0]);
        }

        public static async Task<List<List<string>>> GetCodes(DbConnection connection, string command)
        {
            return await connection.GetData
                (ReadConfiguration.GetValueByKeyFromConfiguration(command), 2);
        }
    }
}
