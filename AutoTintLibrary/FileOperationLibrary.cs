using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using CsvHelper;
using NLog;
using Newtonsoft.Json;
using CsvHelper.Configuration;
using System.Globalization;
using Newtonsoft.Json.Linq;
using UnitsNet;

namespace AutoTintLibrary
{
    public class FileOperationLibrary
    {
        private dynamic client = APIHelper.init();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
        };
        public async Task StartOperation()
        {
            //Init configuration variable
            string csv_history_path = ManageConfig.ReadGlobalConfig("csv_history_path");
            string jsonDispenseLogPath = ManageConfig.ReadGlobalConfig("json_dispense_log_path");
            string csv_history_achive_path = ManageConfig.ReadGlobalConfig("csv_history_achive_path");
            string auto_tint_id = ManageConfig.ReadGlobalConfig("auto_tint_id");
            //find the csv in history files
            DirectoryInfo csvHistoryPathInfo = new DirectoryInfo(csv_history_path);
            foreach (var csvFile in csvHistoryPathInfo.GetFiles("*.csv"))
            {
                var reader = new StreamReader(csvFile.FullName);
                var csv = new CsvReader(reader, csvConfig);
                var records = csv.GetRecords<DispenseHistory>().ToList();

                var dateList = new List<string>();
                //does csv file exist?
                //Extract the csv to json following DISPENSED_DATE
                //(New requirement) 27/06/2021 : extract if the dispensed date from Response of API /dispense_history/last_uploaded/ is earlier than the date in file.
                var latest_dispense_date = await APIHelper.RequestGet(client, $"/dispense_history/last_uploaded/?auto_tint_id={auto_tint_id}");
                //Get all date in csv
                for (int i = 0; i < records.Count(); i++)
                {
                    string[] date = records[i].dispensed_date.Split(' ');
                    dateList.Add(date[0]);

                }

                string[] cleanDate = RemoveDuplicates(dateList);


                //save the dispenselog to file.json following date
                for (int i = 0; i < cleanDate.Count(); i++)
                {
                    var exportRecord = new List<DispenseHistory>();
                    var exportRecordBI = new List<DispenseHistoryBI>();
                    for (int j = 0; j < records.Count(); j++)
                    {

                        if (records[j].dispensed_date.Contains(cleanDate[i]))
                        {
                            exportRecord.Add(records[j]);
                        }

                    }
                    if (exportRecord.Count > 0)
                    {
                        var export_path = $"{jsonDispenseLogPath}\\full_dispense_log_{cleanDate[i].Replace("/", "_")}.json";
                        Logger.Info("Export log path : " + export_path);
                        File.WriteAllText(export_path, JsonConvert.SerializeObject(exportRecord));
                    }
                }

                //Move complete extract file into achive folder
                reader.Close();
                CreateDirectoryIfNotExist(csv_history_achive_path);
                if (File.Exists($"{csv_history_achive_path}\\{csvFile.Name}"))
                {
                    File.Delete($"{csv_history_achive_path}\\{csvFile.Name}");
                }
                File.Move(csvFile.FullName, $"{csv_history_achive_path}\\{csvFile.Name}");
            }


            //N find json files in json dir, Does the files is exist ?
            DirectoryInfo jsonDispensePathInfo = new DirectoryInfo(jsonDispenseLogPath);
            foreach (var jsonFile in jsonDispensePathInfo.GetFiles("*.json"))
            {
                try
                {
                    //Y Did the files name ended with _p2 ? (Does it have file name not end with p2 ?)
                    //if (!jsonFile.Name.Contains("_p2"))
                    //{
                    bool isDispenseDone = false, isDispenseBIDone = false;
                    bool test_p2 = false;
                    int retry = 1, retry_bi = 1;
                    if(test_p2) { retry = 4; }
                    while (retry <= 3 && isDispenseDone == false)
                    {
                        //send to dispense history api
                        var result = await APIHelper.UploadFile(client, "dispense_history", jsonFile.FullName);
                        //string[] mvFile = Directory.GetFiles(jsonFile.FullName);
                        APIHelperResponse response = JsonConvert.DeserializeObject<APIHelperResponse>(result);
                        if(response.statusCode != 201)
                        {
                            retry++;
                            Logger.Error($"Error when upload file retring {retry} {jsonFile.FullName}");
                            Logger.Error($"Service response {response.statusCode} {response.message}");
                        }
                        isDispenseDone = true;
                    }
                    if (retry > 3)
                    {
                        Logger.Error($"Exception on send to api {jsonFile.FullName}");
                        CreateDirectoryIfNotExist($"{jsonDispenseLogPath}\\tmp");
                        string moveTo = $"{jsonDispenseLogPath}\\tmp\\{jsonFile.Name}.json";
                        File.Move(jsonFile.FullName, moveTo);
                        Logger.Info("Transfer to server error move json files to : " + moveTo);
                    }
                    var export_bi_file = $"{jsonDispenseLogPath}\\full_dispense_log_{jsonFile.Name}_bi.json";
                    while (retry_bi <= 3 && retry <=3 && isDispenseBIDone == false)
                    {
                        //Convert successful json file to json bi format
                        dynamic test = convertToBIDataNew(jsonFile.FullName);
                        //var export_path = $"{jsonDispenseLogPath}\\full_dispense_log_{cleanDate[i].Replace("/", "_")}.json";
                        
                        File.WriteAllText(export_bi_file, JsonConvert.SerializeObject(test));
                        //send to dispense history bi api
                        var result = await APIHelper.UploadFile(client, "dispense_history_bi", export_bi_file);
                        APIHelperResponse response = JsonConvert.DeserializeObject<APIHelperResponse>(result);
                        if (response.statusCode != 201)
                        {
                            retry_bi++;
                            Logger.Error($"Error when upload file retring {retry} {jsonFile.FullName}");
                            Logger.Error($"Service response {response.statusCode} {response.message}");
                        }
                        isDispenseBIDone = true;
                    }
                    if (retry_bi > 3)
                    {
                        Logger.Error($"Exception on send to api {jsonFile.FullName}");
                        //change file name to xxx_p2 after unsucessful transfer
                        int extensionIndex = jsonFile.Name.IndexOf(".json");
                        //create tmp directory
                        CreateDirectoryIfNotExist($"{jsonDispenseLogPath}\\tmp");
                        string moveTo = $"{jsonDispenseLogPath}\\tmp\\{jsonFile.Name.Substring(0, extensionIndex)}_p2.json";
                        File.Move(export_bi_file, moveTo);
                        Logger.Info("Transfer to server error move json files to : " + moveTo);
                    }

                    if(retry < 4 && retry_bi < 4) //Fix this flow when normal file is not done we should not change name to _p2
                    {
                        //sucessful transfer remove this json file
                        File.Delete(jsonFile.FullName);
                        Logger.Info($"Transfer to server complete delete json files name {jsonFile.FullName}");

                    }

                    

                    //if(retry_bi > 4)
                    //{
                    //    //change file name to xxx_p2 after unsucessful transfer
                    //    int extensionIndex = jsonFile.Name.IndexOf(".json");
                    //    //create tmp directory
                    //    CreateDirectoryIfNotExist($"{jsonDispenseLogPath}\\tmp");
                    //    string moveTo = $"{jsonDispenseLogPath}\\tmp\\{jsonFile.Name.Substring(0, extensionIndex)}_p2.json";
                    //    File.Move(jsonFile.FullName, moveTo);
                    //    Logger.Info("Transfer to server error move json files to : " + moveTo);
                        
                    //}
                //}


                    
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Exception on create json _p2 : " + ex.ToString());
                }

            }
            //Move all convert "_p2" files back to jsonDispenseLogPath from tmp directory
            CreateDirectoryIfNotExist($"{jsonDispenseLogPath}\\tmp");
            DirectoryInfo jsonDispensetmpPathInfo = new DirectoryInfo($"{jsonDispenseLogPath}\\tmp");
            foreach (var tmpjsonFile in jsonDispensetmpPathInfo.GetFiles("*.json"))
            {
                string moveTo = $"{jsonDispenseLogPath}\\{tmpjsonFile.Name}";
                File.Move(tmpjsonFile.FullName, moveTo);
            }
        }

        private DispenseHistoryBI convertToBIData(string clean_date, string auto_tint_id, DispenseHistory dispenseHistory)
        {
            DispenseHistoryBI data = new DispenseHistoryBI();
            string recordKeyDate = clean_date.Split('_')[2] + clean_date.Split('_')[1] + ((clean_date.Split('_')[0].Length < 2) ? "0" : "") + clean_date.Split('_')[0];
            data.record_key = "ID" + recordKeyDate + auto_tint_id;
            return data;
        }

        private List<DispenseHistoryBI> convertToBIDataNew(string json_history_path)
        {
            string file_path = json_history_path;//@"E:\Tutorial\json_dispense_log\full_dispense_log_21_10_2015_p2_test.json";
            string streamFile = File.ReadAllText(file_path);
            dynamic details = JArray.Parse(streamFile);
            //dynamic stuff = JsonConvert.DeserializeObject<ListDispenseHistory>(details);
            var exportRecordBI = new List<DispenseHistoryBI>();


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
                export_bi.color_name = detail["colour_name"];
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
                export_bi.sale_out_quantity_ea = "";
                export_bi.sale_out_amt_thb = sale_out_amt_thb.ToString();
                export_bi.branch_code = "";
                export_bi.branch_name = "";
                export_bi.type = saleType;
                export_bi.status_shade = status_shade;
                exportRecordBI.Append(export_bi);
            }
            return exportRecordBI;
        }

        private static string[] RemoveDuplicates(List<string> dateList)
        {
            HashSet<string> set = new HashSet<string>(dateList);
            string[] result = new string[set.Count];
            set.CopyTo(result);
            return result;
        }

        private void CreateDirectoryIfNotExist(string filepath)
        {
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
        }
    }
}
