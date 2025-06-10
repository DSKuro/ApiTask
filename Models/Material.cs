using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApiTask.Models
{
    public record Materials
    {
        [JsonPropertyName("material")]
        public Material MaterialAttribute { get; set; }
    }

    public record Material 
    {
        [JsonPropertyName("id")] 
        public Int32 Id { get; set; }

        [JsonPropertyName("node_id")]
        public Int32 NodeId { get; set; }

        [JsonPropertyName("etim_class_id")]
        public string EtimClassId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("types")]
        public Dictionary<string, string> Types { get; set; }

        [JsonPropertyName("min_order_qty")]
        public Dictionary<string, int> MinOrderQty { get; set; }

        [JsonPropertyName("series")]
        public string Series { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("unit")]
        public string Unit { get; set; }

        [JsonPropertyName("volume")]
        public float Volume { get; set; }

        [JsonPropertyName("weight")]
        public float Weight { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("no_price")]
        public bool NoPrice { get; set; }

        [JsonPropertyName("price")]
        public float Price { get; set; }

        [JsonPropertyName("barcode")]
        public List<Int64> BarCode { get; set; }

        [JsonPropertyName("thumbnail_url")]
        public string ThumbnaulUrl { get; set; }

        [JsonPropertyName("additional_images")]
        public List<string> AdditionalImages { get; set; }

        [JsonPropertyName("attributes")]
        public Dictionary<string, string> Attributes { get; set; }

        [JsonPropertyName("etim_attributes")]
        public Dictionary<string, string> EtimAttributes { get; set; }

        [JsonPropertyName("packing")]
        public Dictionary<string, int> Packing { get; set; }

        [JsonPropertyName("avg_delivery")]
        public Dictionary<string, int> AvgDelivery { get; set; }

        [JsonPropertyName("accessories")]
        public List<string> Accessories { get; set; }

        [JsonPropertyName("accessories_codes")]
        public List<string> AccessoriesCodes { get; set; }

        [JsonPropertyName("sale")]
        public List<SaleAttribute> Sale { get; set; }

    }

    public record SaleAttribute()
    {
        [JsonPropertyName("expire")]
        public bool Expire { get; set; }

        [JsonPropertyName("discount")]
        public float Discount { get; set; }

        [JsonPropertyName("date_start")]
        public int StartDate { get; set; }

        [JsonPropertyName("date_end")]
        public int EndDate { get; set; }
    }
}
