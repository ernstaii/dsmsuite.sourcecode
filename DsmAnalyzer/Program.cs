using DsmSuite.Analyzer.Common;
using DsmSuite.Common.Util;
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
using System.Xml;
using System.Xml.Serialization;


namespace DsmAnalyzer
{
    internal class ParseException(string Msg) : Exception(Msg);  // thrown by parseArgs

    internal class Program
    {
        /// <summary>Runner for the application.</summary>
        static public void Main(string[] args)
        {
            int res = new Program().main(args);
            //Console.WriteLine("Press enter");  Console.ReadLine();
            System.Environment.Exit(res);
        }

        //======================== Commandline arguments ==============================
        /// <summary>Specified analyzer to use.</summary>
        IAnalyzer? analyzer;
        ///<summary>Specified settings file to create or read.</summary>
        string? settingsfile;
        ///<summary>Output file for the analysis.</summary>
        string? outputfile;
        ///<summary>Non-parsed commandline arguments, in commandline order.</summary>
        IEnumerable<string>? otherArgs;

        /// <summary>Maps names to supported analyzers.</summary>
        /// Keeps these in alphabetical order for a nicer help().
        private Dictionary<string, IAnalyzer> analyzers = new() {
            { "dot", null },
            { "dotnet", new DsmSuite.Analyzer.DotNet.DotNetAnalyzer() },
            { "jdeps", null },
            { "cpp", null }
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
            } else
                throw new Exception($"Internal error. Unknown command {cmd}");

            return 0;
        }


        /// <summary>
        /// Create a settings file from the default, update it with commandline arguments
        /// and write it to fname, or to stdout, if fname == "-".
        /// </summary>
        void CreateSettingsFile(IAnalyzer analyzer, string fname) {
            XmlSerializer serializer = analyzer.GetSettingsSerializer();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings() { Indent = true };
            ISettings settings = UpdateSettings(analyzer.CreateDefaultSettings());

            if (fname == "-") {
                // Note that write to stdout doesn't use the utf-8 encoding.
                using (XmlWriter xmlWriter = XmlWriter.Create(Console.Out, xmlWriterSettings)) {
                    serializer.Serialize(xmlWriter, settings);
                }
            } else {
                using (XmlWriter xmlWriter = XmlWriter.Create(fname, xmlWriterSettings)) {
                    serializer.Serialize(xmlWriter, settings);
                }
            }
        }

        /// <summary>
        /// Update settings with the options and arguments passed on the commandline and
        /// return the updated settings.
        /// </summary>
        ISettings UpdateSettings(ISettings settings) {
            if (outputfile != null)
                settings.SetOutput(outputfile);
            if (otherArgs != null)
                foreach (string arg in otherArgs)
                    settings.AddInput(arg);
            return settings;
        }


        /// <summary>
        /// Parse the given commandline arguments and return an int indicating the command to execute.
        /// Options are consumed from args, unprocessed arguments remain.
        /// </summary>
        /// <param name="args">Stack of argument; lefmost argument on top</param>
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

                bool needsarg = "cfLo".Contains(opt[1]);

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
                    case 'L': {
                            String arg = args.Pop();
                            if (!analyzers.TryGetValue(arg, out analyzer))
                                throw new ParseException($"Unknown language: {arg}");
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
                "  -L lang  Use analyse for language lang\n" +
                "  -f file  Use settings from file\n" +
                "  -c file  Create settings file\n" +
                "  -o file  Outputfile for the analysis\n" +
                "Supported languages: " + string.Join(", ", analyzers.Keys) + "\n"
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
