using System;
using DatabaseManagerLibrary;
using DatabaseManagerLibrary.BIN;

namespace LocationAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss"));

            //Database db = new BINDatabase("Ship");

            ////db.CreateTable("TestTable", new BINTableFields(new BINField("number", Datatype.Integer), new BINField("datetime", Datatype.DateTime)));
            ////db.SaveChanges();

            //Table table = db.GetTable("TestTable");
            
            ////for (int i = 0; i < 10; i++) table.AddRecord(new object[] { i, DateTime.Now });
            ////db.SaveChanges();

            //Console.WriteLine(db.GetTable("TestTable"));
            //foreach (Record record in table.GetRecords()) Console.WriteLine(record);

            Console.ReadKey();
        }
    }
}
