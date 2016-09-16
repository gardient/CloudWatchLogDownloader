﻿using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using CloudWatchLogDownloader.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
                WriteLogToFile(logGroup, logStream, opt.OutputFilePath);
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

                return allGroups[num];
            }

            var lg = allGroups.FirstOrDefault(x => x.LogGroupName == logGroup);
            if (lg == null)
                throw new Exception("The log group '" + logGroup + "' does not exist.");

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
                    NextToken = (lsResponse != null ? lsResponse.NextToken : null)
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

                for (int i = 0, len = allStreams.Count; i < len; ++i)
                    Console.WriteLine(i + ") " + allStreams[i].LogStreamName);
                int num = ReadIntBetween("Choose log stream: ", 0, allStreams.Count - 1);

                return allStreams[num];
            }

            var ls = allStreams.FirstOrDefault(x => x.LogStreamName == logStream);
            if (ls == null)
                throw new Exception("The log stream '" + logGroup + "' does not exist.");

            return ls;
        }

        private static void WriteLogToFile(LogGroup logGroup, LogStream logStream, string outputFilePath = null)
        {
            client.GetLogEvents(new GetLogEventsRequest("", ""));
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
