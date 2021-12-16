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
            string responseMessage = "Successful Tranfer";
            string csv_history_path = ManageConfig.ReadGlobalConfig("csv_history_path");
            string jsonDispenseLogPath = ManageConfig.ReadGlobalConfig("json_dispense_log_path");
            string csv_history_achive_path = ManageConfig.ReadGlobalConfig("csv_history_achive_path");
            string auto_tint_id = ManageConfig.ReadGlobalConfig("auto_tint_id");
            string programdata_path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            string file_total_log_path = $"{programdata_path}\\tmp\\lib_running_log.json";
            var all_auto_tint_id_list = new List<string>();
            try
            {
                CreateDirectoryIfNotExist($"{jsonDispenseLogPath}");
                //Create isRunning file
                CreateDirectoryIfNotExist($"{programdata_path}\\tmp");
                File.Create($"{programdata_path}\\tmp\\running.tmp").Dispose();
               
                //find the csv in history files
                DirectoryInfo csvHistoryPathInfo = new DirectoryInfo(csv_history_path);
                //Prepare the base database for convert BI data.
                if (csvHistoryPathInfo.GetFiles("*.csv").Length > 0)
                {
                    var baseDBDLresult = await downloadBaseDB();
                    if (!baseDBDLresult) Logger.Error("downloadBaseDB error"); 
                    var baseMMResult = await downloadMaterialMapper();
                    if (!baseMMResult) Logger.Error("downloadMaterialMapper error");
                }
                
                foreach (var csvFile in csvHistoryPathInfo.GetFiles("*.csv"))
                {
                        Encoding file_encoding = TextFileEncodingDetector.DetectTextFileEncoding(csvFile.FullName);
                        //If encoding is utf-8 must read by windows-874, if null must read by utf-8
                        Encoding read_input_as = (file_encoding == Encoding.GetEncoding("utf-8")) ? Encoding.GetEncoding("windows-874") : Encoding.GetEncoding("utf-8");
                        var reader = new StreamReader(csvFile.FullName, read_input_as);
                        var csv = new CsvReader(reader, csvConfig);
                        try
                        {
                            //csv.Configuration.PrepareHeaderForMatch = (string header) => header.Replace(" ", "");
                            var records = csv.GetRecords<DispenseHistory>().ToList();
                            //Clean the records that dispense_formular_id is empty or null
                            records.RemoveAll(x => string.IsNullOrWhiteSpace(x.dispensed_formula_id));

                            if(records.Count == 0)
                            {
                                reader.Close();
                                Logger.Error($"File format is not correct {csvFile.FullName}");
                                Logger.Error("Move file to ignore location");
                                CreateDirectoryIfNotExist($"{csv_history_path}\\ignore_files");
                                if (File.Exists($"{csv_history_path}\\ignore_files\\{csvFile.Name}"))
                                {
                                    File.Delete($"{csv_history_path}\\ignore_files\\{csvFile.Name}");
                                }
                                File.Move(csvFile.FullName, $"{csv_history_path}\\ignore_files\\{csvFile.Name}");
                                continue;
                            }
                            foreach (DispenseHistory record in records)
                            {
                                var properties = record.GetType().GetProperties();
                                foreach (var prop in properties)
                                {
                                    if (prop.Name.Equals("Item"))
                                    {
                                        continue;
                                    }
                                    if (record[prop.Name] == null || prop.Name.Equals("material_pf_code")) continue;
                                    if (record[prop.Name].Equals("NA") || record[prop.Name].Equals("\"NA\""))
                                    {
                                        record[prop.Name] = "";
                                    }
                                }
                            }
                            var dateList = new List<string>();
                            var auto_tint_id_list = new List<string>();
                            //does csv file exist?
                            //Extract the csv to json following DISPENSED_DATE
                            //(New requirement) 27/06/2021 : extract if the dispensed date from Response of API /dispense_history/last_updated/ is earlier than the date in file.
                            string latest_dispense_date = await APIHelper.RequestGet(client, $"/dispense_history/last_updated/?auto_tint_id={auto_tint_id}", auto_tint_id);

                            //string latest_dispense_date = await APIHelper.RequestGet(client, $"/dispense_history/last_updated/?auto_tint_id=11016469AT01");
                            APIHelperResponse latest_dispense_date_response = JsonConvert.DeserializeObject<APIHelperResponse>(latest_dispense_date);
                            //ProgressCounter data = 
                            File.WriteAllText(file_total_log_path, JsonConvert.SerializeObject(new ProgressCounter() { total_file = 0, complete_counter = 0, status = "Converting ..." }), Encoding.UTF8);
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
                                string uk = DateTime.Now.Date.ToString("yyyy", new CultureInfo("en-GB"));
                                int now = Convert.ToInt32(uk);
                                if (year > now)
                                {
                                    date[0] = $"{date[0].Split('/')[0]}/{date[0].Split('/')[1]}/{year - 543}";
                                    records[i].dispensed_date = $"{date[0]} {date[1]}";
                                }

                                bool shouldConvert = false;
                                if (latest_dispense_date_response.statusCode != 404)
                                {
                                    //convert latest_dispense_date_response.message then get the latest_dispense_date.dispensed_date
                                    DispenseHistory dispenseH = JsonConvert.DeserializeObject<DispenseHistory>(latest_dispense_date_response.message);

                                    string[] dd = dispenseH.dispensed_date.Split(' ');
                                    if (dd[0].Split('-')[0].Length < 2) dd[0] = $"0{dd[0].Split('-')[0]}-{dd[0].Split('-')[1]}-{dd[0].Split('-')[2]}";
                                    if (dd[0].Split('-')[1].Length < 2) dd[0] = $"{dd[0].Split('-')[0]}-0{dd[0].Split('-')[1]}-{dd[0].Split('-')[2]}";
                                    string dpdate = $"{dd[0].Split('-')[1]}/{dd[0].Split('-')[2]}/{dd[0].Split('-')[0]}";
                                    //DateTime econvertedDate = DateTime.Parse(dpdate, CultureInfo.GetCultureInfo("en-GB"));
                                    DateTime econvertedDate = DateTime.ParseExact(dpdate, "MM/dd/yyyy", CultureInfo.GetCultureInfo("en-GB"));

                                    if (date[0].Split('/')[0].Length < 2) date[0] = $"0{date[0].Split('/')[0]}/{date[0].Split('/')[1]}/{date[0].Split('/')[2]}";
                                    if (date[0].Split('/')[1].Length < 2) date[0] = $"{date[0].Split('/')[0]}/0{date[0].Split('/')[1]}/{date[0].Split('/')[2]}";
                                    string spdate = $"{date[0].Split('/')[0]}/{date[0].Split('/')[1]}/{date[0].Split('/')[2]}";
                                    //DateTime sconvertedDate = DateTime.Parse(spdate);
                                    DateTime sconvertedDate = DateTime.ParseExact(spdate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"));

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



                                //latest_dispense_date_response.statusCode = 404;
                                if ((latest_dispense_date_response.statusCode != 404) && (shouldConvert))
                                {
                                    dateList.Add(date[0]);
                                }
                                else if (latest_dispense_date_response.statusCode == 404)
                                {
                                    dateList.Add(date[0]);
                                }
                                all_auto_tint_id_list.Add(records[i].company_code);

                            }

                            string[] cleanDate = RemoveDuplicates(dateList);
                            

                            //save the dispenselog to file.json following date
                            for (int i = 0; i < cleanDate.Count(); i++)
                            {
                                var exportRecord = new List<DispenseHistory>();
                                var exportRecordBI = new List<DispenseHistoryBI>();
                                for (int j = 0; j < records.Count(); j++)
                                {

                                    string[] date = records[j].dispensed_date.Split(' ');
                                    if (String.Equals(date[0], cleanDate[i]))
                                    {
                                        exportRecord.Add(records[j]);
                                    }

                                }
                                if (exportRecord.Count > 0)
                                {
                                    var export_path = $"{jsonDispenseLogPath}\\full_dispense_log_{cleanDate[i].Replace("/", "_")}.json";
                                    Logger.Info("Export log path : " + export_path);
                                    File.WriteAllText(export_path, JsonConvert.SerializeObject(exportRecord), Encoding.UTF8);
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
                        } catch (Exception ex)
                        {
                            reader.Close();
                            Logger.Error($"Exception in csv convert file {csvFile.FullName}");
                            Logger.Error($"Exception message {ex.Message}");
                            Logger.Error("Move file to ignore location");
                            CreateDirectoryIfNotExist($"{csv_history_path}\\ignore_files");
                            if (File.Exists($"{csv_history_path}\\ignore_files\\{csvFile.Name}"))
                            {
                                File.Delete($"{csv_history_path}\\ignore_files\\{csvFile.Name}");
                            }
                            File.Move(csvFile.FullName, $"{csv_history_path}\\ignore_files\\{csvFile.Name}");
                        }
            }
            }catch(Exception ex)
            {
                Logger.Error($"Exception in csv convert method {ex.Message}");
            }
            string[] cleanAutoTintId = RemoveDuplicates(all_auto_tint_id_list); //string[]
            if (!String.IsNullOrEmpty(auto_tint_id))
            {
                List<string> tmpList = new List<string>();
                foreach (string id in cleanAutoTintId)
                {
                    tmpList.Add(id);
                }
                tmpList.Add(auto_tint_id);
                cleanAutoTintId = tmpList.ToArray();
            }
            List<AutoTintWithIdV2> dispenser_data_list = new List<AutoTintWithIdV2>();
            foreach (string id in cleanAutoTintId)
            {
                //string debug_id = "99999999AT01";
                string id_for_api = "";
                if (!id.Contains("AT"))
                {
                    id_for_api = $"{id}AT01";
                }
                else
                {
                    id_for_api = id;
                }
                //if (!String.IsNullOrEmpty(auto_tint_id)) id_for_api = auto_tint_id;
                var dispense_data_result = await APIHelper.RequestGet(client, $"/auto_tint/{id_for_api}", auto_tint_id);
                //string result = "{ statusCode : 201, message : \"\" }";
                APIHelperResponse response = JsonConvert.DeserializeObject<APIHelperResponse>(dispense_data_result);
                if (response.statusCode != 200)
                {
                    //throw new Exception($"Dispenser data of {id} not found");
                }
                else
                {
                    AutoTintWithIdV2 dispensev2_dataList = JsonConvert.DeserializeObject<AutoTintWithIdV2>(response.message, JsonSetting);
                    dispenser_data_list.Add(dispensev2_dataList);
                }
            }
            //Move the ignore file back to csv directory (csv_history_path)
            //if (Directory.Exists($"{csv_history_path}\\ignore_files\\"))
            //{
            //    DirectoryInfo csvIgnorePathInfo = new DirectoryInfo($"{csv_history_path}\\ignore_files\\");

            //    foreach (var file in csvIgnorePathInfo.GetFiles())
            //    {
            //        File.Move(file.FullName, $"{csv_history_path}\\{file.Name}");
            //    }
            //}


            //N find json files in json dir, Does the files is exist ?
            DirectoryInfo jsonDispensePathInfo = new DirectoryInfo(jsonDispenseLogPath);
            //Count .json file for calculate percentage.
            int jsonFileTotalCounter = jsonDispensePathInfo.GetFiles("*.json").Length;
            
            var data = new ProgressCounter() { total_file= jsonFileTotalCounter, complete_counter=0,status="Transfer History" };
            File.WriteAllText(file_total_log_path, JsonConvert.SerializeObject(data), Encoding.UTF8);
            foreach (var jsonFile in jsonDispensePathInfo.GetFiles("*.json"))
            {
                try
                {

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
                            Logger.Error($"Error when upload file retiring round {retry} filename {jsonFile.FullName}");
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
                            dynamic test = convertToBIDataNew(jsonFile.FullName, dispenser_data_list);
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
                            Logger.Error($"Error when upload file retrying round {retry_bi} filename {export_bi_file}");
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
                        data = new ProgressCounter() { total_file = jsonFileCounter, complete_counter = (int)((jsonFileTotalCounter-jsonFileCounter) * 100)/jsonFileTotalCounter, status = "Transfering ...." };
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
            File.Delete($"{programdata_path}\\tmp\\lib_running_log.json");
            return JsonConvert.SerializeObject(new { statusCode = statusCode, message = responseMessage });
        }

        private DispenseHistoryBI convertToBIData(string clean_date, string auto_tint_id, DispenseHistory dispenseHistory)
        {
            DispenseHistoryBI data = new DispenseHistoryBI();
            string recordKeyDate = clean_date.Split('_')[2] + clean_date.Split('_')[1] + ((clean_date.Split('_')[0].Length < 2) ? "0" : "") + clean_date.Split('_')[0];
            data.record_key = "ID" + recordKeyDate + auto_tint_id;
            return data;
        }

        public List<DispenseHistoryBI> convertToBIDataNew(string json_history_path,List<AutoTintWithIdV2> dispenser_data_list = null)
        {
            string file_path = json_history_path;//@"E:\Tutorial\json_dispense_log\full_dispense_log_21_10_2015_p2_test.json";
            string streamFile = File.ReadAllText(file_path);
            dynamic details = JArray.Parse(streamFile);
            //dynamic stuff = JsonConvert.DeserializeObject<ListDispenseHistory>(details);
            var exportRecordBI = new List<DispenseHistoryBI>();
            string programdata_path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            string auto_tint_id = ManageConfig.ReadGlobalConfig("auto_tint_id");
            List<BaseData> baseData = new List<BaseData>();
            if (File.Exists($"{programdata_path}\\basedb.json"))
            {
                try
                {
                    string tmpBaseData = File.ReadAllText($"{programdata_path}\\basedb.json");
                    baseData = JsonConvert.DeserializeObject<List<BaseData>>(tmpBaseData);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Exception occurs when parse basedb.json  {ex.Message}");
                    throw ex;
                }
            }
            else
            {
                Logger.Error("basedb.json doesn't exist convertToBI data can't validate status shade");
            }

            JObject material_mapper = new JObject();
            if (File.Exists($"{programdata_path}\\material_mapper.json"))
            {
                try
                {
                    string tmpMaterialData = File.ReadAllText($"{programdata_path}\\material_mapper.json");
                    material_mapper = JObject.Parse(tmpMaterialData);

                }
                catch (Exception ex)
                {
                    Logger.Error($"Exception occurs when parse material_mapper.json  {ex.Message}");
                    throw ex;
                }
            }
            else
            {
                Logger.Error("basedb.json doesn't exist convertToBI data can't validate status shade");
            }

            foreach (dynamic detail in details)
            {
                if (!detail["base_name"].ToString().ToLower().Contains("base")) continue;
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
                    //Convert year from input as Buddish Era to Christian Era
                    int year = int.Parse(dispense_date[0].Split('/')[2]);
                    int now = DateTime.Today.Year;
                    if (year > now)
                    {
                        dispense_date[0] = $"{dispense_date[0].Split('/')[0]}/{dispense_date[0].Split('/')[1]}/{year - 543}";
                    }
                    DateTime parsedDateTime;
                    if (DateTime.TryParseExact(dispense_date[0], "dd/mm/yyyy",
                        CultureInfo.GetCultureInfo("en-US"),
                        DateTimeStyles.None,
                        out parsedDateTime))
                    {
                        formattedDate = parsedDateTime.ToString("yyyymmdd", CultureInfo.GetCultureInfo("en-GB"));
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



                //Do a dispenser_no,customer_key,com_code,sales_org
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

                    foreach (AutoTintWithIdV2 data in dispenser_data_list)
                    {
                        if (!String.IsNullOrEmpty(auto_tint_id)) detail["company_code"] = auto_tint_id;
                        if (data.auto_tint_id.Contains(detail["company_code"].ToString()))
                        {
                            export_bi.com_code = data.com_code;
                            export_bi.sales_org = data.sales_org;
                            break;
                        }
                        else
                        {
                            export_bi.com_code = null;
                            export_bi.sales_org = null;
                        }
                    }
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
                    if (detail[$"component_name{i}"] == null)
                    {
                        continue;
                    }
                    else
                    {
                        String numStr = i.ToString();
                        int indexOfCW = detail[$"component_name{i}"].ToString().IndexOf("CW");
                        if (indexOfCW >= 0)
                        {
                            //Read XX from CWXX_LV to store in object cwxx_lv
                            numStr = detail[$"component_name{i}"].ToString().Substring(indexOfCW + 2, 2);
                            export_bi[$"cw{numStr}_lv"] = detail[$"lines_wanted_amount{i}"].ToString();
                            Double total, lines_wanted_amount = 0;
                            Double.TryParse((export_bi["cw_total"] != null) ? export_bi["cw_total"].ToString() : "0", out total);
                            Double.TryParse((detail[$"lines_wanted_amount{i}"] != null) ? detail[$"lines_wanted_amount{i}"].ToString() : "0", out lines_wanted_amount);
                            total = total + lines_wanted_amount;
                            export_bi["cw_total"] = total.ToString();
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
                    if (libUnits.IndexOf("shots") >= 0)
                    {
                        double amount_to_lites = amount * 0.0443603;
                        string sales_out_qty_gl = UnitConverter.ConvertByName(amount_to_lites, "Volume", "Liter", "UsGallon").ToString();
                        string sales_out_qty_l = UnitConverter.ConvertByName(amount_to_lites, "Volume", "Liter", "Liter").ToString();
                        export_bi.sale_out_quantity_gl = sales_out_qty_gl;
                        export_bi.sale_out_quantity_l = sales_out_qty_l;
                    }
                    else
                    {
                        string sales_out_qty_gl = UnitConverter.ConvertByName(amount, "Volume", libUnits, "UsGallon").ToString();
                        string sales_out_qty_l = UnitConverter.ConvertByName(amount, "Volume", libUnits, "Liter").ToString();
                        export_bi.sale_out_quantity_gl = sales_out_qty_gl;
                        export_bi.sale_out_quantity_l = sales_out_qty_l;
                    }
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
                 * 
                Complete : LINES_WANTED_AMOUNT กับ LINES_DISPENSED_AMOUNT ของทุก component ต้องเท่ากัน ยกเว้นแม่สี ที่จะเป็น 0 เสมอ
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
                    bool matchBaseComponentCondition = false;
                    for (int j = 0;j< baseData.Count; j++)
                    {
                        //var kkkk = detail[$"component_name{i}"].ToString().ToLower();
                        //var eeee = baseData[j].base_name.ToString().ToLower();
                        var vavva = detail[$"lines_dispensed_amount{i}"];
                        if ((detail[$"component_name{i}"].ToString().ToLower() == baseData[j].base_name.ToString().ToLower())
                            &&(detail[$"lines_dispensed_amount{i}"] == 0))
                        {
                            matchBaseComponentCondition = true;
                        }
                        if (matchBaseComponentCondition) break;
                    }
                    if (matchBaseComponentCondition) continue;
                    if (detail[$"lines_wanted_amount{i}"] == null || detail[$"lines_dispensed_amount{i}"] == null)
                    {
                        continue;
                    }
                    if (detail[$"lines_wanted_amount{i}"].ToString().Equals("") && detail[$"lines_dispensed_amount{i}"].ToString().Equals(""))
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

                //material_pf_code
                Boolean isDataMetSearchCriteria = false;
                if (detail["material_pf_code"] == "NA" || detail["material_pf_code"] == "\"NA\"")
                {
                    export_bi.material_pf_code = "";
                    isDataMetSearchCriteria = true;
                }
                else
                {
                    foreach (KeyValuePair<string, JToken> keyValuePair in material_mapper)
                    {
                        if (detail["material_pf_code"] == keyValuePair.Key)
                        {
                            export_bi.material_pf_code = keyValuePair.Value.ToString();
                            isDataMetSearchCriteria = true;
                        }
                    }
                }

                if (!isDataMetSearchCriteria)
                {
                    export_bi.material_pf_code = detail["material_pf_code"];
                }

                export_bi.dispensed_date = dispense_date[0] + " " + dispense_date[1]; //converted to CE.
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
                export_bi.base_value = ((export_bi.base_name.Length > 0) && (export_bi.base_name != " ")) ? detail["base_name"].ToString().Substring(detail["base_name"].ToString().ToLower().IndexOf("base")) : "";
                export_bi.price = detail["price"];
                export_bi.base_price = detail["base_price"];
                export_bi.colorant_price = detail["colorant_price"];
                export_bi.company_name = detail["company_name"];
                export_bi.company_code = detail["company_code"];

                //export_bi.material_pf_code = detail["material_pf_code"];
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
        public async Task<bool> UpdateAutotintVersion()
        {
            try
            {
            string programdata_path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            CreateDirectoryIfNotExist($"{programdata_path}\\tmp");
            File.Create($"{programdata_path}\\tmp\\dbupdate_running.tmp").Dispose();
            string auto_tint_id = ManageConfig.ReadGlobalConfig("auto_tint_id");
            string database_path = ManageConfig.ReadGlobalConfig("database_path");
            string str_response = await APIHelper.GetAutoTintVersion(client, auto_tint_id);

            APIHelperResponse response = JsonConvert.DeserializeObject<APIHelperResponse>(str_response);
            string file_total_log_path = $"{programdata_path}\\tmp\\lib_running_log.json";
            var jsonData = new ProgressCounter() { total_file = 0, complete_counter = 0, status = "Download DB File" };
            File.WriteAllText(file_total_log_path, JsonConvert.SerializeObject(jsonData), Encoding.UTF8);
            if (response.statusCode == 200 )
            {
                AutoTintWithId result = JsonConvert.DeserializeObject<AutoTintWithId>(response.message, JsonSetting);
                if(result.pos_setting == null)
                {
                    Logger.Error($"Pos_setting object is null");
                    File.Delete($"{programdata_path}\\tmp\\dbupdate_running.tmp");
                    File.Delete($"{programdata_path}\\tmp\\lib_running_log.json");
                    File.Create($"{programdata_path}\\tmp\\dbupdate_version_check.tmp").Dispose();
                    return false;
                }
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
                if (shouldDownloadNewDB)
                //if (true)
                {
                    //    //Goto download

                    
                    string downloadURI = $"{checkVersion.file}";
                    string path = ManageConfig.ReadGlobalConfig("programdata_log_path");
                    string tmp_path = $"{path}\\tmp";
                    if (!Directory.Exists(tmp_path))
                    {
                        Directory.CreateDirectory(tmp_path);
                    }
                    if (!APIHelper.APIConnectionCheck(3, 30))
                    {
                        Logger.Error("Internet Connection Error");
                        //throw new Exception("Internet Connection Error");
                        return false;
                    }
                    String[] URIArray = downloadURI.Split('/');
                    WebClient webClient = new WebClient();
                    webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCompleted);
                    webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                    webClient.QueryString.Add("fileName", $"{URIArray[URIArray.Length - 1]}");
                    webClient.QueryString.Add("newVersion", $"{checkVersion.id}");
                    webClient.DownloadFileAsync(new Uri(downloadURI), $"{tmp_path}\\{URIArray[URIArray.Length - 1]}");
                    ////Update to API about new version of database
                    //string data = @"
                    //{
                    //""pos_setting_version_id"": " + checkVersion.id + @"
                    //}
                    //";
                    //dynamic prima_pro_version_response = await APIHelper.RequestPut(client, $"/auto_tint/{auto_tint_id}/pos_update", data, auto_tint_id);
                }
                else
                {
                    //Delete is running file
                    File.Delete($"{programdata_path}\\tmp\\dbupdate_running.tmp");
                    File.Delete($"{programdata_path}\\tmp\\lib_running_log.json");
                    File.Create($"{programdata_path}\\tmp\\dbupdate_version_check.tmp").Dispose();
                }
            }
            else
            {
                //MessageBoxResult AlertMessageBox = System.Windows.MessageBox.Show($"Status Code : {response.statusCode} \nMessage : {response.message}", "Error", MessageBoxButton.OK);
                Logger.Error($"Error on get Autotint Version Status Code : {response.statusCode}  Message : {response.message}");
                //Delete is running file
                File.Delete($"{programdata_path}\\tmp\\dbupdate_running.tmp");
                File.Delete($"{programdata_path}\\tmp\\lib_running_log.json");
                File.Create($"{programdata_path}\\tmp\\dbupdate_version_check.tmp").Dispose();
                return false;
            }
            return true;
            }catch(Exception ex)
            {
                Logger.Error($"Exception on get Autotint Version : Exception {ex.Message}");
                return false;
            }
        }

        private async void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
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
                Logger.Info($"Download new update successful");
                string programdata_path = ManageConfig.ReadGlobalConfig("programdata_log_path");
                File.Delete($"{programdata_path}\\tmp\\dbupdate_running.tmp");
                File.Delete($"{programdata_path}\\tmp\\lib_running_log.json");
                File.Create($"{programdata_path}\\tmp\\dbupdate_version_check.tmp").Dispose();
                string auto_tint_id = ManageConfig.ReadGlobalConfig("auto_tint_id");
                //Update to API about new version of database
                string downLoadFileVersion = ((System.Net.WebClient)(sender)).QueryString["newVersion"];
                string data = @"
                {
                ""pos_setting_version_id"": " + downLoadFileVersion + @"
                }
                ";
                dynamic prima_pro_version_response = await APIHelper.RequestPut(client, $"/auto_tint/{auto_tint_id}/pos_update", data, auto_tint_id);
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception on DownloadCompleted : Exception {ex.Message}");
            }

        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            try
            {
                string programdata_path = ManageConfig.ReadGlobalConfig("programdata_log_path");
                if (File.Exists($"{programdata_path}\\tmp\\lib_running_log.json"))
                {
                    if (e.ProgressPercentage % 5 == 0)
                    {
                        string file_total_log_path = $"{programdata_path}\\tmp\\lib_running_log.json";
                        var jsonData = new ProgressCounter() { total_file = 0, complete_counter = e.ProgressPercentage, status = "Download DB File" };
                        File.WriteAllText(file_total_log_path, JsonConvert.SerializeObject(jsonData), Encoding.UTF8);
                    }
                }
            }catch(Exception ex)
            {
                Logger.Error($"Exception on ProgressChanged : Exception {ex.Message}");
            }
        }

        public async Task<bool> downloadBaseDB()
        {
            string auto_tint_id, pgdata_path = "";
            if (File.Exists(@"C:\ProgramData\TOA_Autotint\config.json"))
            {
                auto_tint_id = ManageConfig.ReadGlobalConfig("auto_tint_id");
                pgdata_path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            }
            else
            {
                auto_tint_id = "999999999AT01";
                pgdata_path = @"C:\ProgramData\TOA_Autotint\Log";
                CreateDirectoryIfNotExist(@"C:\ProgramData\TOA_Autotint\Log");
            }
            try
            {
                string basedata = await APIHelper.RequestGet(client, $"/base/?page=1&page_size=1", auto_tint_id);
                APIHelperResponse response = JsonConvert.DeserializeObject<APIHelperResponse>(basedata);
                if (response.statusCode == 200)
                {
                    var responseData = JsonConvert.DeserializeObject<AutoTintBase>(response.message);
                    decimal totaldata = responseData.count;
                    List<BaseData> jsonData = new List<BaseData>();

                    if (totaldata > 0)
                    {
                        var page = (int)Math.Ceiling(totaldata / 100);
                        for (int i = 1; i <= page; i++)
                        {
                            basedata = await APIHelper.RequestGet(client, $"/base/?page={i}&page_size=100", auto_tint_id);
                            response = JsonConvert.DeserializeObject<APIHelperResponse>(basedata);
                            if (response.statusCode == 200)
                            {
                                responseData = JsonConvert.DeserializeObject<AutoTintBase>(response.message);
                                jsonData.AddRange(responseData.results);
                            }
                        }
                        //Create file from data
                        string file_total_log_path = $"{pgdata_path}\\basedb.json";
                        if (File.Exists($"{pgdata_path}\\basedb.json"))
                        {
                            File.Delete($"{pgdata_path}\\basedb.json");
                        }

                        using (StreamWriter outputFile = new StreamWriter($"{pgdata_path}\\basedb.json", false, Encoding.UTF8))
                        {
                            await outputFile.WriteAsync(JsonConvert.SerializeObject(jsonData));
                        }
                        Logger.Info($"Done on on downloadBaseDB to {file_total_log_path} and Total base data {totaldata}");
                        return true;
                    }
                    Logger.Info($"Done on on downloadBaseDB : Total base data {totaldata}");
                    return true;
                };
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception on downloadBaseDB : Exception {ex.Message}");
                return false;
            }
            return false;    
        }

        public async Task<bool> downloadMaterialMapper()
        {
            string auto_tint_id, pgdata_path = "";
            if (File.Exists(@"C:\ProgramData\TOA_Autotint\material_mapper.json"))
            {
                auto_tint_id = ManageConfig.ReadGlobalConfig("auto_tint_id");
                pgdata_path = ManageConfig.ReadGlobalConfig("programdata_log_path");
            }
            else
            {
                auto_tint_id = "999999999AT01";
                pgdata_path = @"C:\ProgramData\TOA_Autotint\Logs";
                CreateDirectoryIfNotExist(@"C:\ProgramData\TOA_Autotint\Logs");
            }
            try
            {
                string mapperdata = await APIHelper.RequestGet(client, $"/material/mapper/", auto_tint_id);
                APIHelperResponse response = JsonConvert.DeserializeObject<APIHelperResponse>(mapperdata);
                if (response.statusCode == 200)
                {
                    JObject jsonData = JObject.Parse(response.message);
                    //Create file from data
                    string file_total_log_path = $"{pgdata_path}\\material_mapper.json";
                    if (File.Exists($"{pgdata_path}\\material_mapper.json"))
                    {
                        File.Delete($"{pgdata_path}\\material_mapper.json");
                    }

                    using (StreamWriter outputFile = new StreamWriter($"{pgdata_path}\\material_mapper.json", false, Encoding.UTF8))
                    {
                        await outputFile.WriteAsync(JsonConvert.SerializeObject(jsonData));
                    }
                    Logger.Info($"Done on on downloadMaterialMapper");
                    return true;
                };
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception on downloadMaterialMapper : Exception {ex.Message}");
                return false;
            }
            return false;
        }
    }
}
