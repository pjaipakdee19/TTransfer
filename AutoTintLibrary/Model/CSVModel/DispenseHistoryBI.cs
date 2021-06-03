using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTintLibrary
{
    public class ListDispenseHistoryBI
    {
        public List<DispenseHistoryBI> DispenseHistoriesBI { get; set; }
    }
    public class DispenseHistoryBI
    {
        [Name("RECORD_KEY")]
        public string record_key { get; set; }
        [Name("DISPENSED_DATE")]
        public string dispensed_date { get; set; }
        [Name("Date")]
        public string date { get; set; }
        [Name("WANTED_AMOUNT")]
        public string wanted_amount { get; set; }
        [Name("UNIT_NAME")]
        public string unit_name { get; set; }
        [Name("DB_NAME")]
        public string db_name { get; set; }
        [Name("PRODUCT_NAME")]
        public string product_name { get; set; }
        [Name("PRODUCT_CODE")]
        public string product_code { get; set; }
        [Name("COLOUR_CODE")]
        public string colour_code { get; set; }
        [Name("COLOR_NAME")]
        public string color_name { get; set; }
        [Name("COLLECTION_NAME")]
        public string collection_name { get; set; }
        [Name("BASE_NAME")]
        public string base_name { get; set; }
        [Name("BASE")]
        public string base_value { get; set; }
        [Name("PRICE")]
        public string price { get; set; }
        [Name("BASE_PRICE")]
        public string base_price { get; set; }
        [Name("COLORANT_PRICE")]
        public string colorant_price { get; set; }
        [Name("COMPANY_NAME")]
        public string company_name { get; set; }
        [Name("COMPANY_CODE")]
        public string company_code { get; set; }
        [Name("NUMBER_OF_UNITS")]
        public string number_of_units { get; set; }

        [Name("CUSTOMER_KEY")]
        public string customer_key { get; set; }
        [Name("DISTRIBUTION_CHANNEL")]
        public string distribution_channel { get; set; }
        [Name("SALE_OFFICE")]
        public string sale_office { get; set; }
        [Name("SALE_GROUP")]
        public string sale_group { get; set; }
        [Name("SALE_DISTRICT")]
        public string sale_district { get; set; }
        [Name("BUYING_GROUP_TEXT_KEY")]
        public string buying_group_text_key { get; set; }
        [Name("MATERIAL_PF_CODE")]
        public string material_pf_code { get; set; }
        [Name("MARKETING_GROUP")]
        public string marketing_group { get; set; }
        [Name("SUB_MARKETING")]
        public string sub_marketing { get; set; }
        [Name("PRODUCT_SEGMENT")]
        public string product_segment { get; set; }
        [Name("BRAND")]
        public string brand { get; set; }
        [Name("TYPE")]
        public string type { get; set; }
        [Name("FUNCTION")]
        public string function { get; set; }
        [Name("MATERIAL_GROUP5")]
        public string material_group5 { get; set; }
        [Name("COMPONENT_NAME")]
        public string component_name { get; set; }
        [Name("CW01LV")]
        public string cw01lv { get; set; }
        [Name("CW02LV")]
        public string cw02lv { get; set; }
        [Name("CW03LV")]
        public string cw03lv { get; set; }
        [Name("CW04LV")]
        public string cw04lv { get; set; }
        [Name("CW05LV")]
        public string cw05lv { get; set; }
        [Name("CW06LV")]
        public string cw06lv { get; set; }
        [Name("CW07LV")]
        public string cw07lv { get; set; }
        [Name("CW08LV")]
        public string cw08lv { get; set; }
        [Name("CW09LV")]
        public string cw09lv { get; set; }
        [Name("CW10LV")]
        public string cw10lv { get; set; }
        [Name("CW11LV")]
        public string cw11lv { get; set; }
        [Name("CW12LV")]
        public string cw12lv { get; set; }
        [Name("CW13LV")]
        public string cw13lv { get; set; }
        [Name("CW14LV")]
        public string cw14lv { get; set; }
        [Name("CW15LV")]
        public string cw15lv { get; set; }
        [Name("CW16LV")]
        public string cw16lv { get; set; }
        [Name("CW17LV")]
        public string cw17lv { get; set; }
        [Name("CW18LV")]
        public string cw18lv { get; set; }
        [Name("CW19LV")]
        public string cw19lv { get; set; }
        [Name("CW20LV")]
        public string cw20lv { get; set; }
        [Name("CWTOTAL")]
        public string cwtotal { get; set; }
        [Name("SALE_OUT_QUANTITY_GL")]
        public string sale_out_quantity_gl { get; set; }
        [Name("SALE_OUT_QUANTITY_L")]
        public string sale_out_quantity_l { get; set; }
        [Name("SALE_OUT_QUANTITY_EA")]
        public string sale_out_quantity_ea { get; set; }
        [Name("SALE_OUT_AMT_THB")]
        public string sale_out_amt_thb { get; set; }
        [Name("BRANCH_NAME")]
        public string branch_name { get; set; }
        [Name("BRANCH_CODE")]
        public string branch_code { get; set; }
        [Name("SELL_TYPE")]
        public string sell_type { get; set; }
    }
}
