using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTintLibrary
{
    public class BaseData
    {
        public int id { get; set; }
        public string base_name { get; set; }
        public string can { get; set; }
        public string product_code { get; set; }
        public string product_name { get; set; }
        public double price_per_can { get; set; }
        public string surcharge { get; set; }
        public object fill { get; set; }
        public string barcode { get; set; }
        public string base_code { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int? material { get; set; }
    }
    class AutoTintBase
    {
        public Links links { get; set; }
        public int count { get; set; }
        public int total_pages { get; set; }
        public List<BaseData> results { get; set; }
    }
}
