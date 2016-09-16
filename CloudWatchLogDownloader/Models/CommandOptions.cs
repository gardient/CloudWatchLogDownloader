using CommandLine;
using CommandLine.Text;

namespace CloudWatchLogDownloader.Models
{
    internal class CommandOptions
    {
        [Option('g', "logGroup", HelpText = "The log group you want to select the stream from, can end with '*' in which case the option to choose from all groups starting with this will be given")]
        [ValueOption(0)]
        public string LogGroup { get; set; }

        [Option('s', "logStream", HelpText = "The log stream you want to save, can end with '*' in which case the option to choose from all groups starting with this will be given")]
        [ValueOption(1)]
        public string LogStream { get; set; }

        [Option('o', "outputFile", HelpText = "The file to output the logs to")]
        [ValueOption(2)]
        public string OutputFilePath { get; set; }

        [ParserState]
        public IParserState ParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
