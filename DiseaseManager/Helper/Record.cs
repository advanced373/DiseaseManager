using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiseaseManager.Helper
{
    class Record
    {
        public DateTime Date { get; set; }
        public int Cases { get; set; }
        public int Deaths { get; set; }
        public int Population { get; set; }
        public string Country { get; set; }
        public string Continent { get; set; }
    }
}
