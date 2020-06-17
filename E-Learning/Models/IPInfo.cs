using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class IPInfo
    {
        [JsonProperty("ip")]
        public string IPAddress { get; set; }

        [JsonProperty("continent_code")]
        public string Continent_Code { get; set; }
        [JsonProperty("continent_name")]
        public string Continent_Name { get; set; }
        [JsonProperty("country_code")]
        public string Country_Code { get; set; }
        [JsonProperty("country_name")]
        public string Country_Name { get; set; }
        [JsonProperty("region_code")]
        public string Region_Code { get; set; }
        [JsonProperty("region_name")]
        public string Region_Name { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("zip")]
        public string Zip { get; set; }
        [JsonProperty("latitude")]
        public string Latitude { get; set; }
        [JsonProperty("longitude")]
        public string Longitude { get; set; }
    }
}
