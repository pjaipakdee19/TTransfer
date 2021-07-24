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
        [Index(7)]
        [Name("PRODUCT_CODE")]
        public string product_code { get; set; }
        [Index(8)]
        [Name("COLOUR_CODE")]
        public string colour_code { get; set; }
        [Index(9)]
        [Name("COLOR_NAME")]
        public string color_name { get; set; }
        [Index(10)]
        [Name("COLLECTION_NAME")]
        public string collection_name { get; set; }
        [Index(11)]
        [Name("BASE_NAME")]
        public string base_name { get; set; }
        [Index(12)]
        [Name("BASE_VALUE")]
        public string base_value { get; set; }
        [Index(13)]
        [Name("PRICE")]
        public string price { get; set; }
        [Index(14)]
        [Name("BASE_PRICE")]
        public string base_price { get; set; }
        [Index(15)]
        [Name("COLORANT_PRICE")]
        public string colorant_price { get; set; }
        [Index(16)]
        [Name("COMPANY_NAME")]
        public string company_name { get; set; }
        [Index(17)]
        [Name("COMPANY_CODE")]
        public string company_code { get; set; }
        [Index(18)]
        [Name("DISPENSER_NO")]
        public string dispenser_no { get; set; }
        [Index(19)]
        [Name("CUSTOMER_KEY")]
        public string customer_key { get; set; }
        [Index(20)]
        [Name("MATERIAL_PF_CODE")]
        public string material_pf_code { get; set; }
        [Index(21)]
        [Name("TYPE")]
        public string type { get; set; }
        [Index(22)]
        [Name("COMPONENT_NAME")]
        public string component_name { get; set; }
        [Index(23)]
        [Name("CW01_LV")]
        public string cw01_lv { get; set; }
        [Index(24)]
        [Name("CW02_LV")]
        public string cw02_lv { get; set; }
        [Index(25)]
        [Name("CW03_LV")]
        public string cw03_lv { get; set; }
        [Index(26)]
        [Name("CW04_LV")]
        public string cw04_lv { get; set; }
        [Index(27)]
        [Name("CW05_LV")]
        public string cw05_lv { get; set; }
        [Index(28)]
        [Name("CW06_LV")]
        public string cw06_lv { get; set; }
        [Index(29)]
        [Name("CW07_LV")]
        public string cw07_lv { get; set; }
        [Index(30)]
        [Name("CW08_LV")]
        public string cw08_lv { get; set; }
        [Index(31)]
        [Name("CW09_LV")]
        public string cw09_lv { get; set; }
        [Index(32)]
        [Name("CW10_LV")]
        public string cw10_lv { get; set; }
        [Index(33)]
        [Name("CW11_LV")]
        public string cw11_lv { get; set; }
        [Index(34)]
        [Name("CW12_LV")]
        public string cw12_lv { get; set; }
        [Index(35)]
        [Name("CW13_LV")]
        public string cw13_lv { get; set; }
        [Index(36)]
        [Name("CW14_LV")]
        public string cw14_lv { get; set; }
        [Index(37)]
        [Name("CW15_LV")]
        public string cw15_lv { get; set; }
        [Index(38)]
        [Name("CW16_LV")]
        public string cw16_lv { get; set; }
        [Index(39)]
        [Name("CW17_LV")]
        public string cw17_lv { get; set; }
        [Index(40)]
        [Name("CW18_LV")]
        public string cw18_lv { get; set; }
        [Index(41)]
        [Name("CW19_LV")]
        public string cw19_lv { get; set; }
        [Index(42)]
        [Name("CW20_LV")]
        public string cw20_lv { get; set; }
        [Index(43)]
        [Name("CW_TOTAL")]
        public string cw_total { get; set; }
        [Index(44)]
        [Name("SALE_OUT_QUANTITY_GL")]
        public string sale_out_quantity_gl { get; set; }
        [Index(45)]
        [Name("SALE_OUT_QUANTITY_L")]
        public string sale_out_quantity_l { get; set; }
        [Index(46)]
        [Name("SALE_OUT_QUANTITY_EA")]
        public string sale_out_quantity_ea { get; set; }
        [Index(47)]
        [Name("SALE_OUT_AMT_THB")]
        public string sale_out_amt_thb { get; set; }
        [Index(48)]
        [Name("BRANCH_NAME")]
        public string branch_name { get; set; }
        [Index(49)]
        [Name("BRANCH_CODE")]
        public string branch_code { get; set; }
        [Index(50)]
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
