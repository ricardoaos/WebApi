using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class Event
    {
        public string type { get; set; }
        public string destination { get; set; }
        public string origin { get; set; }
        public int amount { get; set; }
    }
}
