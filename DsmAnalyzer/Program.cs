using DsmSuite.Analyzer.Common;
using DsmSuite.Common.Util;


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


        /// <summary>Specified analyzer to use.</summary>
        IAnalyzer? analyzer;
        ///<summary>Specified config file to create/use.</summary>
        string? configfile;

        /// <summary>Maps names to supported analyzers.</summary>
        private Dictionary<string, IAnalyzer> analyzers = new() {
            { "dot", null },
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

            if (cmd == 'h')
                return 0;
            else if (cmd == 'c') {
                if (analyzer == null)
                    error("No analyzer specified.");
                createConfig(analyzer!, configfile!, args);
            } else if (cmd == 0) {
                if (analyzer == null  &&  configfile == null)
                    error("Neither language nor analyzer specified.");
            } else
                throw new Exception($"Internal error. Unknown command {cmd}");

            return 0;
        }


        void createConfig(IAnalyzer analyzer, string fname, IEnumerable<string> args) {
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

                bool needsarg = "fL".Contains(opt[1]);

                if (opt.Length > 2)
                    args.Push(needsarg ? opt.Remove(0,2) : opt.Remove(1, 1));
                if (needsarg  &&  args.Count() == 0)
                    throw new ParseException($"Argument required for {opt}");

                switch (opt[1]) {
                    case 'c':
                        command = 'c';
                        configfile = args.Pop();
                        break;
                    case 'f':
                        configfile = args.Pop();
                        break;
                    case 'h':
                        help();
                        return 'h';
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
                "Usage: DsmAnalyzer [options] [inputs...]\n" +
                "  -h       Show this help text\n"+
                "  -V       Show version information\n" +
                "  -f file  Use config from file\n" +
                "  -c file  Create config file\n" +
                ""
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
