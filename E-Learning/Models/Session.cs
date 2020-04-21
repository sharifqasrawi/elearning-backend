using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class Session
    {
        public long Id { get; set; }
        public int Order { get; set; }
        public string Title_EN { get; set; }
        public int Duration { get; set; }
        public Section Section { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }

        public virtual IEnumerable<SessionContent> Contents { get; set; }
    }
}
