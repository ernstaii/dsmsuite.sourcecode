// SPDX-License-Identifier: GPL-3.0-or-later
using DsmSuite.Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DsmSuite.Analyzer.Common {
    public interface ISettings {

        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// Add the given filename to the input files. May throw a <c>NotSupportedException</c>
        /// if adding files is not supported.
        /// </summary>
        void AddInput(string fname);

        /// <summary>
        /// Set the output file for the results of the analysis. Usually ends in '.dsi'.
        /// </summary>
        void SetOutput(string fname);
    }
}
