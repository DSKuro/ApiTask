using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiTask.Services.ViewModelSubServices
{
    public static class DbDataProcess
    {
        public static async Task<SortingTreeMemento> GetParameters(string command)
        {
            List<List<string>> data = await DbConnection.GetData
                (ReadConfiguration.GetValueByKeyFromConfiguration(command), 1);
            return new SortingTreeMemento(data[0]);
        }

        public static async Task<List<List<string>>> GetCodes(string command)
        {
            return await DbConnection.GetData
                (ReadConfiguration.GetValueByKeyFromConfiguration(command), 2);
        }
    }
}
