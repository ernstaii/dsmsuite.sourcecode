using System.Xml.Serialization;

namespace DsmAnalyzer {

    [Serializable]
    public class DsmAnalyzerSettings {
        public string? Language { get; set; }
        public string? Preprocessor { get; set; }
    }
}
