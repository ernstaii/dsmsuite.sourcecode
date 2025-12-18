using System.Xml.Serialization;

namespace DsmSuite.Analyzer.Common {
    public interface IAnalyzer {
        /// <summary>
        /// Create and return a new settings object with the proper defaults for the language.
        /// </summary>
        public ISettings CreateDefaultSettings();
        /// <summary>
        /// Return an XMLSerializer that can be used for reading/writing a settings file for
        /// the language.
        /// </summary>
        XmlSerializer GetSettingsSerializer();

    }
}
