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
        [Name("record_key")]
        public string record_key { get; set; }
        [Index(1)]
        [Name("dispensed_date")]
        public string dispensed_date { get; set; }
        [Index(2)]
        [Name("date")]
        public string date { get; set; }
        [Index(3)]
        [Name("wanted_amount")]
        public string wanted_amount { get; set; }
        [Index(4)]
        [Name("unit_name")]
        public string unit_name { get; set; }
        [Index(5)]
        [Name("db_name")]
        public string db_name { get; set; }
        [Index(6)]
        [Name("product_name")]
        public string product_name { get; set; }
        [Index(7)]
        [Name("product_code")]
        public string product_code { get; set; }
        [Index(8)]
        [Name("colour_code")]
        public string colour_code { get; set; }
        [Index(9)]
        [Name("color_name")]
        public string color_name { get; set; }
        [Index(10)]
        [Name("collection_name")]
        public string collection_name { get; set; }
        [Index(11)]
        [Name("base_name")]
        public string base_name { get; set; }
        [Index(12)]
        [Name("base_value")]
        public string base_value { get; set; }
        [Index(13)]
        [Name("price")]
        public string price { get; set; }
        [Index(14)]
        [Name("base_price")]
        public string base_price { get; set; }
        [Index(15)]
        [Name("colorant_price")]
        public string colorant_price { get; set; }
        [Index(16)]
        [Name("company_name")]
        public string company_name { get; set; }
        [Index(17)]
        [Name("company_code")]
        public string company_code { get; set; }
        [Index(18)]
        [Name("dispenser_no")]
        public string dispenser_no { get; set; }
        [Index(19)]
        [Name("customer_key")]
        public string customer_key { get; set; }
        [Index(20)]
        [Name("distribution_channel")]
        public string distribution_channel { get; set; }
        [Index(20)]
        [Name("sale_office")]
        public string sale_office { get; set; }
        [Index(20)]
        [Name("sale_group ")]
        public string sale_group { get; set; }
        [Index(20)]
        [Name("sale_district")]
        public string sale_district { get; set; }
        [Index(20)]
        [Name("buying_group")]
        public string buying_group { get; set; }
        [Index(20)]
        [Name("material_pf_code")]
        public string material_pf_code { get; set; }
        [Index(21)]
        [Name("marketing_group")]
        public string marketing_group { get; set; }
        [Index(21)]
        [Name("submarketing")]
        public string submarketing { get; set; }
        [Index(21)]
        [Name("product_segment")]
        public string product_segment { get; set; }
        [Index(21)]
        [Name("brand")]
        public string brand { get; set; }
        [Index(21)]
        [Name("type")]
        public string type { get; set; }
        [Index(22)]
        [Name("function")]
        public string function { get; set; }
        [Index(22)]
        [Name("material_group_5")]
        public string material_group_5 { get; set; }
        [Index(22)]
        [Name("component_name")]
        public string component_name { get; set; }
        [Index(23)]
        [Name("cw01_lv")]
        public string cw01_lv { get; set; }
        [Index(24)]
        [Name("cw02_lv")]
        public string cw02_lv { get; set; }
        [Index(25)]
        [Name("cw03_lv")]
        public string cw03_lv { get; set; }
        [Index(26)]
        [Name("cw04_lv")]
        public string cw04_lv { get; set; }
        [Index(27)]
        [Name("cw05_lv")]
        public string cw05_lv { get; set; }
        [Index(28)]
        [Name("cw06_lv")]
        public string cw06_lv { get; set; }
        [Index(29)]
        [Name("cw07_lv")]
        public string cw07_lv { get; set; }
        [Index(30)]
        [Name("cw08_lv")]
        public string cw08_lv { get; set; }
        [Index(31)]
        [Name("cw09_lv")]
        public string cw09_lv { get; set; }
        [Index(32)]
        [Name("cw10_lv")]
        public string cw10_lv { get; set; }
        [Index(33)]
        [Name("cw11_lv")]
        public string cw11_lv { get; set; }
        [Index(34)]
        [Name("cw12_lv")]
        public string cw12_lv { get; set; }
        [Index(35)]
        [Name("cw13_lv")]
        public string cw13_lv { get; set; }
        [Index(36)]
        [Name("cw14_lv")]
        public string cw14_lv { get; set; }
        [Index(37)]
        [Name("cw15_lv")]
        public string cw15_lv { get; set; }
        [Index(38)]
        [Name("cw16_lv")]
        public string cw16_lv { get; set; }
        [Index(39)]
        [Name("cw17_lv")]
        public string cw17_lv { get; set; }
        [Index(40)]
        [Name("cw18_lv")]
        public string cw18_lv { get; set; }
        [Index(41)]
        [Name("cw19_lv")]
        public string cw19_lv { get; set; }
        [Index(42)]
        [Name("cw20_lv")]
        public string cw20_lv { get; set; }
        [Index(43)]
        [Name("cw_total")]
        public string cw_total { get; set; }
        [Index(44)]
        [Name("sale_out_quantity_gl")]
        public string sale_out_quantity_gl { get; set; }
        [Index(45)]
        [Name("sale_out_quantity_l")]
        public string sale_out_quantity_l { get; set; }
        [Index(46)]
        [Name("sale_out_quantity_ea")]
        public string sale_out_quantity_ea { get; set; }
        [Index(47)]
        [Name("sale_out_amt_thb")]
        public string sale_out_amt_thb { get; set; }
        [Index(48)]
        [Name("branch_name")]
        public string branch_name { get; set; }
        [Index(49)]
        [Name("branch_code")]
        public string branch_code { get; set; }
        [Index(50)]
        [Name("status_shade")]
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
