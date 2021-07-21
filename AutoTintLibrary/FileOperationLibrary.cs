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
using System.Net;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace AutoTintLibrary
{
    public class FileOperationLibrary
    {
        private dynamic client = APIHelper.init();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
            HeaderValidated = null,
            IgnoreBlankLines = true,
            PrepareHeaderForMatch = args => args.Header.Replace(" ", "")

        };
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        public async Task<string> StartOperation()
        {
            //Init configuration variable
            int statusCode = 200;
            string responseMessage = "Succesful Tranfer";
            string csv_history_path = ManageConfig.ReadGlobalConfig("csv_history_path");
            string jsonDispenseLogPath = ManageConfig.ReadGlobalConfig("json_dispense_log_path");
            string csv_history_achive_path = ManageConfig.ReadGlobalConfig("csv_history_achive_path");
            string auto_tint_id = ManageConfig.ReadGlobalConfig("auto_tint_id");
            string programdata_path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            CreateDirectoryIfNotExist($"{jsonDispenseLogPath}");
            //Create isRunning file
            CreateDirectoryIfNotExist($"{programdata_path}\\tmp");
            File.Create($"{programdata_path}\\tmp\\running.tmp").Dispose();
            //find the csv in history files
            DirectoryInfo csvHistoryPathInfo = new DirectoryInfo(csv_history_path);
            foreach (var csvFile in csvHistoryPathInfo.GetFiles("*.csv"))
            {
                var reader = new StreamReader(csvFile.FullName);
                var csv = new CsvReader(reader, csvConfig);
                //csv.Configuration.PrepareHeaderForMatch = (string header) => header.Replace(" ", "");
                var records = csv.GetRecords<DispenseHistory>().ToList();
                //Clean the records that dispense_formular_id is empty or null
                records.RemoveAll(x => string.IsNullOrWhiteSpace(x.dispensed_formula_id));



                var dateList = new List<string>();
                //does csv file exist?
                //Extract the csv to json following DISPENSED_DATE
                //(New requirement) 27/06/2021 : extract if the dispensed date from Response of API /dispense_history/last_updated/ is earlier than the date in file.
                string latest_dispense_date = await APIHelper.RequestGet(client, $"/dispense_history/last_updated/?auto_tint_id={auto_tint_id}", auto_tint_id);
                
                //string latest_dispense_date = await APIHelper.RequestGet(client, $"/dispense_history/last_updated/?auto_tint_id=11016469AT01");
                APIHelperResponse latest_dispense_date_response = JsonConvert.DeserializeObject<APIHelperResponse>(latest_dispense_date);
                //Get all date in csv 
                for (int i = 0; i < records.Count(); i++)
                {
                    string[] date = records[i].dispensed_date.Split(' ');
                    //date[0] = "21/10/2015"
                    //date[1] = "15:30:16"
                    //TODO: if lastest_dispense_date is 404 and lastest_dispense_data.dispensed_date > date we will not add the date to dateList
                    //DateTime econvertedDate = Convert.ToDateTime(latest_dispense_date);
                    //DateTime econvertedDate = Convert.ToDateTime();
                    //Convert Year to C.E. if input is B.E. if compare with today and the year is more than today
                    int year = int.Parse(date[0].Split('/')[2]);
                    int now = DateTime.Today.Year;
                    if (year > now)
                    {
                        date[0] = $"{date[0].Split('/')[1]}/{date[0].Split('/')[0]}/{year - 543}";
                        records[i].dispensed_date = $"{date[0]} {date[1]}";
                    }

                    bool shouldConvert = false;
                    if (latest_dispense_date_response.statusCode != 404)
                    {
                        //convert latest_dispense_date_response.message then get the latest_dispense_date.dispensed_date
                        DispenseHistory dispenseH = JsonConvert.DeserializeObject<DispenseHistory>(latest_dispense_date_response.message);

                        string[] dd = dispenseH.dispensed_date.Split(' ');
                        //date[0] = "2015-10-21"
                        //string dpdate = dd[0].Split('-')[1]+"/"+dd[0].Split('-')[2]+"/"+dd[0].Split('-')[0];
                        string dpdate = $"{dd[0].Split('-')[1]}/{dd[0].Split('-')[2]}/{dd[0].Split('-')[0]}";
                        DateTime econvertedDate = DateTime.Parse(dpdate);

                        string spdate = $"{date[0].Split('/')[1]}/{date[0].Split('/')[0]}/{date[0].Split('/')[2]}";
                        DateTime sconvertedDate = DateTime.Parse(spdate);

                        int result = DateTime.Compare(econvertedDate, sconvertedDate);
                        //if (result < 0)
                        //    relationship = "is earlier than";
                        //else if (result == 0)
                        //    relationship = "is the same time as";
                        //else
                        //    relationship = "is later than";
                        //Console.WriteLine("{0} {1} {2}", econvertedDate, relationship, sconvertedDate);
                        if (result <= 0)
                        {
                            shouldConvert = true;
                        }
                        else
                        {
                            Logger.Info($"Lastest dispense date {econvertedDate} for {auto_tint_id}, it's later than {sconvertedDate} system will not convert and transfer data.");
                        }
                    }



                    latest_dispense_date_response.statusCode = 404;
                    if ((latest_dispense_date_response.statusCode != 404)&&(shouldConvert))
                    {
                        dateList.Add(date[0]);
                    }else if(latest_dispense_date_response.statusCode == 404)
                    {
                        dateList.Add(date[0]);
                    }
                    
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
                        File.WriteAllText(export_path, JsonConvert.SerializeObject(exportRecord),Encoding.UTF8);
                    }
                    else
                    {
                        Logger.Info($"Doesn't have match dispense date in csv with cleanDate ({cleanDate[i].Replace("/", "_")})");
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
            //Count .json file for calculate percentage.
            int jsonFileTotalCounter = jsonDispensePathInfo.GetFiles("*.json").Length;
            string file_total_log_path = $"{programdata_path}\\tmp\\lib_running_log.json";
            var data = new ProgressCounter() { total_file= jsonFileTotalCounter, complete_counter=0,status="Transfer History" };
            File.WriteAllText(file_total_log_path, JsonConvert.SerializeObject(data), Encoding.UTF8);
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
                        var result = await APIHelper.UploadFile(client, "dispense_history", jsonFile.FullName,auto_tint_id);
                        //string result = "{ statusCode : 201, message : \"\" }";
                        APIHelperResponse response = JsonConvert.DeserializeObject<APIHelperResponse>(result);
                        if (response.statusCode != 201)
                        {
                            retry++;
                            Logger.Error($"Error when upload file retring round {retry} filename {jsonFile.FullName}");
                            Logger.Error($"Service response statusCode : {response.statusCode}");
                            Logger.Error($"Service response message :  {response.message}");
                        }
                        if (response.statusCode == 201)
                        {
                            isDispenseDone = true;
                            Logger.Info($"Transfer to API dispense_history success filename : {jsonFile.Name}");
                        }
                    }
                    if (retry > 3)
                    {
                        Logger.Error($"Exception on send to api {jsonFile.FullName}");
                        CreateDirectoryIfNotExist($"{jsonDispenseLogPath}\\tmp");
                        string moveTo = $"{jsonDispenseLogPath}\\tmp\\{jsonFile.Name}.json";
                        File.Move(jsonFile.FullName, moveTo);
                        Logger.Info("Transfer to server error move json files to : " + moveTo);
                        statusCode = 500;
                        responseMessage = "Error dispense_history can't transfer";
                    }
                    int extensionIndex = jsonFile.Name.IndexOf(".json");
                    var export_bi_file = $"{jsonDispenseLogPath}\\{jsonFile.Name.Substring(0, extensionIndex)}_bi.json";
                    while (retry_bi <= 3 && retry <=3 && isDispenseBIDone == false)
                    {
                        //Convert successful json file to json bi format
                        try { 
                            dynamic test = convertToBIDataNew(jsonFile.FullName);
                            File.WriteAllText(export_bi_file, JsonConvert.SerializeObject(test),Encoding.UTF8);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception " + ex.ToString());
                            Logger.Error("Exception on create json _bi : " + ex.ToString());
                        }
                        //var export_path = $"{jsonDispenseLogPath}\\full_dispense_log_{cleanDate[i].Replace("/", "_")}.json";


                        //send to dispense history bi api
                        var result = await APIHelper.UploadFile(client, "dispense_history_bi", export_bi_file, auto_tint_id);
                        //string result = "{ statusCode : 201, message : \"\" }";
                        APIHelperResponse response = JsonConvert.DeserializeObject<APIHelperResponse>(result);
                        if (response.statusCode != 201)
                        {
                            retry_bi++;
                            Logger.Error($"Error when upload file retring round {retry_bi} filename {export_bi_file}");
                            Logger.Error($"Service response statusCode : {response.statusCode}");
                            Logger.Error($"Service response message :  {response.message}");
                        }
                        if(response.statusCode == 201)
                        {
                            isDispenseBIDone = true;
                            Logger.Info($"Transfer to API dispense_history_bi success filename : {jsonFile.Name.Substring(0, extensionIndex)}_bi.json");
                        }
                    }
                    if (retry_bi > 3)
                    {
                        Logger.Error($"Exception on send to api {jsonFile.FullName}");
                        //change file name to xxx_p2 after unsucessful transfer
                        
                        //create tmp directory
                        CreateDirectoryIfNotExist($"{jsonDispenseLogPath}\\tmp");
                        string moveTo = $"{jsonDispenseLogPath}\\tmp\\{jsonFile.Name}";
                        if (!jsonFile.Name.Contains("_p2"))
                        {
                            moveTo = $"{jsonDispenseLogPath}\\tmp\\{jsonFile.Name.Substring(0, extensionIndex)}_p2.json";
                        }
                        
                        File.Move(export_bi_file, moveTo);
                        Logger.Info("Transfer to server error move json files to : " + moveTo);
                        if (statusCode == 500) responseMessage += "\nError dispense_history_bi can't transfer";
                        responseMessage = "Error dispense_history can't transfer";
                    }

                    if(retry < 4 && retry_bi < 4) //Fix this flow when normal file is not done we should not change name to _p2
                    {
                        //sucessful transfer remove this json file
                        File.Delete(jsonFile.FullName);
                        File.Delete(export_bi_file);
                        Logger.Info($"Transfer to server complete delete json files name {jsonFile.FullName}");

                        int jsonFileCounter = jsonDispensePathInfo.GetFiles("*.json").Length;
                        //string file_total_log_path = $"{programdata_path}\\tmp\\lib_running_log.json";
                        int completePercentage = (int)((jsonFileTotalCounter - jsonFileCounter) * 100) / jsonFileTotalCounter;
                        Logger.Info($"Log the percentage to file {completePercentage}");
                        data = new ProgressCounter() { total_file = jsonFileCounter, complete_counter = completePercentage, status = "Transfering ...." };
                        File.WriteAllText(file_total_log_path, JsonConvert.SerializeObject(data), Encoding.UTF8);

                    }       
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Main transfer process : " + ex.ToString());
                    if (statusCode == 500) responseMessage += "\nException Main transfer process";
                    responseMessage = "Exception on Main transfer process";
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
            //Delete is running file
            File.Delete($"{programdata_path}\\tmp\\running.tmp");
            //File.Delete($"{programdata_path}\\tmp\\lib_running_log.json");
            var jsonData = new ProgressCounter() { total_file = 0, complete_counter = 0, status = "Stand by" };
            File.WriteAllText(file_total_log_path, JsonConvert.SerializeObject(jsonData), Encoding.UTF8);
            return JsonConvert.SerializeObject(new { statusCode = statusCode, message = responseMessage });
        }

        private DispenseHistoryBI convertToBIData(string clean_date, string auto_tint_id, DispenseHistory dispenseHistory)
        {
            DispenseHistoryBI data = new DispenseHistoryBI();
            string recordKeyDate = clean_date.Split('_')[2] + clean_date.Split('_')[1] + ((clean_date.Split('_')[0].Length < 2) ? "0" : "") + clean_date.Split('_')[0];
            data.record_key = "ID" + recordKeyDate + auto_tint_id;
            return data;
        }

        public List<DispenseHistoryBI> convertToBIDataNew(string json_history_path)
        {
            string file_path = json_history_path;//@"E:\Tutorial\json_dispense_log\full_dispense_log_21_10_2015_p2_test.json";
            string streamFile = File.ReadAllText(file_path);
            dynamic details = JArray.Parse(streamFile);
            //dynamic stuff = JsonConvert.DeserializeObject<ListDispenseHistory>(details);
            var exportRecordBI = new List<DispenseHistoryBI>();


            foreach (dynamic detail in details)
            {
                //Console.WriteLine(detail);
                //Do a formatted date
                String[] dispense_date = detail["dispensed_date"].ToString().Split(' ');
                string formattedDate = "";
                var export_bi = new DispenseHistoryBI();
                try
                {
                    //Append the day and month to 2 length string
                    if (dispense_date[0].Split('/')[0].Length < 2) dispense_date[0] = $"0{dispense_date[0].Split('/')[0]}/{dispense_date[0].Split('/')[1]}/{dispense_date[0].Split('/')[2]}";
                    if (dispense_date[0].Split('/')[1].Length < 2) dispense_date[0] = $"{dispense_date[0].Split('/')[0]}/0{dispense_date[0].Split('/')[1]}/{dispense_date[0].Split('/')[2]}";
                    
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
                        Logger.Error($"BI Parsing dispense_date fail dispense_date data : {dispense_date[0]}");
                    }
                }
                catch(Exception ex)
                {
                    Logger.Error($"BI Parsing dispense_date fail dispense_date data : {dispense_date[0]}");
                    throw ex;
                }



                //Do a dispenser_no,customer_key
                try { 
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
                    export_bi.dispenser_no = dispenser_no;
                    export_bi.customer_key = customer_key;
                }
                catch(Exception ex)
                {
                    Logger.Error($"BI dispenser_no,customer_key due to {detail["company_code"]}");
                    throw ex;
                }

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
                try
                {
                    double amount = (double)detail["wanted_amount"];
                    string libUnits = detail["unit_name"].ToString().ToLower(); //Quart => UsQuart , GL,Gallon => UsGallon , Liter => Liter , ml => Milliliter
                    if (libUnits.IndexOf("quart") >= 0) libUnits = "UsQuart";
                    if (libUnits.IndexOf("gallon") >= 0) libUnits = "UsGallon";
                    if (libUnits.IndexOf("gl") >= 0) libUnits = "UsGallon";
                    if (libUnits.IndexOf("liter") >= 0) libUnits = "Liter";
                    if (libUnits.IndexOf("kg") >= 0) libUnits = "Liter";
                    if (libUnits.IndexOf("ml") >= 0) libUnits = "Milliliter";
                    string sales_out_qty_gl = UnitConverter.ConvertByName(amount, "Volume", libUnits, "UsGallon").ToString();
                    string sales_out_qty_l = UnitConverter.ConvertByName(amount, "Volume", libUnits, "Liter").ToString();
                    export_bi.sale_out_quantity_gl = sales_out_qty_gl;
                    export_bi.sale_out_quantity_l = sales_out_qty_l;
                }
                catch(Exception ex){
                    Logger.Error($"BI Convert wanted_amount by libUnits exception for {detail["wanted_amount"]} {detail["unit_name"]}");
                    throw ex;
                }


                //sale_out_amt_thb
                try
                {
                double base_price = (double)detail["base_price"];
                double colorant_price = (double)detail["colorant_price"];
                double sale_out_amt_thb = Math.Ceiling((base_price + (base_price * 7 / 100)) + (colorant_price + (colorant_price * 7 / 100)));
                export_bi.sale_out_amt_thb = sale_out_amt_thb.ToString();
                }
                catch (Exception ex)
                {
                    Logger.Error($"BI Convert sale_out_amt_thb exception from {detail["base_price"]} {detail["colorant_price"]}");
                    throw ex;
                }

                //getSaleType
                string saleType = "";
                if ((double)detail["base_price"] > 0)
                {
                    saleType = "Base and Colorant";
                }
                else
                {
                    saleType = "Colorant Only";
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

                export_bi.dispensed_date = detail["dispensed_date"]; //convert to CE.
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
                export_bi.base_value = ((export_bi.base_name.Length > 0)&&(export_bi.base_name != " "))? detail["base_name"].ToString().Substring(detail["base_name"].ToString().Length - (detail["base_name"].ToString().IndexOf("Base") + 3)):"";
                export_bi.price = detail["price"];
                export_bi.base_price = detail["base_price"];
                export_bi.colorant_price = detail["colorant_price"];
                export_bi.company_name = detail["company_name"];
                export_bi.company_code = detail["company_code"];

                export_bi.material_pf_code = detail["material_pf_code"];
                export_bi.component_name = "";

                export_bi.sale_out_quantity_ea = "1";

                export_bi.branch_code = "";
                export_bi.branch_name = "";
                export_bi.type = saleType;
                export_bi.status_shade = status_shade;
                exportRecordBI.Add(export_bi);
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
        dynamic JsonSetting = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };
        public async Task UpdateAutotintVersion()
        {
            string programdata_path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            CreateDirectoryIfNotExist($"{programdata_path}\\tmp");
            File.Create($"{programdata_path}\\tmp\\dbupdate_running.tmp").Dispose();
            string auto_tint_id = ManageConfig.ReadGlobalConfig("auto_tint_id");
            string database_path = ManageConfig.ReadGlobalConfig("database_path");
            string str_response = await APIHelper.GetAutoTintVersion(client, auto_tint_id);

            APIHelperResponse response = JsonConvert.DeserializeObject<APIHelperResponse>(str_response);
            string file_total_log_path = $"{programdata_path}\\tmp\\lib_running_log.json";
            var jsonData = new ProgressCounter() { total_file = 1, complete_counter = 0, status = "Download DB File" };
            File.WriteAllText(file_total_log_path, JsonConvert.SerializeObject(jsonData), Encoding.UTF8);
            if (response.statusCode == 200)
            {
                AutoTintWithId result = JsonConvert.DeserializeObject<AutoTintWithId>(response.message, JsonSetting);
                //lblDatabaseVersionText.Text = "" + result.pos_setting_version;
                DateTime startTimeFormate = DateTime.UtcNow;
                TimeZoneInfo systemTimeZone = TimeZoneInfo.Local;
                DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(startTimeFormate, systemTimeZone);
                //string ICTDateTimeText = localDateTime.ToString("dddd dd MMMM yyyy HH:mm:ff", new System.Globalization.CultureInfo("th-TH"));
                string ICTDateTimeText = localDateTime.ToString("dddd dd MMMM yyyy HH:mm:ff", new System.Globalization.CultureInfo("en-GB"));

                PrismaProLatestVersion checkVersion = new PrismaProLatestVersion();
                //Check the server for newer version.
                checkVersion = await APIHelper.GetDBLatestVersion(client, result.pos_setting.id,auto_tint_id);


                Logger.Info($"Successful on get Autotint Version Status Code : {response.statusCode}  Message : {response.message}");
                var shouldDownloadNewDB = (result.pos_setting_version == null) ? true : (result.pos_setting_version.id < checkVersion.id);
                //if (shouldDownloadNewDB)
                if (true)
                {
                    string downloadURI = $"{checkVersion.file}";
                    string path = ManageConfig.ReadGlobalConfig("programdata_log_path");
                    string tmp_path = $"{path}\\tmp";
                    if (!Directory.Exists(tmp_path))
                    {
                        Directory.CreateDirectory(tmp_path);
                    }
                    //if (!APIHelper.APIConnectionCheck(3, 30)) throw new Exception("Internet Connection Error");
                    String[] URIArray = downloadURI.Split('/');
                    WebClient webClient = new WebClient();
                    webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCompleted);
                    webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                    webClient.QueryString.Add("fileName", $"{URIArray[URIArray.Length - 1]}");
                    webClient.DownloadFileAsync(new Uri(downloadURI), $"{tmp_path}\\{URIArray[URIArray.Length - 1]}");
                    //Update to API about new version of database
                    string data = @"
                    {
                    ""pos_setting_version_id"": " + checkVersion.id + @"
                    }
                    ";
                    //dynamic prima_pro_version_response = await APIHelper.RequestPut(client, $"/auto_tint/{auto_tint_id}/pos_update", data, auto_tint_id);
                    DBversionHandler jsonData2 = new DBversionHandler() { version = $"{checkVersion.number}", datetime = $"{ICTDateTimeText}", filename = $"{URIArray[URIArray.Length - 1]}", auto_tint_id = $"{auto_tint_id}" };
                    File.WriteAllText($"{programdata_path}\\db_version.json", JsonConvert.SerializeObject(jsonData2), Encoding.UTF8);
                }
                else
                {
                    //Delete is running file
                    File.Delete($"{programdata_path}\\tmp\\dbupdate_running.tmp");
                    //File.Delete($"{programdata_path}\\tmp\\lib_running_log.json");
                    var jsonDataElse = new ProgressCounter() { total_file = 0, complete_counter = 0, status = "Stand by" };
                    File.WriteAllText($"{programdata_path}\\tmp\\lib_running_log.json", JsonConvert.SerializeObject(jsonDataElse), Encoding.UTF8);
                    AutoTintWithId result2 = JsonConvert.DeserializeObject<AutoTintWithId>(response.message, JsonSetting);
                    string[] fileName = result2.pos_setting_version.file.Split('/');
                    DBversionHandler jsonData3 = new DBversionHandler() { version = $"{result2.pos_setting_version.number}", datetime = $"{ICTDateTimeText}", filename = $"{fileName[fileName.Length - 1]}", auto_tint_id = $"{auto_tint_id}" };
                    File.WriteAllText($"{programdata_path}\\db_version.json", JsonConvert.SerializeObject(jsonData3), Encoding.UTF8);
                    //DirectoryInfo dInfo = new DirectoryInfo($"{programdata_path}\\tmp\\checkdbVersion.tmp");
                    //DirectorySecurity dSecurity = dInfo.GetAccessControl();
                    //dSecurity.AddAccessRule(new FileSystemAccessRule("everyone", FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.InheritOnly, AccessControlType.Allow));
                    //dInfo.SetAccessControl(dSecurity);
                }
            }
            else
            {
                //MessageBoxResult AlertMessageBox = System.Windows.MessageBox.Show($"Status Code : {response.statusCode} \nMessage : {response.message}", "Error", MessageBoxButton.OK);
                Logger.Error($"Exception on get Autotint Version Status Code : {response.statusCode}  Message : {response.message}");
            }
            
        }

        private void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //Logger.Error($"Exception on get Autotint Version Status Code : {response.statusCode}  Message : {response.message}");
            //temp folder
            string path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            string tmp_path = $"{path}\\tmp";
            //Move file to database path
            string downLoadFileName = ((System.Net.WebClient)(sender)).QueryString["fileName"];
            string database_path = ManageConfig.ReadGlobalConfig("database_path");
            if (File.Exists($"{database_path}\\{downLoadFileName}"))
            {
                File.Delete($"{database_path}\\{downLoadFileName}");
            }
            File.Move($"{tmp_path}\\{downLoadFileName}", $"{database_path}\\{downLoadFileName}");
            Logger.Info($"Download new update succesful");
            string programdata_path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            File.Delete($"{programdata_path}\\tmp\\dbupdate_running.tmp");
            //File.Delete($"{programdata_path}\\tmp\\lib_running_log.json");
            var jsonDataComp = new ProgressCounter() { total_file = 0, complete_counter = 0, status = "Stand by" };
            File.WriteAllText($"{programdata_path}\\tmp\\lib_running_log.json", JsonConvert.SerializeObject(jsonDataComp), Encoding.UTF8);
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            try
            {
                string programdata_path = ManageConfig.ReadGlobalConfig("programdata_log_path");
                string file_total_log_path = $"{programdata_path}\\tmp\\lib_running_log.json";
                if (e.ProgressPercentage % 5 == 0)
                {
                    Logger.Info($"ProgressChanged  : {e.ProgressPercentage}");
                    var jsonData = new ProgressCounter() { total_file = 1, complete_counter = e.ProgressPercentage, status = "Download DB File" };
                    File.WriteAllText(file_total_log_path, JsonConvert.SerializeObject(jsonData), Encoding.UTF8);
                }
            }catch(Exception ex)
            {
                Logger.Error($"Exception on progresschanged : {ex.Message}");

            }
            
        }
    }
}
