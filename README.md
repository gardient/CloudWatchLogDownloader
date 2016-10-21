# CloudWatchLogDownloader
Download full logs from AWS CloudWatch

## Who is it for?
For idiots like me who just want a text dump of the cloud watch log without going through S3

## Why .NET?
Because I was working with the .NET AWSSDK when writing this.

## Why not .NET CORE?
Because the AWSSDK.CloudWatchLogs nuget package doesn't support it. (As of writing this README)

## Usage
If you're still reading this means you want to use this abomination... Good.

### Command line params
|short|long|Description|
|-----|----|-----------|
|-g|--logGroup|The log group you want to select the stream from, can end with '*' in which case the option to choose from all groups starting with this will be given|
|-s|--logStream|The log stream you want to save, can end with '*' in which case the option to choose from all groups starting with this will be given|
|-o|--outputFile|The file to output the logs to.|

If any of the above are not specified the program will prompt for them
