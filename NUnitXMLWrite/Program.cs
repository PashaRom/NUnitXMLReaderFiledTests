using System;
using System.Xml;
using System.Text;
using System.IO;
using System.Linq;
using NLog;
using NUnitXMLReader.Wait;
using NUnitXMLReader.Models;

namespace NUnitXMLReader
{
    class Program
    {
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

                Argument arguments = new Argument(args);

                if (!WaitCondition.WaitAndRetry(
                    () => File.Exists(arguments.XmlPath),
                    () => {
                        var errorMessage = $"File '{arguments.XmlPath}' is not exist after retry {arguments.Retry} times.";
                        Console.WriteLine(errorMessage);
                        logger.Error(errorMessage);
                    },
                    3,
                    TimeSpan.FromSeconds(2)))
                {
                    return 2;
                }

                var xmlDoc = new XmlDocument();
                xmlDoc.Load(arguments.XmlPath);

                XmlElement? root = xmlDoc.DocumentElement;
                StringBuilder resultString = new StringBuilder();
                var counterFailedTest = 0;
                if (root != null)
                {
                    XmlNodeList testCasesList = root.GetElementsByTagName(testCaseElementName);
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
                                    resultString.Append($"cat == {result} || ");
                                    counterFailedTest++;
                                    foreach(var depTest in configuration.Dependences)
                                    {
                                        var dependent = depTest.Dependents
                                            .FirstOrDefault();

                                        if (dependent != 0)
                                        {
                                            if (!depTest.Added)
                                            {
                                                resultString.Append($"cat == {depTest.Main} || ");
                                                depTest.Added = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                var resultFailedString = resultString.Remove(resultString.Length - 3, 3).ToString();

                File.WriteAllText(arguments.ResultFilePath, resultFailedString);

                Console.WriteLine($"Failed test: {counterFailedTest}");
                Console.WriteLine(resultFailedString);
            }
            catch(Exception e)
            {
                logger.Error(e.Message);
                logger.Trace(e.StackTrace);
            }
            return 0;
        }
    }
}
