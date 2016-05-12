using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace pis_doc_generator
{
    class Program
    {
        private StreamWriter sw;

        Program(string logFile)
        {
            sw = File.CreateText(logFile);
        }

        void Write(string s, int indent)
        {
            s = String.Concat(Enumerable.Repeat("\t", indent)) + s;

            Console.WriteLine(s);
            sw.WriteLine(s);
            sw.Flush();
        }

        void Process(string file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            XmlNodeList processes = doc.SelectNodes("/teamworks/process");
            if (processes == null)
                return;

            foreach (XmlNode process in processes)
            {
                Write(process.Attributes?["name"].Value + " - systémová úloha", 0);

                XmlNodeList parameters;
                if ((parameters = process.SelectNodes("./processParameter")) != null)
                {
                    string input = "Vstup: ";
                    string output = "Výstup: ";
                    foreach (XmlNode parameter in parameters)
                    {
                        if (parameter.SelectSingleNode("./parameterType")?.InnerText == "1")
                            input += parameter.Attributes?["name"].Value + ", ";
                        else
                            output += parameter.Attributes?["name"].Value + ", ";
                    }

                    Write(Regex.Replace(input, ", $", ""), 1);
                    Write(Regex.Replace(output, ", $", ""), 1);
                    Write("", 0);
                }

                XmlNodeList items;
                if ((items = process.SelectNodes("./item")) != null)
                {
                    foreach (XmlNode item in items)
                    {
                        string type = item.SelectSingleNode("./tWComponentName")?.InnerText;

                        switch (type)
                        {
                            case "Script":
                                Write("Skript: " + item.SelectSingleNode("./name")?.InnerText, 1);
                                break;
                            case "WSConnector":
                                Write("Webová služba: " + item.SelectSingleNode("./name")?.InnerText, 1);
                                string wsDefXml = item.SelectSingleNode("./TWComponent/definition")?.InnerText ?? "";
                                XmlDocument wsDef = new XmlDocument();
                                wsDef.LoadXml(wsDefXml);


                                Write("Názov služby: " + wsDef.SelectSingleNode("/config/webService")?.InnerText, 2);
                                Write("Metóda služby: " + wsDef.SelectSingleNode("/config/operationName")?.InnerText, 2);

                                XmlNodeList wsParameters;
                                if ((wsParameters = wsDef.SelectNodes("/config/inputParameters/parameter")) != null)
                                {
                                    Write("Vstupy: ", 2);
                                    foreach (XmlNode parameter in wsParameters)
                                        Write(parameter.SelectSingleNode("./name")?.InnerText, 3);
                                }

                                if ((wsParameters = wsDef.SelectNodes("/config/outputParameters/parameter")) != null)
                                {
                                    Write("Výstupy: ", 2);
                                    foreach (XmlNode parameter in wsParameters)
                                        Write(parameter.SelectSingleNode("./name")?.InnerText, 3);
                                }
                                break;
                            case "Switch":
                                Write("Rozhodovací blok: " + item.SelectSingleNode("./name")?.InnerText, 1);
                                break;
                        }
                    }
                }

                Write("", 0);
                Write("", 0);
            }
        }

        static string UnpackFile(string file)
        {
            string tempDir = Regex.Replace(file, "\\.twx$", "_unpack");

            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);

            ZipFile.ExtractToDirectory(file, tempDir);
            return tempDir + Path.DirectorySeparatorChar + "objects";
        }

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Expected one argument, *.twx file");
                Environment.Exit(1);
            }

            string path = UnpackFile(args[0]);
            var files = Directory.EnumerateFiles(path, "*.xml", SearchOption.AllDirectories);

            Program me = new Program(Regex.Replace(args[0], "\\.twx$", ".txt"));

            foreach (string f in files)
            {
                Console.WriteLine("Processing file {0}...", f);
                me.Process(f);
            }
        }
    }
}
