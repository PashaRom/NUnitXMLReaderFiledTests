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

                if(WaitCondition.Retry(
                    () => !File.Exists(arguments.XmlPath),
                    arguments.Retry,
                    TimeSpan.FromSeconds(arguments.Timeout)))
                {
                    var errorMessage = $"File '{arguments.XmlPath}' is not exist after retry {arguments.Retry} times.";
                    Console.WriteLine(errorMessage);
                    logger.Error(errorMessage);
                }

                var xmlDoc = new XmlDocument();
                xmlDoc.Load(arguments.XmlPath);

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
                                        var dependent = depTest.Dependents
                                            .Where(item => item == result)
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

                File.WriteAllText(arguments.ResultFilePath, resultString.ToString());
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
