using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseManagerLibrary;
using DatabaseManagerLibrary.CSV;
using System.Net;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows;

namespace LocationInterface.Utils
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

    class VendorAnalysis
    {
        public string OutputTableName { get; }
        public double CompletionRatio { get; protected set; }
        public Action<double> RatioChangeCallback { get; protected set; }

        public VendorAnalysis(Action<double> RatioChangeCallback, string outputTableName)
        {
            this.RatioChangeCallback = RatioChangeCallback;
            OutputTableName = outputTableName;
        }

        public bool TestConnection()
        {
            return new Ping().Send(Constants.MACVENDORAPISITE).Status == IPStatus.Success;
        }

        public void RunAnalysis(Table[] tables)
        {
            if (TestConnection())
            {
                Console.WriteLine("Loading all MAC addresses.");
                HashSet<string> uniqueMacAddresses = new HashSet<string>();
                foreach (Table table in tables)
                    foreach (Record record in table.GetRecords())
                        uniqueMacAddresses.Add((string)record.GetValue("MAC"));

                Console.WriteLine("Number of unique MAC addresses: {0}", uniqueMacAddresses.Count);

                int i = 0;
                List<string> allVendors = new List<string>();
                foreach (string vendorMACAddress in uniqueMacAddresses)
                    try
                    {
                        CompletionRatio = i++ / uniqueMacAddresses.Count;
                        RatioChangeCallback?.Invoke(CompletionRatio);
                        string responseFromServer = new WebClient().DownloadString($"http://{ Constants.MACVENDORAPISITE }/{ vendorMACAddress }");
                        allVendors.Add(responseFromServer);
                        Console.WriteLine($"{ vendorMACAddress } - { responseFromServer }");
                    }
                    catch (WebException)
                    {
                        Console.WriteLine($"{ vendorMACAddress } - WebException");
                    }

                IEnumerable<Vendor> vendors = from x in allVendors
                                              group x by x into g
                                              let count = g.Count()
                                              orderby count descending
                                              select new Vendor { Name = g.Key, UserCount = count };

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
            else if (MessageBox.Show("Could not connect to vendor API service.", "Connection Error", MessageBoxButton.YesNo) == MessageBoxResult.Yes) RunAnalysis(tables);
        }
    }
}
