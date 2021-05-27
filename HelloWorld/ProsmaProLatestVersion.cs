using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    // For API GET /prisma_pro/{id}/lastest_version
    public class PrismaProLatestVersion
    {
        public int id { get; set; }
        public string version_name { get; set; }
        public string number { get; set; }
        public string file { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }


}
