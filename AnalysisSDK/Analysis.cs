using DatabaseManagerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.ComponentModel;

namespace AnalysisSDK
{
    public interface IAnalysis
    {
        /// <summary>
        /// The name of the analysis plugin.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The desrcription of the analysis plugin.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The function called to run the analysis.
        /// </summary>
        /// <param name="tables">The selected data tables.</param>
        /// <param name="percentageCompletionChange"></param>
        void Run(Table[] tables, Action<double> percentageCompletionChange);

        /// <summary>
        /// Fetch an analysis result for emailing
        /// </summary>
        /// <param name="metadata">The metadata provided by the user</param>
        /// <returns>An analysis result representing the outcome of the analysis lookup.</returns>
        AnalysisResult FetchResult(string metadata);
    }

    public class AnalysisResult
    {
        public ResultRequestOutcome Outcome { get; set; }
        public string Content { get; set; }

        public AnalysisResult()
        {
            Outcome = ResultRequestOutcome.ErrUnknown;
            Content = "";
        }
        public AnalysisResult(ResultRequestOutcome outcome)
        {
            Outcome = outcome;
            Content = "";
        }
        public AnalysisResult(string content)
        {
            Outcome = ResultRequestOutcome.OK;
            Content = content;
        }
        
        public static AnalysisResult InvalidMetadata { get { return new AnalysisResult(ResultRequestOutcome.ErrInvalidMetadata); } }

        public static AnalysisResult PluginError(string message)
        {
            return new AnalysisResult { Outcome = ResultRequestOutcome.ErrPluginInternal, Content = message };
        }
    }

    public enum ResultRequestOutcome
    {
        OK,
        [Description("Invalid Metadata Error")]
        ErrInvalidMetadata,
        [Description("Internal Plugin Error")]
        ErrPluginInternal,
        [Description("Unknown Error")]
        ErrUnknown,
    }

    public interface IMapper
    {
        /// <summary>
        /// The name of the mapper plugin.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The description of the mapper plugin.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Called when a table is loaded by the user.
        /// </summary>
        /// <param name="tables">The tables loaded by the user</param>
        void LoadTables(Table[] tables);

        /// <summary>
        /// Called at the end of every update call of the LocationMap
        /// </summary>
        /// <param name="gameTime">Data representing timings since the last update.</param>
        void Update(GameTime gameTime);

        /// <summary>
        /// Called at the end of every draw call of the LocationMap
        /// </summary>
        /// <param name="gameTime">Data representing timings since the last draw.</param>
        /// <param name="spriteBatch">The spritebatch used to draw textures and text.</param>
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
