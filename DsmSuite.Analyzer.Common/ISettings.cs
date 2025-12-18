using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DsmSuite.Analyzer.Common {
    public interface ISettings {
        /// <summary>
        /// Add the given filename to the input files. May throw an exception if adding
        /// files (or this type of file) is not supported.
        /// </summary>
        void AddInput(string fname);
        /// <summary>
        /// Set the output file for the results of the analysis. Usually ends in '.dsi'.
        /// </summary>
        /// <param name="fname"></param>
        void SetOutput(string fname);
    }
}
