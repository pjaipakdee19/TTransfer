using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppDotNetFW
{
    public class ListDispenseHistory
    {
        public List<DispenseHistory> DispenseHistories { get; set; }
    }
    public class DispenseHistory
    {
        [Name("DISPENSED_FORMULA_ID")]
        public int dispensed_formula_id { get; set; }
        [Name("DISPENSED_DATE")]
        public string dispensed_date { get; set; }
        [Name("WANTED_AMOUNT")]
        public string wanted_amount { get; set; }
        [Name("UNIT_NAME")]
        public string unit_name { get; set; }
        [Name("MODIFIED")]
        public string modified { get; set; }
        [Name("ORIGINAL_TYPE")]
        public string original_type { get; set; }
        [Name("DB_NAME")]
        public string db_name { get; set; }
        [Name("PRODUCT_NAME")]
        public string product_name { get; set; }
        [Name("PRODUCT_CODE")]
        public string product_code { get; set; }

        [Name("FILTER_GROUP")]
        public string filter_group { get; set; }
        [Name("COLOUR_CODE")]
        public string colour_code { get; set; }
        [Name("COLOR_NAME")]
        public string color_name { get; set; }
        [Name("COLLECTION_NAME")]
        public string collection_name { get; set; }
        [Name("COLLECTION_CODE")]
        public string collection_code { get; set; }
        [Name("FILTER_GROUP1")]
        public string filter_group1 { get; set; }
        [Name("RGB")]
        public string rgb { get; set; }
        [Name("ORDER_NUMBER")]
        public string order_number { get; set; }
        [Name("REFERENCE")]
        public string reference { get; set; }
        [Name("DELIVERY_DATE")]
        public string delivery_date { get; set; }
        [Name("ORDER_REMARK")]
        public string order_remark { get; set; }
        [Name("FORMULA_DATE")]
        public string formula_date { get; set; }
        [Name("SUBSTRATE")]
        public string substrate { get; set; }
        [Name("COMMENTS")]
        public string comments { get; set; }
        [Name("COMMENTS1")]
        public string comments1 { get; set; }
        [Name("COMMENTS2")]
        public string comments2 { get; set; }
        [Name("WARNING_MESSAGE")]
        public string warning_message { get; set; }
        [Name("OWN_CREATE_DATE")]
        public string own_create_date { get; set; }
        [Name("BARCODE")]
        public string barcode { get; set; }
        [Name("BASE_NAME")]
        public string base_name { get; set; }
        [Name("CUSTOMER_NAME")]
        public string customer_name { get; set; }
        [Name("ZIPCODE")]
        public string zipcode { get; set; }
        [Name("CITY")]
        public string city { get; set; }
        [Name("PHONE")]
        public string phone { get; set; }
        [Name("NOTE")]
        public string note { get; set; }
        [Name("DISCOUNT")]
        public string discount { get; set; }
        [Name("EMAIL")]
        public string email { get; set; }
        [Name("LAB")]
        public string lab { get; set; }
        [Name("L")]
        public string l { get; set; }
        [Name("A")]
        public string a { get; set; }
        [Name("B")]
        public string b { get; set; }
        [Name("SORT_FIELD")]
        public string sort_field { get; set; }
        [Name("GLOSSVALUE")]
        public string glossvalue { get; set; }
        [Name("REFLECTANCE1")]
        public string reflectance1 { get; set; }
        [Name("Y")]
        public string y { get; set; }
        [Name("REFLECTANCE2")]
        public string reflectance2 { get; set; }
        [Name("REFLECTANCE3")]
        public string reflectance3 { get; set; }
        [Name("REFLECTANCE4")]
        public string reflectance4 { get; set; }
        [Name("REFLECTANCE5")]
        public string reflectance5 { get; set; }
        [Name("REFLECTANCE6")]
        public string reflectance6 { get; set; }
        [Name("REFLECTANCE7")]
        public string reflectance7 { get; set; }
        [Name("REFLECTANCE8")]
        public string reflectance8 { get; set; }
        [Name("REFLECTANCE9")]
        public string reflectance9 { get; set; }
        [Name("REFLECTANCE10")]
        public string reflectance10 { get; set; }
        [Name("REFLECTANCE11")]
        public string reflectance11 { get; set; }
        [Name("REFLECTANCE12")]
        public string reflectance12 { get; set; }
        [Name("REFLECTANCE13")]
        public string reflectance13 { get; set; }
        [Name("REFLECTANCE14")]
        public string reflectance14 { get; set; }
        [Name("REFLECTANCE15")]
        public string reflectance15 { get; set; }
        [Name("REFLECTANCE16")]
        public string reflectance16 { get; set; }
        [Name("REFLECTANCE17")]
        public string reflectance17 { get; set; }
        [Name("REFLECTANCE18")]
        public string reflectance18 { get; set; }
        [Name("REFLECTANCE19")]
        public string reflectance19 { get; set; }
        [Name("REFLECTANCE20")]
        public string reflectance20 { get; set; }
        [Name("REFLECTANCE21")]
        public string reflectance21 { get; set; }
        [Name("REFLECTANCE22")]
        public string reflectance22 { get; set; }
        [Name("REFLECTANCE23")]
        public string reflectance23 { get; set; }
        [Name("REFLECTANCE24")]
        public string reflectance24 { get; set; }
        [Name("REFLECTANCE25")]
        public string reflectance25 { get; set; }
        [Name("REFLECTANCE26")]
        public string reflectance26 { get; set; }
        [Name("REFLECTANCE27")]
        public string reflectance27 { get; set; }
        [Name("REFLECTANCE28")]
        public string reflectance28 { get; set; }
        [Name("REFLECTANCE29")]
        public string reflectance29 { get; set; }
        [Name("REFLECTANCE30")]
        public string reflectance30 { get; set; }
        [Name("REFLECTANCE31")]
        public string reflectance31 { get; set; }
        [Name("REFLECTANCE32")]
        public string reflectance32 { get; set; }
        [Name("EXTRA_DISCOUNT")]
        public string extra_discount { get; set; }
        [Name("PRICE")]
        public string price { get; set; }
        [Name("BASE_PRICE")]
        public string base_price { get; set; }
        [Name("COLORANT_PRICE")]
        public string colorant_price { get; set; }
        [Name("VAT_PERCENT")]
        public string vat_percent { get; set; }
        [Name("EXTRA_COMMENT")]
        public string extra_comment { get; set; }
        [Name("SURCHARGE")]
        public string surcharge { get; set; }
        [Name("FORMULA_BARCODE")]
        public string formula_barcode { get; set; }
        [Name("QRCODEL1")]
        public string qrcodel1 { get; set; }
        [Name("QRCODEL2")]
        public string qrcodel2 { get; set; }
        [Name("QRCODEL3")]
        public string qrcodel3 { get; set; }
        [Name("QRCODEL4")]
        public string qrcodel4 { get; set; }
        [Name("QRCODEL5")]
        public string qrcodel5 { get; set; }
        [Name("QRCODEL6")]
        public string qrcodel6 { get; set; }
        [Name("QRCODEL7")]
        public string qrcodel7 { get; set; }
        [Name("QRCODEL8")]
        public string qrcodel8 { get; set; }
        [Name("QRCODEL9")]
        public string qrcodel9 { get; set; }
        [Name("QRCODEL10")]
        public string qrcodel10 { get; set; }
        [Name("QRCODEL11")]
        public string qrcodel11 { get; set; }
        [Name("QRCODEL12")]
        public string qrcodel12 { get; set; }
        [Name("QRCODEL13")]
        public string qrcodel13 { get; set; }
        [Name("QRCODEL14")]
        public string qrcodel14 { get; set; }
        [Name("QRCODEL15")]
        public string qrcodel15 { get; set; }
        [Name("COMPANY_NAME")]
        public string company_name { get; set; }
        [Name("COMPANY_CODE")]
        public string company_code { get; set; }
        [Name("MATERIAL_PF_CODE")]
        public string material_pf_code { get; set; }
        [Name("COMPONENT_NAME1")]
        public string component_name1 { get; set; }
        [Name("DURATION_MILLISECONDS1")]
        public string duration_milliseconds1 { get; set; }
        [Name("LINES_WANTED_AMOUNT1")]
        public string lines_wanted_amount1 { get; set; }
        [Name("LINES_DISPENSED_AMOUNT1")]
        public string lines_dispensed_amount1 { get; set; }
        [Name("COMPONENT_NAME2")]
        public string component_name2 { get; set; }
        [Name("DURATION_MILLISECONDS2")]
        public string duration_milliseconds2 { get; set; }
        [Name("LINES_WANTED_AMOUNT2")]
        public string lines_wanted_amount2 { get; set; }
        [Name("LINES_DISPENSED_AMOUNT2")]
        public string lines_dispensed_amount2 { get; set; }
        [Name("COMPONENT_NAME3")]
        public string component_name3 { get; set; }
        [Name("DURATION_MILLISECONDS3")]
        public string duration_milliseconds3 { get; set; }
        [Name("LINES_WANTED_AMOUNT3")]
        public string lines_wanted_amount3 { get; set; }
        [Name("LINES_DISPENSED_AMOUNT3")]
        public string lines_dispensed_amount3 { get; set; }
        [Name("COMPONENT_NAME4")]
        public string component_name4 { get; set; }
        [Name("DURATION_MILLISECONDS4")]
        public string duration_milliseconds4 { get; set; }
        [Name("LINES_WANTED_AMOUNT4")]
        public string lines_wanted_amount4 { get; set; }
        [Name("LINES_DISPENSED_AMOUNT4")]
        public string lines_dispensed_amount4 { get; set; }
        [Name("COMPONENT_NAME5")]
        public string component_name5 { get; set; }
        [Name("DURATION_MILLISECONDS5")]
        public string duration_milliseconds5 { get; set; }
        [Name("LINES_WANTED_AMOUNT5")]
        public string lines_wanted_amount5 { get; set; }
        [Name("LINES_DISPENSED_AMOUNT5")]
        public string lines_dispensed_amount5 { get; set; }
        [Name("COMPONENT_NAME6")]
        public string component_name6 { get; set; }
        [Name("DURATION_MILLISECONDS6")]
        public string duration_milliseconds6 { get; set; }
        [Name("LINES_WANTED_AMOUNT6")]
        public string lines_wanted_amount6 { get; set; }
        [Name("LINES_DISPENSED_AMOUNT6")]
        public string lines_dispensed_amount6 { get; set; }
    }
}
