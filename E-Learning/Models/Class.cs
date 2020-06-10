using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class Class
    {
        [Key]
        public string Id { get; set; }
        public string Name_EN { get; set; }
        public string Name_FR { get; set; }

        [JsonIgnore]
        public Course Course { get; set; }
        public long? CourseId { get; set; }

        public IList<ClassUser> ClassUsers { get; set; }

    }
}
