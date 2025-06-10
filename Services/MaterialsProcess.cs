using ApiTask.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ApiTask.Services
{
    public static class MaterialsProcess
    {
        private static readonly string MaterialSite = "material";

        public static async Task GetMaterialDataImpl(ObservableCollection<SelectedMaterial> SelectedMaterials,
            string path)
        {
            List<string> codes = ExcelParser.GetDataFromFile(path);
            await GetAllMaterials(codes, SelectedMaterials);
        }

        private static async Task GetAllMaterials(List<string> codes, ObservableCollection<SelectedMaterial> SelectedMaterials)
        {
            SelectedMaterials.Clear();
            string site = ReadConfiguration.GetValueByKeyFromConfiguration(MaterialSite);
            foreach (string code in codes)
            {
                await GetMaterial(site, code, SelectedMaterials);
            }
        }

        private static async Task GetMaterial(string site, string code, ObservableCollection<SelectedMaterial> SelectedMaterials)
        {
            Materials material = (Materials)await Http.GetDataWithJSON(GetMaterialPath(site, code),
                typeof(Materials));
            SelectedMaterials.Add(new SelectedMaterial(material));
        }

        private static Http.QueryBuilder GetMaterialPath(string uri, string code)
        {
            Http.QueryBuilder path = new Http.QueryBuilder(uri);
            path.SetQuery("code", code);
            return path;
        }
    }
}
