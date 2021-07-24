using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        [Index(0)]
        [Name("RECORD_KEY")]
        public string record_key { get; set; }
        [Index(1)]
        [Name("DISPENSED_DATE")]
        public string dispensed_date { get; set; }
        [Index(2)]
        [Name("Date")]
        public string date { get; set; }
        [Index(3)]
        [Name("WANTED_AMOUNT")]
        public string wanted_amount { get; set; }
        [Index(4)]
        [Name("UNIT_NAME")]
        public string unit_name { get; set; }
        [Index(5)]
        [Name("DB_NAME")]
        public string db_name { get; set; }
        [Index(6)]
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
        [Name("BASE_VALUE")]
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
        [Name("DISPENSER_NO")]
        public string dispenser_no { get; set; }
        [Name("CUSTOMER_KEY")]
        public string customer_key { get; set; }
        [Name("MATERIAL_PF_CODE")]
        public string material_pf_code { get; set; }
        [Name("TYPE")]
        public string type { get; set; }
        [Name("COMPONENT_NAME")]
        public string component_name { get; set; }
        [Name("CW01_LV")]
        public string cw01_lv { get; set; }
        [Name("CW02_LV")]
        public string cw02_lv { get; set; }
        [Name("CW03_LV")]
        public string cw03_lv { get; set; }
        [Name("CW04_LV")]
        public string cw04_lv { get; set; }
        [Name("CW05_LV")]
        public string cw05_lv { get; set; }
        [Name("CW06_LV")]
        public string cw06_lv { get; set; }
        [Name("CW07_LV")]
        public string cw07_lv { get; set; }
        [Name("CW08_LV")]
        public string cw08_lv { get; set; }
        [Name("CW09_LV")]
        public string cw09_lv { get; set; }
        [Name("CW10_LV")]
        public string cw10_lv { get; set; }
        [Name("CW11_LV")]
        public string cw11_lv { get; set; }
        [Name("CW12_LV")]
        public string cw12_lv { get; set; }
        [Name("CW13_LV")]
        public string cw13_lv { get; set; }
        [Name("CW14_LV")]
        public string cw14_lv { get; set; }
        [Name("CW15_LV")]
        public string cw15_lv { get; set; }
        [Name("CW16_LV")]
        public string cw16_lv { get; set; }
        [Name("CW17_LV")]
        public string cw17_lv { get; set; }
        [Name("CW18_LV")]
        public string cw18_lv { get; set; }
        [Name("CW19_LV")]
        public string cw19_lv { get; set; }
        [Name("CW20_LV")]
        public string cw20_lv { get; set; }
        [Name("CW_TOTAL")]
        public string cw_total { get; set; }
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
        [Name("STATUS_SHADE")]
        public string status_shade { get; set; }

        public object this[string propertyName]
        {
            get
            {
                Type myType = typeof(DispenseHistoryBI);
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                return myPropInfo.GetValue(this, null);
            }
            set
            {
                Type myType = typeof(DispenseHistoryBI);
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                myPropInfo.SetValue(this, value, null);
            }
        }
    }
}
