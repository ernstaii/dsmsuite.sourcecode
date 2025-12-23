// SPDX-License-Identifier: GPL-3.0-or-later
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

        /// <summary>
        /// Perform an analysis using the given settings. Events and messages are written to
        /// <c>Logger</c>; the caller is responsible for initalizing and finalising <c>Logger</c>.
        /// Filenames are resolved relative to the given path.
        /// </summary>
        public void Analyze(ISettings settings, string? path);

    }
}
