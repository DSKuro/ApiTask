using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiTask.Services.ViewModelSubServices
{
    public class DbDataProcess
    {
        public async Task<SortingTreeMemento> GetParameters(DbConnection connection, string command)
        {
            List<List<string>> data = await connection.GetData
                (ReadConfiguration.GetValueByKeyFromConfiguration(command), 1);
            return new SortingTreeMemento(data[0]);
        }

        public async Task<List<List<string>>> GetCodes(DbConnection connection, string command)
        {
            return await connection.GetData
                (ReadConfiguration.GetValueByKeyFromConfiguration(command), 2);
        }
    }
}
