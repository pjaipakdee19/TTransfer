using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppDotNetFW
{
    public class DispenseHistory
    {
        [Name("DISPENSED_FORMULA_ID")]
        public int DISPENSED_FORMULA_ID { get; set; }
        [Name("DISPENSED_DATE")]
        public string DISPENSED_DATE { get; set; }
        [Name("WANTED_AMOUNT")]
        public string WANTED_AMOUNT { get; set; }
        [Name("UNIT_NAME")]
        public string UNIT_NAME { get; set; }
        [Name("MODIFIED")]
        public string MODIFIED { get; set; }
        [Name("ORIGINAL_TYPE")]
        public string ORIGINAL_TYPE { get; set; }
        [Name("DB_NAME")]
        public string DB_NAME { get; set; }
        [Name("PRODUCT_NAME")]
        public string PRODUCT_NAME { get; set; }
        [Name("PRODUCT_CODE")]
        public string PRODUCT_CODE { get; set; }

        [Name("FILTER_GROUP")]
        public string FILTER_GROUP { get; set; }
        [Name("COLOUR_CODE")]
        public string COLOUR_CODE { get; set; }
        [Name("COLOR_NAME")]
        public string COLOR_NAME { get; set; }
        [Name("COLLECTION_NAME")]
        public string COLLECTION_NAME { get; set; }
        [Name("COLLECTION_CODE")]
        public string COLLECTION_CODE { get; set; }
        [Name("FILTER_GROUP1")]
        public string FILTER_GROUP1 { get; set; }
        [Name("RGB")]
        public string RGB { get; set; }
        [Name("ORDER_NUMBER")]
        public string ORDER_NUMBER { get; set; }
        [Name("REFERENCE")]
        public string REFERENCE { get; set; }
        [Name("DELIVERY_DATE")]
        public string DELIVERY_DATE { get; set; }
        [Name("MODIFIED")]
        public string Modified { get; set; }
        [Name("ORDER_REMARK")]
        public string ORDER_REMARK { get; set; }
        [Name("FORMULA_DATE")]
        public string FORMULA_DATE { get; set; }
        [Name("SUBSTRATE")]
        public string SUBSTRATE { get; set; }
        [Name("COMMENTS")]
        public string COMMENTS { get; set; }
        [Name("COMMENTS1")]
        public string COMMENTS1 { get; set; }
        [Name("COMMENTS2")]
        public string COMMENTS2 { get; set; }
        [Name("WARNING_MESSAGE")]
        public string WARNING_MESSAGE { get; set; }
        [Name("OWN_CREATE_DATE")]
        public string OWN_CREATE_DATE { get; set; }
        [Name("BARCODE")]
        public string BARCODE { get; set; }
        [Name("BASE_NAME")]
        public string BASE_NAME { get; set; }
        [Name("CUSTOMER_NAME")]
        public string CUSTOMER_NAME { get; set; }
        [Name("ZIPCODE")]
        public string ZIPCODE { get; set; }
        [Name("CITY")]
        public string CITY { get; set; }
        [Name("PHONE")]
        public string PHONE { get; set; }
        [Name("NOTE")]
        public string NOTE { get; set; }
        [Name("DISCOUNT")]
        public string DISCOUNT { get; set; }
        [Name("EMAIL")]
        public string EMAIL { get; set; }
        [Name("LAB")]
        public string LAB { get; set; }
        [Name("L")]
        public string L { get; set; }
        [Name("A")]
        public string A { get; set; }
        [Name("B")]
        public string B { get; set; }
        [Name("SORT_FIELD")]
        public string SORT_FIELD { get; set; }
        [Name("GLOSSVALUE")]
        public string GLOSSVALUE { get; set; }
        [Name("REFLECTANCE1")]
        public string REFLECTANCE1 { get; set; }
        [Name("Y")]
        public string Y { get; set; }
        [Name("REFLECTANCE2")]
        public string REFLECTANCE2 { get; set; }
        [Name("REFLECTANCE3")]
        public string REFLECTANCE3 { get; set; }
        [Name("REFLECTANCE4")]
        public string REFLECTANCE4 { get; set; }
        [Name("REFLECTANCE5")]
        public string REFLECTANCE5 { get; set; }
        [Name("REFLECTANCE6")]
        public string REFLECTANCE6 { get; set; }
        [Name("REFLECTANCE7")]
        public string REFLECTANCE7 { get; set; }
        [Name("REFLECTANCE8")]
        public string REFLECTANCE8 { get; set; }
        [Name("REFLECTANCE9")]
        public string REFLECTANCE9 { get; set; }
        [Name("REFLECTANCE10")]
        public string REFLECTANCE10 { get; set; }
        [Name("REFLECTANCE11")]
        public string REFLECTANCE11 { get; set; }
        [Name("REFLECTANCE12")]
        public string REFLECTANCE12 { get; set; }
        [Name("REFLECTANCE13")]
        public string REFLECTANCE13 { get; set; }
        [Name("REFLECTANCE14")]
        public string REFLECTANCE14 { get; set; }
        [Name("REFLECTANCE15")]
        public string REFLECTANCE15 { get; set; }
        [Name("REFLECTANCE16")]
        public string REFLECTANCE16 { get; set; }
        [Name("REFLECTANCE17")]
        public string REFLECTANCE17 { get; set; }
        [Name("REFLECTANCE18")]
        public string REFLECTANCE18 { get; set; }
        [Name("REFLECTANCE19")]
        public string REFLECTANCE19 { get; set; }
        [Name("REFLECTANCE20")]
        public string REFLECTANCE20 { get; set; }
        [Name("REFLECTANCE21")]
        public string REFLECTANCE21 { get; set; }
        [Name("REFLECTANCE22")]
        public string REFLECTANCE22 { get; set; }
        [Name("REFLECTANCE23")]
        public string REFLECTANCE23 { get; set; }
        [Name("REFLECTANCE24")]
        public string REFLECTANCE24 { get; set; }
        [Name("REFLECTANCE25")]
        public string REFLECTANCE25 { get; set; }
        [Name("REFLECTANCE26")]
        public string REFLECTANCE26 { get; set; }
        [Name("REFLECTANCE27")]
        public string REFLECTANCE27 { get; set; }
        [Name("REFLECTANCE28")]
        public string REFLECTANCE28 { get; set; }
        [Name("REFLECTANCE29")]
        public string REFLECTANCE29 { get; set; }
        [Name("REFLECTANCE30")]
        public string REFLECTANCE30 { get; set; }
        [Name("REFLECTANCE31")]
        public string REFLECTANCE31 { get; set; }
        [Name("REFLECTANCE32")]
        public string REFLECTANCE32 { get; set; }
        [Name("EXTRA_DISCOUNT")]
        public string EXTRA_DISCOUNT { get; set; }
        [Name("PRICE")]
        public string PRICE { get; set; }
        [Name("BASE_PRICE")]
        public string BASE_PRICE { get; set; }
        [Name("COLORANT_PRICE")]
        public string COLORANT_PRICE { get; set; }
        [Name("VAT_PERCENT")]
        public string VAT_PERCENT { get; set; }
        [Name("EXTRA_COMMENT")]
        public string EXTRA_COMMENT { get; set; }
        [Name("SURCHARGE")]
        public string SURCHARGE { get; set; }
        [Name("FORMULA_BARCODE")]
        public string FORMULA_BARCODE { get; set; }
        [Name("QRCODEL1")]
        public string QRCODEL1 { get; set; }
        [Name("QRCODEL2")]
        public string QRCODEL2 { get; set; }
        [Name("QRCODEL3")]
        public string QRCODEL3 { get; set; }
        [Name("QRCODEL4")]
        public string QRCODEL4 { get; set; }
        [Name("QRCODEL5")]
        public string QRCODEL5 { get; set; }
        [Name("QRCODEL6")]
        public string QRCODEL6 { get; set; }
        [Name("QRCODEL7")]
        public string QRCODEL7 { get; set; }
        [Name("QRCODEL8")]
        public string QRCODEL8 { get; set; }
        [Name("QRCODEL9")]
        public string QRCODEL9 { get; set; }
        [Name("QRCODEL10")]
        public string QRCODEL10 { get; set; }
        [Name("QRCODEL11")]
        public string QRCODEL11 { get; set; }
        [Name("QRCODEL12")]
        public string QRCODEL12 { get; set; }
        [Name("QRCODEL13")]
        public string QRCODEL13 { get; set; }
        [Name("QRCODEL14")]
        public string QRCODEL14 { get; set; }
        [Name("QRCODEL15")]
        public string QRCODEL15 { get; set; }
        [Name("COMPANY_NAME")]
        public string COMPANY_NAME { get; set; }
        [Name("COMPANY_CODE")]
        public string COMPANY_CODE { get; set; }
        [Name("MATERIAL_PF_CODE")]
        public string MATERIAL_PF_CODE { get; set; }
        [Name("COMPONENT_NAME1")]
        public string COMPONENT_NAME1 { get; set; }
        [Name("DURATION_MILLISECONDS1")]
        public string DURATION_MILLISECONDS1 { get; set; }
        [Name("LINES_WANTED_AMOUNT1")]
        public string LINES_WANTED_AMOUNT1 { get; set; }
        [Name("LINES_DISPENSED_AMOUNT1")]
        public string LINES_DISPENSED_AMOUNT1 { get; set; }
        [Name("COMPONENT_NAME2")]
        public string COMPONENT_NAME2 { get; set; }
        [Name("DURATION_MILLISECONDS2")]
        public string DURATION_MILLISECONDS2 { get; set; }
        [Name("LINES_WANTED_AMOUNT2")]
        public string LINES_WANTED_AMOUNT2 { get; set; }
        [Name("LINES_DISPENSED_AMOUNT2")]
        public string LINES_DISPENSED_AMOUNT2 { get; set; }
        [Name("COMPONENT_NAME3")]
        public string COMPONENT_NAME3 { get; set; }
        [Name("DURATION_MILLISECONDS3")]
        public string DURATION_MILLISECONDS3 { get; set; }
        [Name("LINES_WANTED_AMOUNT3")]
        public string LINES_WANTED_AMOUNT3 { get; set; }
        [Name("LINES_DISPENSED_AMOUNT3")]
        public string LINES_DISPENSED_AMOUNT3 { get; set; }
        [Name("COMPONENT_NAME4")]
        public string COMPONENT_NAME4 { get; set; }
        [Name("DURATION_MILLISECONDS4")]
        public string DURATION_MILLISECONDS4 { get; set; }
        [Name("LINES_WANTED_AMOUNT4")]
        public string LINES_WANTED_AMOUNT4 { get; set; }
        [Name("LINES_DISPENSED_AMOUNT4")]
        public string LINES_DISPENSED_AMOUNT4 { get; set; }
        [Name("COMPONENT_NAME5")]
        public string COMPONENT_NAME5 { get; set; }
        [Name("DURATION_MILLISECONDS5")]
        public string DURATION_MILLISECONDS5 { get; set; }
        [Name("LINES_WANTED_AMOUNT5")]
        public string LINES_WANTED_AMOUNT5 { get; set; }
        [Name("LINES_DISPENSED_AMOUNT5")]
        public string LINES_DISPENSED_AMOUNT5 { get; set; }
        [Name("COMPONENT_NAME6")]
        public string COMPONENT_NAME6 { get; set; }
        [Name("DURATION_MILLISECONDS6")]
        public string DURATION_MILLISECONDS6 { get; set; }
        [Name("LINES_WANTED_AMOUNT6")]
        public string LINES_WANTED_AMOUNT6 { get; set; }
        [Name("LINES_DISPENSED_AMOUNT6")]
        public string LINES_DISPENSED_AMOUNT6 { get; set; }
    }
}
