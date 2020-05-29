using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class UserQuiz
    {
        public long Id { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }

        [JsonIgnore]
        public Quiz Quiz { get; set; }
        public long QuizId { get; set; }
        public DateTime? TakeDateTime { get; set; }
        public bool? IsSubmitted { get; set; }
        public bool? IsStarted{ get; set; }
        public bool? IsOngoing{ get; set; }
        public float? Result { get; set; }


    }
}
