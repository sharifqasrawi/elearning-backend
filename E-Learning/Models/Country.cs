using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name_EN { get; set; }
        public string Name_FR { get; set; }
        public string FlagPath { get; set; }
        public bool? IsAvailable { get; set; }

    }
}
