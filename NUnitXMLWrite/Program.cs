using System;
using System.Xml;
using System.Text;
using System.IO;

namespace NUnitXMLWrite
{
    class Program
    {
        private static string _xmlPath = String.Empty;
        private static string _resultFilepath = Path.Combine(Directory.GetCurrentDirectory(), "resultFailedTest.txt");
        private const string testCaseElementName = "test-case";
        private const string resultAtributeName = "result";
        private const string resaltAtributeValue = "Failed";
        private const string propertyElementName = "property";
        private const string propertyAtributName = "value";
        static int Main(string[] args)
        {
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
                    _resultFilepath = GetArgValue(arg);
                }
                else if(arg.Contains("-h"))
                {
                    PrintInfo();
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
                                Console.Write(testCasesList.Count != counter ? $"cat == {result} || " : $"cat == {result}");
                                resultString.Append(testCasesList.Count != counter ? $"cat == {result} || " : $"cat == {result}");
                            }
                        }
                    }
                    counter++;
                }

            }

            File.WriteAllText(_resultFilepath, resultString.ToString());

            Console.ReadKey();
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
