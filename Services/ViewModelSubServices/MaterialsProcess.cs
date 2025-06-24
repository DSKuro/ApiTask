using ApiTask.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ApiTask.Services.ViewModelSubServices
{
    public class MaterialsProcess
    {
        private static readonly string MaterialSite = "material";

        private ExcelParser _excelParser;

        public MaterialsProcess()
        {
            _excelParser = new ExcelParser();
        }

        public async Task GetMaterialDataImpl(ObservableCollection<SelectedMaterial> SelectedMaterials,
            string path)
        {
            List<string> codes = _excelParser.GetDataFromFile(path);
            await GetAllMaterials(codes, SelectedMaterials);
        }

        private async Task GetAllMaterials(List<string> codes, ObservableCollection<SelectedMaterial> SelectedMaterials)
        {
            SelectedMaterials.Clear();
            string site = ReadConfiguration.GetValueByKeyFromConfiguration(MaterialSite);
            foreach (string code in codes)
            {
                await GetMaterial(site, code, SelectedMaterials);
            }
        }

        private async Task GetMaterial(string site, string code, ObservableCollection<SelectedMaterial> SelectedMaterials)
        {
            Materials material = (Materials)await Http.GetDataWithJSON(GetMaterialPath(site, code),
                typeof(Materials));
            SelectedMaterials.Add(new SelectedMaterial(material));
        }

        private Http.QueryBuilder GetMaterialPath(string uri, string code)
        {
            Http.QueryBuilder path = new Http.QueryBuilder(uri);
            path.SetQuery("code", code);
            return path;
        }
    }
}
