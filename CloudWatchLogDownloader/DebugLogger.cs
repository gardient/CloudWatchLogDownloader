using System;

namespace CloudWatchLogDownloader
{
    internal static class DebugLogger
    {
        private static bool _debug;
        public static bool Debug
        {
            get { return _debug; }
            set
            {
                if (value && !_debug)
                {
                    WriteLine("debug mode active");
                }
                _debug = value;
            }
        }

        public static void Write(string str)
        {
            if (_debug)
            {
                var oldcolor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(str);
                Console.ForegroundColor = oldcolor;
            }
        }

        public static void Write(object obj)
        {
            if (_debug)
            {
                var oldcolor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(obj);
                Console.ForegroundColor = oldcolor;
            }
        }

        public static void Write(string format, params object[] args)
        {
            if (_debug)
            {
                var oldcolor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(format, args);
                Console.ForegroundColor = oldcolor;
            }
        }

        public static void WriteLine(string str)
        {
            if (_debug)
            {
                var oldcolor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(str);
                Console.ForegroundColor = oldcolor;
            }
        }

        public static void WriteLine(object obj)
        {
            if (_debug)
            {
                var oldcolor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(obj);
                Console.ForegroundColor = oldcolor;
            }
        }

        public static void WriteLine(string format, params object[] args)
        {
            if (_debug)
            {
                var oldcolor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(format, args);
                Console.ForegroundColor = oldcolor;
            }
        }
    }
}
