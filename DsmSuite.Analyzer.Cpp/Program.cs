// SPDX-License-Identifier: GPL-3.0-or-later
using DsmSuite.Analyzer.Common;
using DsmSuite.Analyzer.Cpp.Settings;
using DsmSuite.Analyzer.Cpp.Transformation;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Common.Util;
using System.Reflection;
using System.Xml.Serialization;

namespace DsmSuite.Analyzer.Cpp
{
    public class CppAnalyzer : IAnalyzer {
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

        public ConsoleAction(AnalyzerSettings analyzerSettings) : base("Analyzing C++ source code")
        {
            _analyzerSettings = analyzerSettings;
        }

        protected override bool CheckPrecondition()
        {
            return true;
        }

        protected override void LogInputParameters()
        {
            Logger.LogUserMessage("Input directories:");
            foreach (string sourceDirectory in _analyzerSettings.Input.SourceDirectories)
            {
                Logger.LogUserMessage($" {sourceDirectory}");
            }
            Logger.LogUserMessage($"Resolve method: {_analyzerSettings.Analysis.ResolveMethod}");
        }

        protected override void Action()
        {
            DsiModel model = new DsiModel("Analyzer", _analyzerSettings.Transformation.IgnoredNames, Assembly.GetExecutingAssembly());
            Analysis.Analyzer analyzer = new Analysis.Analyzer(model, _analyzerSettings, this);
            analyzer.Analyze();

            Transformer transformer = new Transformer(model, _analyzerSettings.Transformation, this);
            transformer.Transform();

            model.Save(_analyzerSettings.Output.Filename, _analyzerSettings.Output.Compress, this);
            Logger.LogUserMessage($"Found elements={model.CurrentElementCount} relations={model.CurrentRelationCount} resolvedRelations={model.ResolvedRelationPercentage:0.0}% ambiguousRelations={model.AmbiguousRelationPercentage:0.0}%");
        }

        protected override void LogOutputParameters()
        {
            Logger.LogUserMessage($"Output file: {_analyzerSettings.Output.Filename} compressed={_analyzerSettings.Output.Compress}");
        }
    }

    public static class Program
    {
        static void Main(string[] args)
        {
            Logger.Init(Assembly.GetExecutingAssembly(), true);

            if (args.Length < 1)
            {
                Logger.LogUserMessage("Usage: DsmSuite.Analyzer.Cpp <settings-file>");
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
