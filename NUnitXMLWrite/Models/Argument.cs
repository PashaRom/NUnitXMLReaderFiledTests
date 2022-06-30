using System;
using System.IO;

namespace NUnitXMLReader.Models
{
    public class Argument
    {
        private readonly string[] _args;
        public Argument(string[] args)
        {
            _args = args;
            InitArgs();
        }
        public string XmlPath { get; private set; }
        public string ResultFilePath { get; private set; } = Path.Combine(Directory.GetCurrentDirectory(), "resultFailedTest.txt");
        public int Retry { get; private set; }
        public int Timeout { get; private set; }

        private void InitArgs()
        {
            foreach (var arg in _args)
            {
                if (arg.Contains("xmlPath"))
                {
                    XmlPath = GetArgValue(arg);
                }
                else if (arg.Contains("resultFilepath"))
                {
                    ResultFilePath = GetArgValue(arg);
                }
                else if (arg.Contains("-h"))
                {
                    PrintInfo();
                    return;
                }
                else if (arg.Contains("retry"))
                {
                    Retry = int.Parse(GetArgValue(arg));
                }
                else if (arg.Contains("timeout"))
                {
                    Timeout = int.Parse(GetArgValue(arg));
                }
            }
        }

        private string GetArgValue(string arg) => arg.Split("=")[1];
        private static void PrintInfo()
        {
            Console.WriteLine(" - xmlPath: Set the path to the intput NUnit v3 XML file.\n");

            Console.WriteLine(" - resultFilepath: Set the path to the output .txt file with failed tests." +
                "Default value: Directory.GetCurrentDirectory() + resultFailedTest.txt \n");

            Console.WriteLine(" - retry: The number of repetitions to check for the presence of an xml file");

            Console.WriteLine(" - timeout: time of retrying to check for the presence of an xml file, sec");

            Console.WriteLine(" - -h: info");
        }
    }
}
