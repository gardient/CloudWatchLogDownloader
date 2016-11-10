using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using CloudWatchLogDownloader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CloudWatchLogDownloader
{
    class Program
    {
        private static AmazonCloudWatchLogsClient client;
        static void Main(string[] args)
        {
            var opt = new CommandOptions();
            if (CommandLine.Parser.Default.ParseArguments(args, opt))
            {
                client = new AmazonCloudWatchLogsClient();
                var logGroup = GetLogGroup(opt.LogGroup);
                var logStream = GetLogStream(logGroup, opt.LogStream);
                WriteLogToFile(logGroup, logStream, opt.LiveStream, opt.OutputFilePath);
            }
        }

        private static LogGroup GetLogGroup(string logGroup = null)
        {
            List<LogGroup> allGroups = new List<LogGroup>();
            DescribeLogGroupsResponse lgResponse = null;
            do
            {
                lgResponse = client.DescribeLogGroups(new DescribeLogGroupsRequest { NextToken = (lgResponse != null ? lgResponse.NextToken : null) });
                allGroups.AddRange(lgResponse.LogGroups);
            } while (!string.IsNullOrWhiteSpace(lgResponse.NextToken));

            if (string.IsNullOrWhiteSpace(logGroup) || logGroup[logGroup.Length - 1] == '*')
            {
                if (!string.IsNullOrWhiteSpace(logGroup))
                {
                    logGroup = logGroup.Substring(0, logGroup.Length - 1);
                    allGroups = allGroups.Where(x => x.LogGroupName.StartsWith(logGroup)).ToList();
                }

                for (int i = 0, len = allGroups.Count; i < len; ++i)
                    Console.WriteLine(i + ") " + allGroups[i].LogGroupName);
                int num = ReadIntBetween("Choose log group: ", 0, allGroups.Count - 1);

                Console.Clear();
                Console.WriteLine("You choose LogGroup: " + allGroups[num].LogGroupName);
                return allGroups[num];
            }

            var lg = allGroups.FirstOrDefault(x => x.LogGroupName == logGroup);
            if (lg == null)
                throw new Exception("The log group '" + logGroup + "' does not exist.");

            Console.WriteLine("You choose LogGroup: " + lg.LogGroupName);
            return lg;
        }

        private static LogStream GetLogStream(LogGroup logGroup, string logStream = null)
        {
            List<LogStream> allStreams = new List<LogStream>();
            DescribeLogStreamsResponse lsResponse = null;
            do
            {
                lsResponse = client.DescribeLogStreams(new DescribeLogStreamsRequest
                {
                    NextToken = (lsResponse != null ? lsResponse.NextToken : null),
                    LogGroupName = logGroup.LogGroupName
                });
                allStreams.AddRange(lsResponse.LogStreams);
            } while (!string.IsNullOrWhiteSpace(lsResponse.NextToken));

            if (string.IsNullOrWhiteSpace(logStream) || logStream[logStream.Length - 1] == '*')
            {
                if (!string.IsNullOrWhiteSpace(logStream))
                {
                    logStream = logStream.Substring(0, logStream.Length - 1);
                    allStreams = allStreams.Where(x => x.LogStreamName.StartsWith(logStream)).ToList();
                }

                allStreams = allStreams.OrderByDescending(x => x.CreationTime).ToList();

                for (int i = 0, len = allStreams.Count; i < len; ++i)
                    Console.WriteLine(i + ") " + allStreams[i].LogStreamName);
                int num = ReadIntBetween("Choose log stream: ", 0, allStreams.Count - 1);

                Console.Clear();
                Console.WriteLine("You choose LogGroup: " + logGroup.LogGroupName + Environment.NewLine + "You choose LogStream: " + allStreams[num].LogStreamName);
                return allStreams[num];
            }

            var ls = allStreams.FirstOrDefault(x => x.LogStreamName == logStream);
            if (ls == null)
                throw new Exception("The log stream '" + logStream + "' does not exist.");

            Console.WriteLine("You choose LogStream: " + ls.LogStreamName);
            return ls;
        }

        private static void WriteLogToFile(LogGroup logGroup, LogStream logStream, bool liveStream, string outputFilePath = null)
        {
            string output = null;
            if (string.IsNullOrWhiteSpace(outputFilePath))
            {
                Console.WriteLine("Choose Output file [logs/" + logStream.LogStreamName + ".log]: ");
                output = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(output))
                    output = "logs/" + logStream.LogStreamName + ".log";
            }
            else
            {
                output = outputFilePath;
            }

            if (!Directory.GetParent(output).Exists)
                Directory.CreateDirectory(Directory.GetParent(output).FullName);

            Console.WriteLine("Downloading log into " + output);

            bool lsMessage = !liveStream;

            using (StreamWriter sw = new StreamWriter(output))
            {
                GetLogEventsResponse leResponse = null;
                do
                {
                    leResponse = client.GetLogEvents(new GetLogEventsRequest
                    {
                        LogGroupName = logGroup.LogGroupName,
                        LogStreamName = logStream.LogStreamName,
                        StartFromHead = true,
                        NextToken = (leResponse != null ? leResponse.NextForwardToken : null)
                    });

                    foreach (var ev in leResponse.Events)
                        sw.WriteLine(ev.Message);

                    sw.Flush();

                    if (!leResponse.Events.Any() && !lsMessage)
                    {
                        lsMessage = true;
                        ConsoleColor oldcolor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Now streaming live from lg: " + logGroup.LogGroupName + " ls: " + logStream.LogStreamName + " into " + output);
                        Console.ForegroundColor = oldcolor;
                        Console.WriteLine("{0}{0}Press CTRL+C to stop...", Environment.NewLine);
                    }
                } while ((leResponse.NextForwardToken != null && leResponse.Events.Any()) || liveStream);
            }
        }

        private static int ReadIntBetween(string message, int min, int max)
        {
            Console.Write(message);
            int num;
            while (!int.TryParse(Console.ReadLine(), out num) && num >= min && num <= max)
                Console.Write(Environment.NewLine + "Please enter an integer between " + min + " and " + max);
            return num;
        }
    }
}
