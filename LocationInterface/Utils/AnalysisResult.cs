using DatabaseManagerLibrary;
using DatabaseManagerLibrary.CSV;
using System;

namespace LocationInterface.Utils
{
    public abstract class AnalysisResult
    {
        public abstract string ShortOutputString { get; }
        public abstract string StandardOutputString { get; }
        public abstract string LongOutputString { get; }
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

        public VendorAnalysisResult(string tableName)
        {
            ResultsTable = new CSVDatabase(SettingsManager.Active.AnalysisFolder).GetTable(tableName);
        }
    }
}