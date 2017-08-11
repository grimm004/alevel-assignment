using System;
using System.Diagnostics;
using DatabaseManagerLibrary;

namespace LocationAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            CSVDatabase db = new CSVDatabase("Ship");

            foreach (Record record in ((CSVTable)db.GetTable("day1")).SortRecords("", false)) Console.WriteLine(record);

            Console.ReadKey();
        }
    }
}
