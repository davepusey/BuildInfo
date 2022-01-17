using BuildInfoAdapter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SetAssemblyProperties
{
    class Program
    {
        private static readonly ProductVersion productVersion = new ProductVersion();

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Usage();
            }
            else
            {
                FileInfo projectFile = new FileInfo(args[0]);

                if (projectFile.Exists == false) throw new FileNotFoundException("The specified file does not exist.", projectFile.FullName);
                if (projectFile.Extension != ".csproj") throw new ArgumentException("The specified file does not have the expected .csproj file extension.");

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(projectFile.FullName);

                if (xmlDocument.DocumentElement.HasAttribute("Sdk"))
                {
                    XmlNode xmlPropertyGroupNode = xmlDocument.SelectSingleNode("/Project/PropertyGroup");

                    bool versionElementFound = false;
                    List<XmlElement> elementsToRemove = new List<XmlElement>();

                    foreach (XmlElement xmlPropertyElement in xmlPropertyGroupNode.ChildNodes)
                    {
                        switch (xmlPropertyElement.Name)
                        {
                            case "AssemblyVersion":
                            case "FileVersion":
                                elementsToRemove.Add(xmlPropertyElement);
                                break;
                            case "Version":
                                xmlPropertyElement.InnerText = productVersion.ToString();
                                versionElementFound = true;
                                break;
                        }
                    }

                    foreach (XmlElement xmlPropertyElement in elementsToRemove)
                    {
                        xmlPropertyElement.ParentNode.RemoveChild(xmlPropertyElement);
                    }

                    if (versionElementFound == false)
                    {
                        XmlElement versionElement = xmlDocument.CreateElement("Version");
                        versionElement.InnerText = productVersion.ToString();
                        xmlPropertyGroupNode.AppendChild(versionElement);
                    }

                    xmlDocument.Save(projectFile.FullName);

                    Console.WriteLine("[" + BuildInfo.Name + "] Version updated to " + productVersion.ToString());
                }
                else
                {
                    Console.WriteLine("WARNING: The specified project file does not appear to be in the newer Microsoft.NET.Sdk format.");
                    Console.WriteLine();
                    Console.WriteLine("For more information, please refer to:");
                    Console.WriteLine("\thttps://docs.microsoft.com/en-us/dotnet/core/project-sdk/overview#project-files");
                }
            }
        }

        private static void Usage()
        {
            Console.ResetColor();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(BuildInfo.Name + " " + BuildInfo.Version);
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("DESCRIPTION");
            Console.WriteLine();
            //----------------O---------|---------|---------|---------|---------|---------|---------|---------|---------|---------|---------|---------O
            Console.WriteLine("\tUtility for automatically updating Assembly properties prior to the Visual Studio Build process.");
            Console.WriteLine();
            Console.WriteLine("\tWithin the specified Project file, the follow changes are made:");
            Console.WriteLine();
            Console.WriteLine("\t* The property Version is updated, or added if not found, to be the current UTC date and time. The value is");
            Console.WriteLine("\t  formatted using the pattern \"1.0.YYDDD.HHMM\" where YY is the year, DDD is the day of the year, HH is the");
            Console.WriteLine("\t  hour, and MM is the minute.");
            Console.WriteLine();
            Console.WriteLine("\t* The properties AssemblyVersion and FileVersion are removed if found. This forces Visual Studio to use the");
            Console.WriteLine("\t  value of the Version property for these properties too.");
            //----------------O---------|---------|---------|---------|---------|---------|---------|---------|---------|---------|---------|---------O
            Console.WriteLine();
            Console.WriteLine("USAGE");
            Console.WriteLine();
            Console.WriteLine("\t" + BuildInfo.Name + ".exe <ProjectFile>");
            Console.WriteLine();
            Console.WriteLine("OPTIONS");
            Console.WriteLine();
            Console.WriteLine("\t<ProjectFile>\tPath the the .csproj file to be updated");
        }
    }
}
