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
        [Name("RECORD KEY")]
        public string record_key { get; set; }
        [Index(1)]
        [Name("DISPENSED_DATE")]
        public string dispensed_date { get; set; }
        [Index(2)]
        [Name("DATE")]
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
        [Name("Base")]
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
        [Name("CUSTOMER KEY")]
        public string customer_key { get; set; }
        [Index(20)]
        [Name("Distribution Channel")]
        public string distribution_channel { get; set; }
        [Index(25)]
        [Name("MATERIAL_PF_CODE")]
        public string material_pf_code { get; set; }
        [Index(33)]
        [Name("COMPONENT_NAME")]
        public string component_name { get; set; }
        [Index(34)]
        [Name("CW01 LV")]
        public string cw01_lv { get; set; }
        [Index(35)]
        [Name("CW02 LV")]
        public string cw02_lv { get; set; }
        [Index(36)]
        [Name("CW03 LV")]
        public string cw03_lv { get; set; }
        [Index(37)]
        [Name("CW04 LV")]
        public string cw04_lv { get; set; }
        [Index(38)]
        [Name("CW05 LV")]
        public string cw05_lv { get; set; }
        [Index(39)]
        [Name("CW06 LV")]
        public string cw06_lv { get; set; }
        [Index(40)]
        [Name("CW07 LV")]
        public string cw07_lv { get; set; }
        [Index(41)]
        [Name("CW08 LV")]
        public string cw08_lv { get; set; }
        [Index(42)]
        [Name("CW09 LV")]
        public string cw09_lv { get; set; }
        [Index(43)]
        [Name("CW10 LV")]
        public string cw10_lv { get; set; }
        [Index(44)]
        [Name("CW11 LV")]
        public string cw11_lv { get; set; }
        [Index(45)]
        [Name("CW12 LV")]
        public string cw12_lv { get; set; }
        [Index(46)]
        [Name("CW13 LV")]
        public string cw13_lv { get; set; }
        [Index(47)]
        [Name("CW14 LV")]
        public string cw14_lv { get; set; }
        [Index(48)]
        [Name("CW15 LV")]
        public string cw15_lv { get; set; }
        [Index(49)]
        [Name("CW16 LV")]
        public string cw16_lv { get; set; }
        [Index(50)]
        [Name("CW17 LV")]
        public string cw17_lv { get; set; }
        [Index(51)]
        [Name("CW18 LV")]
        public string cw18_lv { get; set; }
        [Index(52)]
        [Name("CW19 LV")]
        public string cw19_lv { get; set; }
        [Index(53)]
        [Name("CW20 LV")]
        public string cw20_lv { get; set; }
        [Index(54)]
        [Name("CW Total")]
        public string cw_total { get; set; }
        [Index(55)]
        [Name("Sale Out Quantity  (GL)")]
        public string sale_out_quantity_gl { get; set; }
        [Index(56)]
        [Name("Sale Out Quantity (L)")]
        public string sale_out_quantity_l { get; set; }
        [Index(57)]
        [Name("Sale Out Quantity (EA)")]
        public string sale_out_quantity_ea { get; set; }
        [Index(58)]
        [Name("Sale out Amt (THB)")]
        public string sale_out_amt_thb { get; set; }
        [Index(59)]
        [Name("BRANCH NAME")]
        public string branch_name { get; set; }
        [Index(60)]
        [Name("BRANCH CODE")]
        public string branch_code { get; set; }
        [Index(61)]
        [Name("TYPE SHOP")]
        public string type { get; set; }
        [Index(62)]
        [Name("STATUS SHADE")]
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
