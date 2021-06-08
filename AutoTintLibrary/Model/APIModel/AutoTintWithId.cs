using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTintLibrary
{
    // AutoTintWithId myDeserializedClass = JsonConvert.DeserializeObject<AutoTintWithId>(myJsonResponse); 
    // Also use with /pos_history_update/
    public class Company
    {
        public int id { get; set; }
        public string company_code { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class PosSetting
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class PosSettingVersion
    {
        public int id { get; set; }
        public string version_name { get; set; }
        public double number { get; set; }
        public string file { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int setting_id { get; set; }
        public string setting_name { get; set; }
    }

    public class AutoTintWithId
    {
        public int id { get; set; }
        public Company company { get; set; }
        public PosSetting pos_setting { get; set; }
        public PosSettingVersion pos_setting_version { get; set; }
        public string auto_tint_id { get; set; }
        public string auto_tint_model_name { get; set; }
        public DateTime pos_history_retrieval_last_updated { get; set; }
        public string pos_history_retrieval_last_updated_filename { get; set; }
        public DateTime pos_setting_update_last_requested { get; set; }
        public DateTime pos_setting_update_last_updated { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }


}
