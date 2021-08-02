using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTintLibrary
{
    public class Links
    {
        public object next { get; set; }
        public object previous { get; set; }
        public string last { get; set; }
    }

    public class Setting
    {
        public int id { get; set; }
        public string version_name { get; set; }
        public double number { get; set; }
        public string file { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class Result
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<Setting> settings { get; set; }
    }

    public class PrismaPro
    {
        public Links links { get; set; }
        public int count { get; set; }
        public int total_pages { get; set; }
        public List<Result> results { get; set; }
    }


}
