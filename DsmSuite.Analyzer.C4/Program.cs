using DsmSuite.Analyzer.C4.Settings;
using DsmSuite.Analyzer.Common;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Common.Util;
using System.Reflection;
using System.Xml.Serialization;

namespace DsmSuite.Analyzer.C4
{
    public class C4Analyzer : IAnalyzer {
        ///<inheritdoc/>
        public ISettings CreateDefaultSettings() {
            return AnalyzerSettings.CreateDefault();
        }

        ///<inheritdoc/>
        public XmlSerializer GetSettingsSerializer() {
            return new XmlSerializer(typeof(AnalyzerSettings));
        }

        ///<inheritdoc/>
        public void Analyze(ISettings settings, string? path) {
            AnalyzerSettings s = (AnalyzerSettings) settings;
            s.ResolvePaths(path);
            new ConsoleAction(s).Execute();
        }
    }

    public class ConsoleAction : ConsoleActionBase
    {
        private readonly AnalyzerSettings _analyzerSettings;

        public ConsoleAction(AnalyzerSettings analyzerSettings) : base("Analyzing C4 code")
        {
            _analyzerSettings = analyzerSettings;
        }

        protected override bool CheckPrecondition()
        {
            bool result = true;
            if (!File.Exists(_analyzerSettings.Input.Workspace))
            {
                result = false;
                Logger.LogUserMessage($"Input Workspace '{_analyzerSettings.Input.Workspace}' does not exist.");
            }
            return result;
        }

        protected override void LogInputParameters()
        {
            Logger.LogUserMessage($"Input directory:{_analyzerSettings.Input.Workspace}");
        }

        protected override void Action()
        {
            DsiModel model = new DsiModel("Analyzer", _analyzerSettings.Transformation.IgnoredNames, Assembly.GetExecutingAssembly());

            Analysis.Analyzer analyzer = new Analysis.Analyzer(model, _analyzerSettings, this);
            analyzer.Analyze();

            model.Save(_analyzerSettings.Output.Filename, _analyzerSettings.Output.Compress, this);
            Logger.LogUserMessage($"Found elements={model.CurrentElementCount} relations={model.CurrentRelationCount} resolvedRelations={model.ResolvedRelationPercentage:0.0}%");
        }

        protected override void LogOutputParameters()
        {
            Logger.LogUserMessage($"Output file: {_analyzerSettings.Output.Filename} compressed={_analyzerSettings.Output.Compress}");
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            Logger.Init(Assembly.GetExecutingAssembly(), true);

            if (args.Length < 1)
            {
                Logger.LogUserMessage("Usage: DsmSuite.Analyzer.C4 <settingsfile>");
            }
            else
            {
                FileInfo settingsFileInfo = new FileInfo(args[0]);
                if (!settingsFileInfo.Exists)
                {
                    AnalyzerSettings.WriteToFile(settingsFileInfo.FullName, AnalyzerSettings.CreateDefault());
                    Logger.LogUserMessage("Settings file does not exist. Default one created");

                }
                else
                {
                    AnalyzerSettings analyzerSettings = AnalyzerSettings.ReadFromFile(settingsFileInfo.FullName);
                    Logger.LogLevel = analyzerSettings.LogLevel;

                    ConsoleAction action = new ConsoleAction(analyzerSettings);
                    action.Execute();
                }
            }

            Logger.Flush();
        }
    }
}
