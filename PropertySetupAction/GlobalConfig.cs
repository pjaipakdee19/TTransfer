using System;
using System.Collections.Generic;
using System.Text;

namespace PropertySetupAction
{
    class GlobalConfig
    {
        public string global_config_path { get; set; }
        public string auto_tint_id { get; set; }
        public string csv_history_path { get; set; }
        public string csv_history_achive_path { get; set; }
        public string json_dispense_log_path { get; set; }
        public string database_path { get; set; }
        public string programdata_log_path { get; set; }
        public string service_operation_start { get; set; }
        public string service_operation_stop { get; set; }
        public string start_random_minutes_threshold { get; set; }
        public string base_url { get; set; }

    }
}
