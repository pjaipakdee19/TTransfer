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

namespace StandaloneConverterApp
{
    public partial class ReportConverter : Form
    {
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
        public ReportConverter()
        {
            InitializeComponent();
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Text = Text + " " + version.Major + "." + version.Minor;// + " (build " + version.Build + ")"; //change form title
        }

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
            }

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                save_location = dialog.FileName;
            }

            if (String.IsNullOrEmpty(csv_filepath)) { MessageBox.Show($"Please select he csv file", "Error", MessageBoxButtons.OK); return; }
            if (String.IsNullOrEmpty(save_location)) { MessageBox.Show($"Please select the output location", "Error", MessageBoxButtons.OK); return; }
            //Read the contents of the file into a stream
            MessageBox.Show($"File path : {csv_filepath} | Export path : {save_location}", "File Content at path: " + csv_filepath, MessageBoxButtons.OK);
            Logger.Info($"Test Log File path : {csv_filepath} | Export path : {save_location}");
        }

        public async Task<string> convertCSV(string input_path)
        {
            string statusCode = string.Empty;
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
                        var export_path = $"{save_location}\\full_dispense_log_{cleanDate[i].Replace("/", "_")}.json";
                        Logger.Info("Export log path : " + export_path);
                        File.WriteAllText(export_path, JsonConvert.SerializeObject(exportRecord), Encoding.UTF8);
                    }
                    else
                    {
                        Logger.Info($"Doesn't have match dispense date in csv with cleanDate ({cleanDate[i].Replace("/", "_")})");
                    }
                }

                return JsonConvert.SerializeObject(new { statusCode = statusCode, message = responseMessage });
            }catch(Exception ex)
            {
                return JsonConvert.SerializeObject(new { statusCode = "Error", message = ex.Message });
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



