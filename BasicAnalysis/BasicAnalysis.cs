using System;
using System.Collections.Generic;
using AnalysisSDK;
using DatabaseManagerLibrary;
using DatabaseManagerLibrary.CSV;

namespace BasicAnalysis
{
    class DeckCountPair
    {
        public string Deck { get; set; }
        public int Count { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1} records", Deck, Count);
        }
    }

    public class BasicAnalysisResult : IAnalysisResult
    {
        public string ShortOutputString
        {
            get
            {
                return StandardOutputString;
            }
        }

        public string StandardOutputString
        {
            get
            {
                Database database = new CSVDatabase("Analysis");
                Table table = database.GetTable("BasicAnalysis");

                string output = "";
                foreach (DeckCountPair pair in table.GetRecords<DeckCountPair>()) output += pair.ToString() + Environment.NewLine;
                return output;
            }
        }

        public string LongOutputString
        {
            get
            {
                return StandardOutputString;
            }
        }
    }

    public class BasicAnalysis : IAnalysis
    {
        public Dictionary<string, int> DeckCounts { get; set; }

        public void Run(Table[] tables, Action<double> PercentageCompletionChange)
        {
            DeckCounts = new Dictionary<string, int>();

            for (int i = 0; i < tables.Length; i++)
            {
                tables[i].SearchRecords(RecordCallback);
                PercentageCompletionChange?.Invoke(i / (double)tables.Length);
            }

            Database database = new CSVDatabase("Analysis");
            if (database.GetTable("BasicAnalysis") != null) database.DeleteTable("BasicAnalysis");
            database.SaveChanges();

            Table basicAnalysisTable = database.CreateTable("BasicAnalysis", new CSVTableFields("Deck:STRING,Count:INTEGER"));
            foreach (KeyValuePair<string, int> item in DeckCounts)
                basicAnalysisTable.AddRecord(new object[] { item.Key, item.Value });
            database.SaveChanges();

            PercentageCompletionChange?.Invoke(0);
        }

        public void RecordCallback(Record record)
        {
            LocationRecord locationRecord = record.ToObject<LocationRecord>();
            if (DeckCounts.ContainsKey(locationRecord.Deck)) DeckCounts[locationRecord.Deck]++;
            else DeckCounts[locationRecord.Deck] = 1;
        }
    }
}
