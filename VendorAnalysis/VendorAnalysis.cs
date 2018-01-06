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
    
    class VendorAnalysis : IAnalysis
    {
        public double CompletionRatio { get; protected set; }

        public string Description { get { return "Analyse the MAC addresses for their vendors."; } }

        public const string API = "api.macvendors.com";

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

        public void Run(Table[] tables, Action<double> ratioChangeCallback)
        {
            Console.WriteLine("Loading all MAC addresses.");
            HashSet<string> uniqueMacAddresses = new HashSet<string>();
            foreach (Table table in tables)
                foreach (Record record in table.GetRecords())
                    uniqueMacAddresses.Add((string)record.GetValue("MAC"));

            Console.WriteLine("Number of unique MAC addresses: {0}", uniqueMacAddresses.Count);

            foreach (string vendorMACAddress in uniqueMacAddresses) Console.WriteLine(vendorMACAddress);

            int addressesCompleted = 0;
            List<string> allVendors = new List<string>();
            foreach (string vendorMACAddress in uniqueMacAddresses)
            {
                try
                {
                    CompletionRatio = addressesCompleted++ / (float)uniqueMacAddresses.Count;
                    ratioChangeCallback?.Invoke(CompletionRatio);
                    string responseFromServer = new WebClient().DownloadString($"http://{ API }/{ vendorMACAddress }");
                    allVendors.Add(responseFromServer);
                    Console.WriteLine($"{ vendorMACAddress } - { responseFromServer }");
                }
                catch (WebException)
                {
                    Console.WriteLine($"{ vendorMACAddress } - WebException");
                }
            }

            IEnumerable<Vendor> vendors = from vendor in allVendors
                                          group vendor by vendor into vendorGroup
                                          let count = vendorGroup.Count()
                                          orderby count descending
                                          select new Vendor { Name = vendorGroup.Key, UserCount = count };

            using (FileStream stream = new FileStream($"Analysis\\VendorAnalysis-{ DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") }.csv", FileMode.Create))
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

        public AnalysisResult FetchResult(string metadata)
        {
            Table resultsTable = new CSVDatabase("Analysis").GetTable($"VendorAnalysis-{ metadata }");
            if (resultsTable == null) return new AnalysisResult { Outcome = ResultRequestOutcome.ErrInvalidMetadata, Content = "Metadata should be the date reference to the Vendor Analysis." };

            string resultString = "";
            foreach (Record record in resultsTable.GetRecords()) resultString += $"{ record.GetValue<string>("Name") } - { record.GetValue<int>("Count") }{ Environment.NewLine }";
            return new AnalysisResult(resultString);
        }
    }
}
