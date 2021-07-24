using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using AutoTintLibrary;
using System.Diagnostics;
using System.Threading;

namespace StandaloneConverterApp
{
    public partial class ReportConverter : Form
    {
        public ReportConverter()
        {
            InitializeComponent();
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Text = " History Converter " + version.Major + "." + version.Minor;// + " (build " + version.Build + ")"; //change form title
        }

        public CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
            HeaderValidated = null,
            IgnoreBlankLines = true,
            PrepareHeaderForMatch = args => args.Header.Replace(" ", "")

        };
        dynamic JsonSetting = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };
        string csv_filepath = string.Empty;
        string save_location = string.Empty;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        string selected_filename = string.Empty;
        private void selectFilebtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "csv (*.csv)|*.csv|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    csv_filepath = openFileDialog.FileName;
                }
                fileNameTbx.Text = $"{csv_filepath}";
                selected_filename = openFileDialog.SafeFileName;
            }

        }
        private void convertBtn_Click(object sender, EventArgs e)
        {
            
            SaveFileDialog dialog = new SaveFileDialog();
            string initDirforConvert = "";
            if (File.Exists(@"C:\ProgramData\TOA_Autotint\config.json"))
            {
                string csv_history_path = ManageConfig.ReadGlobalConfig("csv_history_path");
                initDirforConvert = csv_history_path;
            }
            else
            {
                initDirforConvert = "C:\\";
            }

            dialog.InitialDirectory = initDirforConvert;
            dialog.Filter = "csv (*.csv)|*.csv|All files (*.*)|*.*";
            dialog.Title = "Save an csv File";
            if (selected_filename != string.Empty) {
                string[] tmp = selected_filename.Split('.');
                dialog.FileName = $"{tmp[0]}_bi.{tmp[1]}";
            }

            if(dialog.ShowDialog() == DialogResult.Cancel) return;
            if (dialog.FileName != string.Empty)
            {
                exportPathTbx.Text = dialog.FileName;
                save_location = dialog.FileName;
            }
            else
            {
                MessageBox.Show($"Please select the output location", "Error", MessageBoxButtons.OK);
                return;
            }

            if (String.IsNullOrEmpty(csv_filepath)) { MessageBox.Show($"Please select the input csv file", "Error", MessageBoxButtons.OK); return; }

            var workingThread = new Thread(new ThreadStart(converter));
            workingThread.Start();
        }
        public void converter()
        {
            statusLbl.Invoke((MethodInvoker)(() =>
            {
                statusLbl.Text = "Running ...";
            }));
            
            string temp_path = $"{save_location.Substring(0,save_location.LastIndexOf('\\'))}\\export_tmp";
            string result = convertCSV(csv_filepath, temp_path);
            APIHelperResponse res = JsonConvert.DeserializeObject<APIHelperResponse>(result);
            if (res.statusCode == 500)
            {
                Logger.Error($"Error {res.statusCode} Message : {res.message}");
                MessageBox.Show($"Error to convert {csv_filepath} to location {save_location}\nErrorCode: {res.statusCode}\nMessage : {res.message}", "Error", MessageBoxButtons.OK);
                return;
            }
            try
            {
                FileOperationLibrary fo = new FileOperationLibrary();
                
                DirectoryInfo jsonTempPath = new DirectoryInfo($"{temp_path}");
                List<DispenseHistoryBI> allRecord = new List<DispenseHistoryBI>();
                foreach (var jsonFile in jsonTempPath.GetFiles("*.json"))
                {
                    List<DispenseHistoryBI> onefileRecord = fo.convertToBIDataNew(jsonFile.FullName);
                    allRecord.AddRange(onefileRecord);
                }

                using (var writer = new StreamWriter($"{save_location}", false, System.Text.Encoding.UTF8))
                {
                    using (var csv = new CsvWriter(writer, csvConfig))
                    {
                        csv.WriteRecords(allRecord);
                    }
                }
                MessageBox.Show($"Operation Done | Export path : {save_location}", "File Content at path: " + csv_filepath, MessageBoxButtons.OK);
                statusLbl.Invoke((MethodInvoker)(() =>
                {
                    statusLbl.Text = "Complete";
                }));
                Directory.Delete(jsonTempPath.FullName,true);
                string argument = "/select, \"" + save_location + "\"";

                Process.Start("explorer.exe", argument);
            }
            catch (Exception ex)
            {
                statusLbl.Invoke((MethodInvoker)(() =>
                {
                    statusLbl.Text = $"{ex.Message}";
                }));
                Console.WriteLine("Exception " + ex.ToString());
                Logger.Error("Exception on create json _bi : " + ex.ToString());
            }
        }

        public string convertCSV(string input_path,string save_location)
        {
            statusLbl.Invoke((MethodInvoker)(() =>
            {
                statusLbl.Text = "Converting ...";
            }));
            int statusCode = 200;
            string responseMessage = string.Empty;
            try
            {
                var reader = new StreamReader(input_path);
                var csv = new CsvReader(reader, csvConfig);
                var records = csv.GetRecords<DispenseHistory>().ToList();
                records.RemoveAll(x => string.IsNullOrWhiteSpace(x.dispensed_formula_id));
                var dateList = new List<string>();
                for (int i = 0; i < records.Count(); i++)
                {
                    string[] date = records[i].dispensed_date.Split(' ');
                    int year = int.Parse(date[0].Split('/')[2]);
                    int now = DateTime.Today.Year;
                    if (year > now)
                    {
                        date[0] = $"{date[0].Split('/')[1]}/{date[0].Split('/')[0]}/{year - 543}";
                        records[i].dispensed_date = $"{date[0]} {date[1]}"; 
                    }
                    dateList.Add(date[0]);
                }
                string[] cleanDate = RemoveDuplicates(dateList);
                //save the dispenselog to file.json following date
                for (int i = 0; i < cleanDate.Count(); i++)
                {
                    var exportRecord = new List<DispenseHistory>();
                    for (int j = 0; j < records.Count(); j++)
                    {

                        if (records[j].dispensed_date.Contains(cleanDate[i]))
                        {
                            exportRecord.Add(records[j]);
                        }

                    }
                    if (exportRecord.Count > 0)
                    {
                        CreateDirectoryIfNotExist($"{save_location}");
                        var export_path = $"{save_location}\\full_dispense_log_{cleanDate[i].Replace("/", "_")}.json";
                        Logger.Info("Export log path : " + export_path);
                        File.WriteAllText(export_path, JsonConvert.SerializeObject(exportRecord), Encoding.UTF8);
                    }
                    else
                    {
                        Logger.Info($"Doesn't have match dispense date in csv with cleanDate ({cleanDate[i].Replace("/", "_")})");
                    }
                }
                
                reader.Close();

                return JsonConvert.SerializeObject(new { statusCode = 200, message = responseMessage });
            }catch(Exception ex)
            {
                return JsonConvert.SerializeObject(new { statusCode = 500, message = ex.Message });
            }
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



