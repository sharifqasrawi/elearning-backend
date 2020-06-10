using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class Section
    {
        public long Id { get; set; }
        public int Order { get; set; }
        public string Name_EN { get; set; }
        public string Name_FR { get; set; }
        public string Slug_EN { get; set; }
        public string Slug_FR { get; set; }

        //[JsonIgnore]
        public Course Course { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }

        public IList<Session> Sessions { get; set; }
    }
}
