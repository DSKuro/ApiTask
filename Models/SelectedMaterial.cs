using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace ApiTask.Models
{
    public class SelectedMaterial : ObservableObject
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public float Price { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public List<string> Attributes { get; set; }

        public SelectedMaterial(Materials material)
        {
            InitializeAttributes(material);
        }

        private void InitializeAttributes(Materials material)
        {
            InitializeBaseAttributes(material);
            Attributes = new List<string>();
            FillParameters(material);
        }

        private void InitializeBaseAttributes(Materials material)
        {
            this.Name = material.MaterialAttribute.Name;
            this.Code = material.MaterialAttribute.Code;
            this.Price = material.MaterialAttribute.Price;
            this.Url = material.MaterialAttribute.Url;
            this.ThumbnailUrl = material.MaterialAttribute.ThumbnaulUrl;
        }

        private void FillParameters(Materials material)
        {
            Attributes.Clear();
            foreach ((string key, string value) in material.MaterialAttribute.Attributes)
            {
                Attributes.Add($"{key}: {value}");
            }
        }
    }
}
