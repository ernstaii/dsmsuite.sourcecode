// SPDX-License-Identifier: GPL-3.0-or-later
using DsmSuite.Analyzer.Common;
using DsmSuite.Common.Util;
using System.Xml;
using System.Xml.Serialization;

namespace DsmSuite.Analyzer.Uml.Settings
{
    [Serializable]
    public class Input
    {
        public string Filename { get; set; }
    }

    [Serializable]
    public class Output
    {
        public string Filename { get; set; }
        public bool Compress { get; set; }
    }

    /// <summary>
    /// Settings used during analysis. Persisted in XML format using serialization.
    /// </summary>
    [Serializable]
    public class AnalyzerSettings : ISettings
    {
        public LogLevel LogLevel { get; set; }
        public Input Input { get; set; }
        public Output Output { get; set; }

        /// <inheritdoc/>
        public void AddInput(string fname) {
            if (!String.IsNullOrEmpty(Input.Filename))
                Logger.LogWarning("Replacing input file");
            Input.Filename = fname;
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
                Input = new Input(),
                Output = new Output(),
            };

            analyzerSettings.Input.Filename = "Model.eap";
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
            Input.Filename = FilePath.ResolveFile(settingFilePath, Input.Filename);
            Output.Filename = FilePath.ResolveFile(settingFilePath, Output.Filename);
        }
    }
}
