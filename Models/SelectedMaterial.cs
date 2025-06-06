using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace ApiTask.Models
{
    public class SelectedMaterial : ObservableObject
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public float Price { get; set; }
        public Dictionary<string, string> Attributes { get; set; }

        public SelectedMaterial(Materials material)
        {
            InitializeAttributes(material);
        }

        private void InitializeAttributes(Materials material)
        {
            this.Name = material.MaterialAttribute.Name;
            this.Code = material.MaterialAttribute.Code;
            this.Price = material.MaterialAttribute.Price;
            this.Attributes = material.MaterialAttribute.Attributes;
        }
    }
}
