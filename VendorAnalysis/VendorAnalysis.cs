using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseManagerLibrary;
using System.Net;
using System.IO;
using System.Net.NetworkInformation;
using AnalysisSDK;
using DatabaseManagerLibrary.CSV;
using System.Windows;

namespace VendorAnalysis
{
    class Vendor
    {
        public string Name { get; set; }
        public int UserCount { get; set; }

        public override string ToString()
        {
            return string.Format("\"{0}\",{1}", Name, UserCount);
        }
    }

    public class VendorAnalysisResult : AnalysisResult
    {
        public override string ShortOutputString
        {
            get
            {
                return $"{ ResultsTable.RecordCount } unique vendors.";
            }
        }
        public override string StandardOutputString
        {
            get
            {
                string resultsString = "";
                foreach (Record record in ResultsTable.GetRecords()) resultsString += $"{ record.GetValue<string>("Name") } - { record.GetValue<int>("Count") }{ Environment.NewLine }";
                return resultsString;
            }
        }
        public override string LongOutputString
        {
            get
            {
                string resultsString = "";
                foreach (Record record in ResultsTable.GetRecords()) resultsString += $"{ record.GetValue<string>("Name") } - { record.GetValue<int>("Count") }{ Environment.NewLine }";
                return resultsString;
            }
        }

        protected Table ResultsTable { get; set; }

        public VendorAnalysisResult(string folder, string tableName)
        {
            ResultsTable = new CSVDatabase(folder).GetTable(tableName);
        }
    }

    class VendorAnalysis
    {
        public string OutputTableName { get; }
        public double CompletionRatio { get; protected set; }
        public Action<double> RatioChangeCallback { get; protected set; }
        public const string API = "api.macvendors.com";

        public VendorAnalysis(Action<double> RatioChangeCallback, string outputTableName)
        {
            this.RatioChangeCallback = RatioChangeCallback;
            OutputTableName = outputTableName;
        }

        public bool TestConnection()
        {
            try
            {
                return new Ping().Send(API).Status == IPStatus.Success;
            }
            catch (PingException)
            {
                return false;
            }
        }

        public void RunAnalysis(Table[] tables)
        {
            Console.WriteLine("Loading all MAC addresses.");
            HashSet<string> uniqueMacAddresses = new HashSet<string>();
            foreach (Table table in tables)
                foreach (Record record in table.GetRecords())
                    uniqueMacAddresses.Add((string)record.GetValue("MAC"));

            Console.WriteLine("Number of unique MAC addresses: {0}", uniqueMacAddresses.Count);

            int addressesCompleted = 0;
            List<string> allVendors = new List<string>();
            bool successful = false;
            foreach (string vendorMACAddress in uniqueMacAddresses)
            {
                do
                    try
                    {
                        CompletionRatio = addressesCompleted++ / (float)uniqueMacAddresses.Count;
                        RatioChangeCallback?.Invoke(CompletionRatio);
                        string responseFromServer = new WebClient().DownloadString($"http://{ API }/{ vendorMACAddress }");
                        allVendors.Add(responseFromServer);
                        Console.WriteLine($"{ vendorMACAddress } - { responseFromServer }");
                        successful = true;
                    }
                    catch (WebException)
                    {
                        successful = false;
                        Console.WriteLine($"{ vendorMACAddress } - WebException");
                    }
                while (successful || MessageBox.Show("Could not connect to vendor API service. Retry?", "Connection Error", MessageBoxButton.YesNo) == MessageBoxResult.Yes) ;
            }

            IEnumerable<Vendor> vendors = from vendor in allVendors
                                            group vendor by vendor into vendorGroup
                                            let count = vendorGroup.Count()
                                            orderby count descending
                                            select new Vendor { Name = vendorGroup.Key, UserCount = count };

            using (FileStream stream = new FileStream($"Analysis\\{ OutputTableName }.csv", FileMode.Create))
            {
                byte[] data = Encoding.UTF8.GetBytes("Name:string,Count:integer" + Environment.NewLine);
                stream.Write(data, 0, data.Length);
                foreach (Vendor vendor in vendors)
                {
                    data = Encoding.UTF8.GetBytes(vendor + Environment.NewLine);
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine($"{ vendor.Name } - { vendor.UserCount }");
                }
            }
        }
    }
}
