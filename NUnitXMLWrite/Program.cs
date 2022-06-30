using System;
using System.Xml;
using System.Text;
using System.IO;
using System.Linq;
using NLog;

namespace NUnitXMLReader
{
    class Program
    {
        private static string _xmlPath = String.Empty;
        private static string _resultFilePath = Path.Combine(Directory.GetCurrentDirectory(), "resultFailedTest.txt");
        private const string testCaseElementName = "test-case";
        private const string resultAtributeName = "result";
        private const string resaltAtributeValue = "Failed";
        private const string propertyElementName = "property";
        private const string propertyAtributName = "value";
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static int Main(string[] args)
        {
            try
            {
                var configuration = Configuration.Get;

                if(args.Length == 0)
                {
                    Console.WriteLine("Please enter arguments or use -h");
                    return 1;
                }

                foreach(var arg in args)
                {
                    if(arg.Contains("xmlPath"))
                    {
                        _xmlPath = GetArgValue(arg);
                    }
                    else if (arg.Contains("resultFilepath"))
                    {
                        _resultFilePath = GetArgValue(arg);
                    }
                    else if(arg.Contains("-h"))
                    {
                        PrintInfo();
                        return 1;
                    }

                }

                var xmlDoc = new XmlDocument();
                xmlDoc.Load(_xmlPath);

                XmlElement? root = xmlDoc.DocumentElement;
                StringBuilder resultString = new StringBuilder();

                if(root != null)
                {
                    XmlNodeList testCasesList = root.GetElementsByTagName(testCaseElementName);
                    var counter = 1;
                    foreach(XmlElement testCase in testCasesList)
                    {
                        var resultAtribute = testCase.GetAttribute(resultAtributeName);
                        if(resultAtribute != null && resultAtribute.Equals(resaltAtributeValue))
                        {
                            XmlNodeList propertyElements = testCase.GetElementsByTagName(propertyElementName);

                            foreach(XmlElement property in propertyElements)
                            {
                                var numberOfTestCase = property.GetAttribute(propertyAtributName);
                                if(int.TryParse(numberOfTestCase, out int result))
                                {
                                    var stringResult = string.Empty;

                                    if(counter == 1 || testCasesList.Count == counter)
                                    {
                                        stringResult = $"cat == {result}";
                                    }
                                    else if(testCasesList.Count != counter)
                                    {
                                        stringResult = $"cat == {result} || ";
                                    }

                                    Console.Write(stringResult);
                                    resultString.Append(stringResult);

                                    foreach(var depTest in configuration.Dependences)
                                    {
                                        var dependent = depTest.Dependents.Where(item => item == result)
                                            .FirstOrDefault();

                                        if (dependent != 0)
                                        {
                                            if (!depTest.Added)
                                            {
                                                resultString.Append(testCasesList.Count != counter ? $"cat == {depTest.Main} || " : $"cat == {depTest.Main}");
                                                depTest.Added = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        counter++;
                    }

                }

                File.WriteAllText(_resultFilePath, resultString.ToString());
            }
            catch(Exception e)
            {
                logger.Error(e.Message);
                logger.Trace(e.StackTrace);
            }
            return 0;
        }

        private static string GetArgValue(string arg) => arg.Split("=")[1];

        private static void PrintInfo()
        {
            Console.WriteLine(" - xmlPath: Set the path to the intput NUnit v3 XML file.\n");

            Console.WriteLine(" - resultFilepath: Set the path to the output .txt file with failed tests." +
                "Default value: Directory.GetCurrentDirectory() + resultFailedTest.txt \n");

            Console.WriteLine(" - -h: info");
        }
    }
}
