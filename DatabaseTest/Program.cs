using System;
using System.IO;
using DatabaseManagerLibrary;
using DatabaseManagerLibrary.CSV;

//rssireccount:INT,journey_type:TEXT,user_type:TEXT,apmac:TEXT,trilat_result:TEXT,floor:TEXT,area:TEXT,visit:INT,lable:TEXT,source:TEXT,ycoords:NUMBER,xcoords:NUMBER,zcoords:NUMBER,username:TEXT,assettype:TEXT,engine_ref:TEXT,start_ts:DATETIME,site:TEXT,first_ts:DATETIME,mac:TEXT,dwell:INT,building:TEXT,dwell_periods:INT,last_updated_time:DATETIME,rssifix:INT

namespace DatabaseTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Database database = new CSVDatabase("Database");
            Console.WriteLine(database);
            foreach (Table table in database.Tables)
            {
                Console.WriteLine(table);
                foreach (Record record in table.GetRecords("floor", "Deck_4"))
                    Console.WriteLine(record.ToObject<LocationRecord>());
            }
            Console.ReadKey();
        }
    }

    public class LocationRecord
    {
        [FieldIdentifier("mac")]
        public string MAC { get; set; }
        [FieldIdentifier("start_ts")]
        public DateTime Date { get; set; }
        [FieldIdentifier("floor")]
        public string Floor { get; set; }
        [FieldIdentifier("xcoords")]
        public double X { get; set; }
        [FieldIdentifier("ycoords")]
        public double Y { get; set; }

        [Ignore]
        public string Area { get; set; }

        public override string ToString()
        {
            return $"{ MAC }, { Date }, { Floor }, ({ X }, { Y })";
        }
    }
}
