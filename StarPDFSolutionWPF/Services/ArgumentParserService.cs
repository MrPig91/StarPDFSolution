using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarPDFSolutionWPF.Services
{
    public static class ArgumentParserService
    {
        private const char _delimiter = '|';

        public static string PrepareMessage(string[] args)
        {
            return string.Join(_delimiter, args);
        }

        public static string[] ParseMessage(string message)
        {
            return message.Split(_delimiter, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        public static Tuple<string?, List<string>> ParseStartUpArgs(string[] args)
        {
            List<string> filePaths = new List<string>();
            string? mode = null;
            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "--filepaths" && i + 1 < args.Length)
                    {
                        // Assuming file paths are comma-separated
                        filePaths = args[i + 1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
                        i++; // Move to the next argument
                    }
                    else if (args[i] == "--mode" && i + 1 < args.Length)
                    {
                        mode = args[i + 1].ToLower(); // Convert mode to lowercase for case insensitivity
                        i++; // Move to the next argument
                    }
                }
            }

            return Tuple.Create(mode, filePaths);
        }
    }
}
