using DsmSuite.Common.Util;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime;
using DsmSuite.Analyzer.Common;

namespace DsmSuite.Analyzer.C4.Settings
{
    [Serializable]
    public class InputSettings
    {
        public string Workspace { get; set; }
    }

    [Serializable]
    public class OutputSettings
    {
        public string Filename { get; set; }
        public bool Compress { get; set; }
    }

    [Serializable]
    public class TransformationSettings
    {
        public List<string> IgnoredNames { get; set; }

        public List<string> IncludedNames { get; set; }
    }

    /// <summary>
    /// Settings used during code analysis. Persisted in XML format using serialization.
    /// </summary>
    [Serializable]
    public class AnalyzerSettings : ISettings
    {
        public LogLevel LogLevel { get; set; }
        public InputSettings Input { get; set; }
        public TransformationSettings Transformation { get; set; }
        public OutputSettings Output { get; set; }

        /// <inheritdoc/>
        public void AddInput(string fname) {
            if (!String.IsNullOrEmpty(Input.Workspace))
                Logger.LogWarning("Replacing input file");
            Input.Workspace = fname;
        }

        /// <inheritdoc/>
        public void SetOutput(string fname) {
            Output.Filename = fname;
        }

        public static AnalyzerSettings CreateDefault()
        {
            AnalyzerSettings analyzerSettings = new AnalyzerSettings
            {
                LogLevel = LogLevel.Error,
                Input = new InputSettings
                {
                    Workspace = "workspace.json",
                },
                Transformation = new TransformationSettings(),
                Output = new OutputSettings(),
            };

            analyzerSettings.Transformation.IgnoredNames = new();

            analyzerSettings.Output.Filename = "workspace.dsi";
            analyzerSettings.Output.Compress = true;
            return analyzerSettings;
        }

        public static void WriteToFile(string filename, AnalyzerSettings analyzerSettings)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings() { Indent = true };
            XmlSerializer serializer = new XmlSerializer(typeof(AnalyzerSettings));

            using (XmlWriter xmlWriter = XmlWriter.Create(filename, xmlWriterSettings))
            {
                serializer.Serialize(xmlWriter, analyzerSettings);
            }
        }

        public static AnalyzerSettings ReadFromFile(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AnalyzerSettings));

            AnalyzerSettings analyzerSettings;
            using (XmlReader reader = XmlReader.Create(filename))
            {
                analyzerSettings = (AnalyzerSettings)serializer.Deserialize(reader);
            }

            analyzerSettings.ResolvePaths(Path.GetDirectoryName(filename));
            return analyzerSettings;
        }

        public void ResolvePaths(string settingFilePath)
        {
            Output.Filename = FilePath.ResolveFile(settingFilePath, Output.Filename);
        }
    }
}
