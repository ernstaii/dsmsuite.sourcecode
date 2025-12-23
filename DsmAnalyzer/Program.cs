using DsmSuite.Analyzer.Common;
using DsmSuite.Common.Util;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;


namespace DsmAnalyzer
{
    internal class ParseException(string Msg) : Exception(Msg);  // thrown by parseArgs

    internal record PreProcessor(string cmd, string analyzer);

    internal class Program {
        private const string XMLROOT = "DsmAnalyzer";

        /// <summary>Runner for the application.</summary>
        static public void Main(string[] args) {
            int res = new Program().main(args);
            //Console.WriteLine("Press enter");  Console.ReadLine();
            System.Environment.Exit(res);
        }

        //======================== (Mostly) Commandline arguments ==============================
        /// <summary> Preprocessor command to run, from the commandline or settingsfile.</summary>
        string? preprocessor;
        /// <summary>
        /// Analyzer to use, from the commandline if specified; otherwise from the settingsfile.
        /// </summary>
        IAnalyzer? analyzer;
        /// <summary>Name of the used Analyzer.</summary>
        string? analyzerName;
        ///<summary>Specified settings file to create or read.</summary>
        string? settingsfile;
        ///<summary>Output file for the analysis.</summary>
        string? outputfile;
        ///<summary>Non-parsed commandline arguments, in commandline order.</summary>
        IEnumerable<string>? otherArgs;
        ///<summary>Log level for the Analyzer</summary>
        LogLevel? loglevel;

        /// <summary>Maps names to supported analyzers.</summary>
        /// Keep these in alphabetical order for a nicer help().
        private Dictionary<string, IAnalyzer> analyzers = new(StringComparer.OrdinalIgnoreCase) {
            { "C4",         new DsmSuite.Analyzer.C4.C4Analyzer() },
            { "cpp",        new DsmSuite.Analyzer.Cpp.CppAnalyzer() },
            { "dot",        new DsmSuite.Analyzer.Dot.DotAnalyzer() },
            { "dotnet",     new DsmSuite.Analyzer.DotNet.DotNetAnalyzer() },
            { "dependenpy", new DsmSuite.Analyzer.Python.PythonAnalyzer() },
            { "uml",        new DsmSuite.Analyzer.Uml.UMLAnalyzer() },
        };

        ///<summary>Maps names to preprocessor command and backend.</summary>
        private Dictionary<string, PreProcessor> preprocessors = new(StringComparer.OrdinalIgnoreCase) {
            { "jdeps",  new( "jdeps -dotoutput {TMPDIR} {INPUT}", "dot" ) },
            { "python", new( "dependenpy -l -f json -o {TMPFILE} {INPUT}", "dependenpy" ) },
        };


        int main(string[] argArray) {
            int cmd = 0;
            Stack<string> args = new(argArray.Reverse());
            try {
                cmd = parseArgs(args);
            } catch (ParseException e) {
                error(e.Message);
            }
            otherArgs = args;

            if (cmd == 'h')
                return 0;
            else if (cmd == 'c') {
                if (analyzer == null)
                    error("No analyzer specified.");
                CreateSettingsFile(analyzer!, settingsfile!);
            } else if (cmd == 0) {
                if (analyzer == null  &&  settingsfile == null)
                    error("Neither language nor analyzer specified.");
                Analyze(analyzer, settingsfile);
            } else
                throw new Exception($"Internal error. Unknown command {cmd}");

            return 0;
        }


        /// <summary>
        /// Performs an analysis. If <c>fname</c> is null, a default configuration for <c>analyzer</c>
        /// is created and completed with the commandline arguments.<br/>
        /// If <c>fname</c> is not null, the commandline arguments are completed with the settings from
        /// this file and then an analysis is executed. The commandline takes precedence, so the passed
        /// analyzer takes precedence over the one specified in the settings file.<br/>
        /// It both an analyzer and fname are null, an error is generated.
        /// </summary>
        void Analyze(IAnalyzer? analyzerOverride, string? fname) {
            ISettings settings;
            string? rootdir;
            int exitcode;

            if (fname == null) {
                settings = analyzer!.CreateDefaultSettings();
                rootdir = Directory.GetCurrentDirectory();
            } else {
                settings = ReadSettingsFile(fname);
                rootdir = new FileInfo(fname).DirectoryName;
            }
            UpdateSettings(settings);
            ConnectPreprocessor(settings);
            //WriteSettingsFile(analyzerOverride ?? this.analyzer!, settings, "-"); return;

            Logger.Init(Assembly.GetExecutingAssembly(), true);
            if (!string.IsNullOrEmpty(preprocessor)) {
                Logger.LogUserMessage($"Running preprocessor: {preprocessor}");
                exitcode = RunProgram(preprocessor);
                if (exitcode != 0) {
                    Logger.LogError("Preprocessor terminated with exitcode {exitcode}");
                    error("Preprocessor failed");
                }
            }
            (analyzerOverride ?? this.analyzer!).Analyze(settings, rootdir);
            Logger.Flush();
        }


        /// <summary>
        /// Run the command using the "standard" shell, aka system() and return its exit code.
        /// </summary>
        int RunProgram(string command) {
            ProcessStartInfo pinfo;

            if (OperatingSystem.IsWindows()) {
                pinfo = new() { FileName = "cmd.exe", Arguments = "/c "+ command };
            } else {
                pinfo = new() { FileName = "bash", ArgumentList = { "-c", command } };
            }
            pinfo.UseShellExecute = false;

            using (Process p = new()) {
                p.StartInfo = pinfo;
                p.Start();
                p.WaitForExit();
                return p.ExitCode;
            }
        }


        /// <summary>
        /// Connect the output of the preprocessor to the input of the analyzer,
        /// using a temporary file/directory.
        /// </summary>
        void ConnectPreprocessor(ISettings settings) {
            if (preprocessor == null)
                return;

            string temp;

            if (preprocessor.Contains("{TMPDIR}")) {
                temp = Directory.CreateTempSubdirectory().FullName;
                preprocessor = preprocessor.Replace("{TMPDIR}", temp);
                settings.AddInput(temp);
            } else if (preprocessor.Contains("{TMPFILE}")) {
                temp = Path.GetTempFileName();
                preprocessor = preprocessor.Replace("{TMPFILE}", temp);
                settings.AddInput(temp);
            }
        }

        /// <summary>
        /// Create a settings file from the default, update it with commandline arguments
        /// and write it to fname, or to stdout, if fname == "-".
        /// </summary>
        void CreateSettingsFile(IAnalyzer analyzer, string fname) {
            WriteSettingsFile(analyzer, UpdateSettings(analyzer.CreateDefaultSettings()), fname);
        }


        /// <summary>
        /// Write the active frontend settings together with the given Analyzer settings to a new
        /// file fname, or to stdout, if fname == "-".
        /// </summary>
        void WriteSettingsFile(IAnalyzer analyzer, ISettings settings, string fname) {
            XmlSerializer serializer = analyzer.GetSettingsSerializer();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings() { Indent = true };
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            // Note that write to stdout doesn't use the utf-8 encoding.
            using ( XmlWriter xmlWriter = fname == "-" ?
                    XmlWriter.Create(Console.Out, xmlWriterSettings) :
                    XmlWriter.Create(fname, xmlWriterSettings) ) {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement(XMLROOT);
                WriteFrontendSettings(xmlWriter);
                serializer.Serialize(xmlWriter, settings, ns);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
        }


        ///<summary>Write the frontend settings to the given XMLWriter.</summary>
        public void WriteFrontendSettings(XmlWriter writer) {
            XmlSerializer serializer = new XmlSerializer(typeof(DsmAnalyzerSettings));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            DsmAnalyzerSettings settings = new() { Language = analyzerName, Preprocessor = preprocessor };
            serializer.Serialize(writer, settings, ns);
        }


        /// <summary>
        /// Read the given settings file. Empty commandline arguments are filled with
        /// the frontend settings from the file. The analyzer settings are returned.
        /// </summary>
        ISettings ReadSettingsFile(string fname) {
            ISettings settings;
            FileInfo settingsFileInfo = new FileInfo(fname);

            if (!settingsFileInfo.Exists)
                error($"Settings file {fname} does not exist.");

            using (XmlReader reader = XmlReader.Create(settingsFileInfo.FullName)) {
                XmlForwardTo(reader, XMLROOT);
                reader.ReadStartElement(XMLROOT);
                MergeFrontendSettings(reader);  // Sets analyzer, if necessary
                settings = ReadAnalyzerSettings(analyzer!, reader);
            }

            return settings;
        }


        /// <summary>
        /// Read analyzer settings from reader and return them. In case of errors,
        /// error() is invoked.
        /// </summary>
        ISettings ReadAnalyzerSettings(IAnalyzer analyzer, XmlReader reader) {
            ISettings? settings;

            XmlSerializer serializer = analyzer.GetSettingsSerializer();
            settings = (ISettings?) serializer.Deserialize(reader);
            if (settings == null)
                error("Reading settings file failed. (not a settings file?)");

            return settings!;
        }


        ///<summary>
        /// Read the frontend settings from the given XmlReader and use them to set the
        /// arguments that were not specified on the commandline.
        /// Errors are handled by invoking error().
        ///</summary>
        public void MergeFrontendSettings(XmlReader reader) {
            XmlForwardTo(reader, nameof(DsmAnalyzerSettings));
            XmlSerializer serializer = new XmlSerializer(typeof(DsmAnalyzerSettings));
            DsmAnalyzerSettings? settings = (DsmAnalyzerSettings?) serializer.Deserialize(reader);

            if (settings == null)
                error("Reading settings file failed. (not a settings file?)");

            if (analyzer == null) {
                if (settings!.Language == null)
                    error($"No language in the settings file and not on the commandline either.");
                if (!analyzers.TryGetValue(settings.Language!, out analyzer))
                    error($"Unknown language in settings file: {settings.Language!}");
            } else if (settings!.Language != null  &&  !analyzers.ContainsKey(settings.Language))
                Console.Error.WriteLine($"Warning: Unknown language {settings.Language} in settings file.");

            if (preprocessor == null)
                preprocessor = settings.Preprocessor;
        }


        /// <summary>
        /// Update settings and class variables with the options and arguments passed on the
        /// commandline and return the updated settings.
        /// </summary>
        ISettings UpdateSettings(ISettings settings) {
            if (loglevel != null)
                settings.LogLevel = (LogLevel) loglevel;
            if (outputfile != null)
                settings.SetOutput(outputfile);
            if (otherArgs != null) {
                if (preprocessor != null) {
                    string inputs = string.Join(" ", otherArgs);
                    if (!string.IsNullOrEmpty(inputs))
                        preprocessor = preprocessor.Replace("{INPUT}", inputs);
                } else {
                    try {
                        foreach (string arg in otherArgs)
                            settings.AddInput(arg);
                    } catch (NotSupportedException) {
                        error("Inputs on the commandline not supported for the selected language.");
                    }
                }
            }
            return settings;
        }


        /// <summary>
        /// Read from reader until the next start tag. If this equals tag, return.
        /// If not, or a read error occurs, error() is invoked.
        /// </summary>
        private void XmlForwardTo(XmlReader reader, string tag) {
            do {
                if (!reader.Read())
                    error($"Failed to parse {reader.BaseURI}");
            } while (reader.NodeType != XmlNodeType.Element);
            if (reader.Name != tag)
                error($"Malformed settings file {reader.BaseURI}: expected {tag} element");
        }


        /// <summary>
        /// Parse the given commandline arguments and return an int indicating the command to execute.
        /// Options are consumed from args, unprocessed arguments remain.
        /// </summary>
        /// <param name="args">Stack of arguments; lefmost argument on top</param>
        /// <returns>0, 'c', 'h'</returns>
        /// <exception cref="ParseException"></exception>
        int parseArgs(Stack<string> args) {
            int command = 0;

            while (args.Count > 0  &&  args.Peek().StartsWith("-")) {
                String opt = args.Pop();
                if (opt.Length == 1)
                    throw new ParseException("Option required after '-'");
                if (opt == "--")
                    break;

                bool needsarg = "cfLov".Contains(opt[1]);

                if (opt.Length > 2)
                    args.Push(needsarg ? opt.Remove(0,2) : opt.Remove(1, 1));
                if (needsarg  &&  args.Count() == 0)
                    throw new ParseException($"Argument required for {opt}");

                switch (opt[1]) {
                    case 'c':
                        command = 'c';
                        settingsfile = args.Pop();
                        break;
                    case 'f':
                        settingsfile = args.Pop();
                        break;
                    case 'h':
                        help();
                        return 'h';
                    case 'o':
                        if (outputfile != null)
                            throw new ParseException($"-o can be used only once.");
                        outputfile = args.Pop();
                        break;
                    case 'v': {
                            string arg = args.Pop();
                            if (!Enum.TryParse<LogLevel>(arg, true, out var level))
                                throw new ParseException($"Unknown verbosity: {arg}");
                            loglevel = level;
                        }
                        break;
                    case 'L': {
                            string name = args.Pop();
                            if (preprocessors.TryGetValue(name, out var prep)) {
                                preprocessor = prep.cmd;
                                analyzer = analyzers[prep.analyzer];
                                analyzerName = prep.analyzer;
                            } else if (analyzers.TryGetValue(name, out analyzer)) {
                                // normalize case to that in dictionary.
                                analyzerName = analyzers.First(
                                        kvp => analyzers.Comparer.Equals(kvp.Key, name) ).Key;
                            } else
                                throw new ParseException($"Unknown language: {name}");
                        }
                        break;
                    case 'V':
                        Console.WriteLine(SystemInfo.VersionLong);
                        break;
                    default:
                        throw new ParseException($"Unknown option -{opt[1]}");
                }
            }

            return command;
        }


        private void help() {
            Console.WriteLine(
                "Usage: DsmAnalyzer [options] [inputfile...]\n" +
                "  -h       Show this help text\n"+
                "  -V       Show version information\n" +
                "  -L lang  Use analyzer for language lang\n" +
                "  -f file  Use settings from file\n" +
                "  -c file  Create settings file. Use - for stdout\n" +
                "  -o file  Outputfile for the analysis\n" +
                "  -v level Set verbosity for the analyzer, with level one of\n" +
                "           " + string.Join(", ", Enum.GetNames(typeof(LogLevel))) + "\n" +
                "Supported languages: " + string.Join(", ",
                        Enumerable.Concat( preprocessors.Keys, analyzers.Keys)) + "\n"
            );
        }

        /// <summary>
        /// Print the error message to stderr and terminate the program.
        /// </summary>
        private void error(string msg, int errorcode = 1) {
            Console.Error.WriteLine(msg);
            Environment.Exit(errorcode);
        }
    }
}
