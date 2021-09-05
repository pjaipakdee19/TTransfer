using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTintLibrary
{
        public class CompanyV2
        {
            public int id { get; set; }
            public string company_code { get; set; }
            public string old_company_code { get; set; }
            public string name { get; set; }
            public object phone_number { get; set; }
            public object address { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
        }

        public class PosSettingV2
        {
            public int id { get; set; }
            public string name { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
        }

        public class AutoTintWithIdV2
    {
            public int id { get; set; }
            public CompanyV2 company { get; set; }
            public PosSettingV2 pos_setting { get; set; }
            public object pos_setting_version { get; set; }
            public string auto_tint_id { get; set; }
            public string auto_tint_model_name { get; set; }
            public string computer_model_name { get; set; }
            public string computer_os_name { get; set; }
            public object phone_number { get; set; }
            public object pos_history_retrieval_last_updated { get; set; }
            public string pos_history_retrieval_last_updated_filename { get; set; }
            public object pos_setting_update_last_requested { get; set; }
            public object pos_setting_update_last_updated { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public string com_code { get; set; }
            public string sales_org { get; set; }
        }


}
