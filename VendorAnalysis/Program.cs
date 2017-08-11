using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseManagerLibrary;
using System.Net;
using System.IO;

namespace LocationAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Loading Database...");
            CSVDatabase database = new CSVDatabase("Ship");
            Console.WriteLine("Done!");
            Console.WriteLine("Fetching all records...");
            Record[] records = database.GetTable("day1").records.ToArray();
            Console.WriteLine("Count: {0}", records.Length);

            HashSet<string> uniqueMacAddresses = new HashSet<string>();
            foreach (Record record in records)
                uniqueMacAddresses.Add((string)record.GetValue("MAC"));

            Console.WriteLine("Number of unique MAC addresses: {0}", uniqueMacAddresses.Count);

            List<string> allVendors = new List<string>();
            foreach (string vendorMACAddress in uniqueMacAddresses)
            {
                try
                {
                    string responseFromServer = new WebClient().DownloadString(string.Format("http://api.macvendors.com/{0}", vendorMACAddress));
                    allVendors.Add(responseFromServer);
                    Console.WriteLine("{0} - {1}", vendorMACAddress, responseFromServer);
                }
                catch (WebException)
                {
                    Console.WriteLine("{0} - WebException", vendorMACAddress);
                }
            }

            IEnumerable<Vendor> vendors = from x in allVendors
                                          group x by x into g
                                          let count = g.Count()
                                          orderby count descending
                                          select new Vendor { Name = g.Key, UserCount = count };

            using (FileStream stream = new FileStream("C:\\Users\\grimm\\Desktop\\VendorCounts.txt", FileMode.Create))
                foreach (Vendor vendor in vendors)
                {
                    byte[] data = Encoding.UTF8.GetBytes(vendor + Environment.NewLine);
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine("{0} - {1}", vendor.Name, vendor.UserCount);
                }

            Console.ReadKey();
        }
    }

    class Vendor
    {
        public string Name { get; set; }
        public int UserCount { get; set; }

        public override string ToString()
        {
            return string.Format("\"{0}\",{1}", Name, UserCount);
        }
    }
}
