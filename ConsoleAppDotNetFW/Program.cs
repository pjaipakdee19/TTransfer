using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using UnitsNet;
using System;
using System.Runtime;
using System.Runtime.InteropServices;

using AutoTintLibrary;
using System.Reflection;

namespace ConsoleAppDotNetFW
{
    class Program
    {
        
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
        };

        //Creating the extern function...  
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);
        [Flags]
        enum ConnectionStates
        {
            Modem = 0x1,
            LAN = 0x2,
            Proxy = 0x4,
            RasInstalled = 0x10,
            Offline = 0x20,
            Configured = 0x40,
        }

        public static async Task Main(string[] args)
        {
            int status_code;
            int connection_retry = 1;
            bool isConnected = false;
            while (connection_retry < 5 && !isConnected)
            {
                isConnected = InternetGetConnectedState(out status_code, 0);
                Console.WriteLine(string.Format("Is connected :{0} Flags:{1}", isConnected, status_code));
                connection_retry++;
                if (!isConnected) System.Threading.Thread.Sleep(5000);
            }

            string[] date = "21/10/2532 15:30:16".Split(' ');
            int year = int.Parse(date[0].Split('/')[2]);
            int now = DateTime.Today.Year;
            if(year > now)
            {
                date[0] = $"{date[0].Split('/')[1]}/{date[0].Split('/')[0]}/{year-543}";
            }



            //string file_path = @"E:\Tutorial\json_dispense_log\full_dispense_log_21_10_2015_p2_p2.json";
            string file_path = @"E:\Tutorial\json_dispense_log\full_dispense_log_21_10_2015_p2_test.json";
            string streamFile = File.ReadAllText(file_path);
            dynamic details = JArray.Parse(streamFile);
            //dynamic stuff = JsonConvert.DeserializeObject<ListDispenseHistory>(details);
            var exportRecordBI = new List<DispenseHistoryBI>();
            //foreach (DispenseHistory history in stuff)
            //{
            //    var export_bi = new DispenseHistoryBI();

            //    export_bi.dispensed_date = history.dispensed_date; //convert to BE.
            //    export_bi.date = history.dispensed_date; //convert to yyyymmdd

            dynamic client = new RestClient();
            var latest_dispense_date = await APIHelper.GetLatestDispenseRecord(client, "11016469",$"/dispense_history/last_uploaded/");
            var test = await APIHelper.RequestGet(client, "/auto_tint/");
            //    exportRecordBI.Append(export_bi);
            //}
            foreach (dynamic detail in details)
            {
                //Console.WriteLine(detail);
                var export_bi = new DispenseHistoryBI();

                //Do a formatted date
                String[] dispense_date = detail["dispensed_date"].ToString().Split(' ');
                string formattedDate = "";
                DateTime parsedDateTime;
                if (DateTime.TryParseExact(dispense_date[0], "dd/mm/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out parsedDateTime))
                {
                    formattedDate = parsedDateTime.ToString("yyyymmdd");
                }
                else
                {
                    Console.WriteLine("Parsing failed");
                }

                //Do a dispenser_no,customer_key
                String[] sp_company_code = detail["company_code"].ToString().Split(new String[] { "AT" }, StringSplitOptions.None);
                String dispenser_no, customer_key = "";
                if (sp_company_code.Length > 1)
                {
                    dispenser_no = $"{sp_company_code[1]}";
                }
                else
                {
                    dispenser_no = "01";
                }
                customer_key = sp_company_code[0];

                //Do an cwlv
                //export_bi["cw01_lv"] = "";
                int component_qty = 20;
                for (int i = 1; i <= component_qty; i++)
                {
                    String numStr = i.ToString();
                    if (numStr.Length <= 1)
                    {
                        numStr = $"0{numStr}";
                    }
                    if (detail[$"component_name{i}"] == null)
                    {
                        export_bi[$"cw{numStr}_lv"] = "";
                    }
                    else
                    {
                        if (detail[$"component_name{i}"].ToString().IndexOf("CW") >= 0)
                        {
                            export_bi[$"cw{numStr}_lv"] = detail[$"lines_wanted_amount{i}"].ToString();
                            Double total, lines_wanted_amount = 0;
                            Double.TryParse((export_bi["cw_total"] != null) ? export_bi["cw_total"].ToString() : "0", out total);
                            Double.TryParse((detail[$"lines_wanted_amount{i}"] != null) ? detail[$"lines_wanted_amount{i}"].ToString() : "0", out lines_wanted_amount);
                            total = total + lines_wanted_amount;
                            export_bi["cw_total"] = total.ToString();
                        }
                        else
                        {
                            export_bi[$"cw{numStr}_lv"] = "";
                        }
                    }
                }

                //Convert a sales out qty (gl)
                double amount = (double)detail["wanted_amount"];
                string libUnits = detail["unit_name"].ToString(); //Quart => UsQuart , GL,Gallon => UsGallon , Liter => Liter , ml => Milliliter
                if (libUnits.IndexOf("Quart") >= 0) libUnits = "UsQuart";
                if (libUnits.IndexOf("Gallon") >= 0) libUnits = "UsGallon";
                if (libUnits.IndexOf("GL") >= 0) libUnits = "UsGallon";
                if (libUnits.IndexOf("Liter") >= 0) libUnits = "Liter";
                if (libUnits.IndexOf("ml") >= 0) libUnits = "Milliliter";
                string sales_out_qty_gl = UnitConverter.ConvertByName(amount, "Volume", libUnits, "UsGallon").ToString();
                string sales_out_qty_l = UnitConverter.ConvertByName(amount, "Volume", libUnits, "Liter").ToString();

                //sale_out_amt_thb
                double base_price = (double)detail["base_price"];
                double colorant_price = (double)detail["colorant_price"];
                double sale_out_amt_thb = Math.Ceiling((base_price + (base_price * 7 / 100)) + (colorant_price + (colorant_price * 7 / 100)));

                //getSaleType
                string saleType = "";
                if ((double)detail["base_price"] > 0)
                {
                    saleType = detail["base_price"].ToString();
                }
                else
                {
                    saleType = detail["colorant_price"].ToString();
                }

                //status shade
                /*
                 * Complete : LINES_WANTED_AMOUNT กับ LINES_DISPENSED_AMOUNT ของทุก component ต้องเท่ากัน ยกเว้นแม่สี ที่จะเป็น 0 เสมอ
                Error  : LINES_WANTED_AMOUNT กับ LINES_DISPENSED_AMOUNT ของ component ไม่เท่ากัน แค่มี 1 อันไม่เท่า ก็เป็น Error เลย
                View  :  LINES_DISPENSED_AMOUNT ของทุก component เป็น 0 สถานะนี้คือเรียกดูสูตรการผสมเฉยๆ ไม่มีการฉีดสีลงไปผสม
                 * 
                 */
                string status_shade = "Complete";
                bool errorFlag = false;
                bool viewFlag = true;
                for (int i = 1; i <= component_qty; i++)
                {
                    String numStr = i.ToString();
                    if (detail[$"lines_wanted_amount{i}"] == null || detail[$"lines_dispensed_amount{i}"] == null)
                    {
                        continue;
                    }
                    else
                    {
                        if (detail[$"lines_wanted_amount{i}"].ToString() != detail[$"lines_dispensed_amount{i}"].ToString()) errorFlag = true;
                        if (detail[$"lines_dispensed_amount{i}"] > 0) viewFlag = false;
                    }

                }
                if (errorFlag) status_shade = "Error";
                if (viewFlag) status_shade = "View";

                export_bi.dispensed_date = detail["dispensed_date"]; //convert to BE.
                export_bi.date = formattedDate;
                export_bi.record_key = detail["dispensed_formula_id"] + formattedDate + detail["company_code"];
                export_bi.wanted_amount = detail["wanted_amount"];
                export_bi.unit_name = detail["unit_name"];
                export_bi.db_name = detail["db_name"];
                export_bi.product_name = detail["product_name"];
                export_bi.product_code = detail["product_code"];
                export_bi.colour_code = detail["colour_code"];
                export_bi.color_name = detail["color_name"];
                export_bi.collection_name = detail["collection_name"];
                export_bi.base_name = detail["base_name"];
                export_bi.base_value = detail["base_name"].ToString().Substring(detail["base_name"].ToString().Length - (detail["base_name"].ToString().IndexOf("Base") + 3));
                export_bi.price = detail["price"];
                export_bi.base_price = detail["base_price"];
                export_bi.colorant_price = detail["colorant_price"];
                export_bi.company_name = detail["company_name"];
                export_bi.company_code = detail["company_code"];
                export_bi.dispenser_no = dispenser_no;
                export_bi.customer_key = customer_key;
                export_bi.material_pf_code = detail["material_pf_code"];
                export_bi.component_name = "";
                export_bi.sale_out_quantity_gl = sales_out_qty_gl;
                export_bi.sale_out_quantity_l = sales_out_qty_l;
                export_bi.sale_out_quantity_ea = "1";
                export_bi.sale_out_amt_thb = sale_out_amt_thb.ToString();
                export_bi.branch_code = "";
                export_bi.branch_name = "";
                export_bi.type = saleType;
                export_bi.status_shade = status_shade;

            }
            //Console.WriteLine(string.Concat("Hi ", details["FirstName"], " " + details["LastName"]));
            //Console.WriteLine(details);
            Console.ReadLine();
        }
    }
}