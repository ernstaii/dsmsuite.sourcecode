// SPDX-License-Identifier: GPL-3.0-or-later
using DsmSuite.Analyzer.Common;
using DsmSuite.Common.Util;
using System.Xml;
using System.Xml.Serialization;

namespace DsmSuite.Analyzer.Dot.Settings
{
    [Serializable]
    public class InputSettings
    {
        public string DotFileDirectory { get; set; }
    }

    [Serializable]
    public class TransformationSettings
    {
        public List<string> IgnoredNames { get; set; }
    }

    [Serializable]
    public class OutputSettings
    {
        public string Filename { get; set; }
        public bool Compress { get; set; }
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
            if (!String.IsNullOrEmpty(Input.DotFileDirectory))
                Logger.LogWarning("Replacing input file");
            Input.DotFileDirectory = fname;
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
                Input = new InputSettings(),
                Transformation = new TransformationSettings(),
                Output = new OutputSettings(),
            };

            analyzerSettings.Input.DotFileDirectory = "";

            analyzerSettings.Transformation.IgnoredNames = new List<string>();

            analyzerSettings.Output.Filename = "Output.dsi";
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
            Input.DotFileDirectory = FilePath.ResolveFile(settingFilePath, Input.DotFileDirectory);
            Output.Filename = FilePath.ResolveFile(settingFilePath, Output.Filename);
        }
    }
}
