using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class Visit
    {
        public long Id { get; set; }
        public string IPAddress { get; set; }
        public string BrowserInfo { get; set; }
        public string Continent_Code { get; set; }
        public string Continent_Name { get; set; }
        public string Country_Code { get; set; }
        public string Country_Name { get; set; }
        public string Region_Code { get; set; }
        public string Region_Name { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public DateTime? DateTime { get; set; }
        public long? DayVisitsCount { get; set; }
    }
}
