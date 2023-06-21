using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using static SfdScriptUtil.TemplateStrings;

namespace SfdScriptUtil
{

    class Program
    {
        static string version = "v1.1.2";
        static Dictionary<string, List<string>> configuration = new Dictionary<string, List<string>>();
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 1 && (args[0] == "-?" || args[0] == "--help"))
            {
                ShowHelp();
                return;
            }

            if (args.Length == 1 && (args[0] == "u" || args[0] == "uncompile"))
            {
                Reconstruct();
                return;
            }

            if (args.Length > 0 && args[0] == "init")
            {
                if (args.Length > 1)
                {
                    Init(args[1]);
                    return;
                }
                Init();
                return;
            }

            if (args.Length > 0 && args[0] == "class")
            {
                if (args.Length > 1)
                {
                    Class(args[1]);
                    return;
                }
                Class("Class");
                return;
            }




            bool dryRun = true;

            List<string> outputPaths = new List<string>();
            configuration.Add("output", outputPaths);
            configuration.Add("o", outputPaths);

            List<string> headerPaths = new List<string>();
            configuration.Add("header", headerPaths);
            configuration.Add("h", headerPaths);

            List<string> footerPaths = new List<string>();
            configuration.Add("footer", footerPaths);
            configuration.Add("f", footerPaths);

            List<string> inputFolders = new List<string>();
            configuration.Add("sourcedir", inputFolders);
            configuration.Add("s", inputFolders);

            List<string> inputFiles = new List<string>();
            configuration.Add("input", inputFiles);
            configuration.Add("i", inputFiles);

            List<string> excludeFiles = new List<string>();
            configuration.Add("exclude", excludeFiles);
            configuration.Add("e", excludeFiles);

            if (File.Exists(".sfconfig"))
            {
                foreach (string line in File.ReadAllLines(".sfconfig"))
                {
                    string[] lines = line.Split(':');
                    if (lines.Length != 2) continue;


                    string command = line.Split(':')[0].ToLower();
                    string value = line.Split(':')[1].Trim();
                    if (configuration.ContainsKey(command))
                    {
                        configuration[command].Add(value);
                    }
                    else if (command == "dry" || command == "d")
                    {
                        if (value.ToLower() == "false")
                        {
                            dryRun = false;
                        }
                    }
                    else if (command == "redux" || command == "r")
                    {
                        if (value.ToLower() == "true")
                        {
                            EditorInterface.TargetSFR();
                        }
                    }
                }
            }

            for (int i = 0; i < args.Length; i += 2)
            {
                string command = args[i].Replace("-", "");
                string value = args[i + 1];

                if (configuration.ContainsKey(command))
                {
                    configuration[command].Add(value);
                }
                else if (command == "dry" || command == "d")
                {
                    if (value.ToLower() == "false")
                    {
                        dryRun = false;
                    }

                }
                else if (command == "redux" || command == "r")
                {
                    if (value.ToLower() == "true")
                    {
                        EditorInterface.TargetSFR();
                    }
                }
            }

            string script = "";
            string editorScript = "";

            foreach (string path in headerPaths)
            {
                if (File.Exists(path))
                {
                    script += File.ReadAllText(path) + "\n";
                }
            }

            foreach (string path in inputFiles)
            {
                if (File.Exists(path))
                {
                    string content = File.ReadAllText(path);
                    content = content.Replace(startClass, start).Replace(endClass, end);
                    content = content.Replace('\t', ' ');
                    Regex rx = new Regex(Regex.Escape(start) + "(.|\n)*?" + Regex.Escape(end));
                    Match rxm = rx.Match(content);
                    if (rxm.Success)
                    {
                        script += rxm.Value.Replace(start, "/* " + Path.GetFileName(path) + " */").Replace(end, "\r\n");
                        editorScript += rxm.Value.Replace(start, "/* " + Path.GetFileName(path) + " */").Replace(end, "\r\n");
                    }
                }
            }

            foreach (string dir in inputFolders)
            {
                foreach (string path in Directory.GetFiles(dir))
                {
                    if (path.ToLower().EndsWith(".cs") && !excludeFiles.Any(e => path.EndsWith(e)))
                    {
                        string content = File.ReadAllText(path);
                        content = content.Replace(startClass, start).Replace(endClass, end);
                        content = content.Replace('\t', ' ');
                        Regex rx = new Regex(Regex.Escape(start) + "(.|\n)*?" + Regex.Escape(end));
                        Match rxm = rx.Match(content);
                        if (rxm.Success)
                        {
                            script += rxm.Value.Replace(start, "/* " + Path.GetFileName(path) + " */").Replace(end, "\r\n");
                            editorScript += rxm.Value.Replace(start, "/* " + Path.GetFileName(path) + " */").Replace(end, "\r\n");
                        }
                    }
                }
            }

            foreach (string path in footerPaths)
            {
                if (File.Exists(path))
                {
                    script += File.ReadAllText(path) + "\n";
                }
            }

            foreach (string path in outputPaths)
            {
                File.WriteAllText(path, script);
            }

            if (!dryRun)
            {
                EditorInterface.PasteScript(editorScript);
                EditorInterface.StartMap();
            }

            VersionCheck();
        }

        private static void VersionCheck()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://github.com/NotRustyBot/scriptutil/releases/latest/");
            string newest = request.GetResponse().ResponseUri.Segments.Last();
            if (newest != version)
            {
                Console.WriteLine("new version \x1b[33m[" + newest + "]\x1b[0m is available: " + "https://github.com/NotRustyBot/scriptutil/releases/latest/");
            }
        }

        private static void Class(string filename)
        {
            if (!filename.EndsWith(".cs")) filename = filename + ".cs";
            Console.WriteLine("creating " + filename);
            File.WriteAllText(filename, GetCsprojString());
            File.WriteAllText(filename, classStart + "\r\n\r\n" + classEnd);
        }

        private static void ShowHelp()
        {
            Console.WriteLine("tip: you can create .sfconfig file and use the same options in the following format:");
            Console.WriteLine("option: value");
            Console.WriteLine();
            Console.WriteLine("-o | --output [file]".PadRight(30) + "file to write the output to");
            Console.WriteLine("-h | --header [file]".PadRight(30) + "file that will be added to the begging of output. Will not be present in editor");
            Console.WriteLine("-s | --sourcedir [directory]".PadRight(30) + "every .cs file in the directory will be added to output");
            Console.WriteLine("-i | --input [file]".PadRight(30) + "file that will be added to output");
            Console.WriteLine("-e | --exclude [file]".PadRight(30) + "this file won't be included even if it is in sourcedir");
            Console.WriteLine("-f | --footer [file]".PadRight(30) + "file that will be added to the end of output. Will not be present in editor");
            Console.WriteLine("-d | --dry [false]".PadRight(30) + "when set to false, output will be pasted to the editor too, and the map will be launched.");
            Console.WriteLine("-r | --redux [true]".PadRight(30) + "when set to true, SFR map editor will be targeted for pasting the script and launching the map");
            Console.WriteLine();
            Console.WriteLine("all options, except --dry and --redux can be specified multiple times, keeping the previous value as well.");
            Console.WriteLine("be aware that .sfconfig headers and footers will be used before the parameter ones.");
            Console.WriteLine();
            Console.WriteLine("special commands");
            Console.WriteLine("u | uncompile".PadRight(30) + "creates project files from clipboard");
            Console.WriteLine("init".PadRight(30) + "creates empty project");
            Console.WriteLine("class".PadRight(30) + "creates new class file");

        }

        private static void Reconstruct()
        {
            string source = EditorInterface.GetClipboard();
            Regex regex = new Regex(@"\/\*\s([^\s\\]*)\s\*\/");
            List<string> names = new List<string>();
            foreach (Match item in regex.Matches(source))
            {
                names.Add(item.Value.Replace("/* ", "").Replace(" */", ""));
            }

            string[] codes = regex.Split(source);
            for (int i = 0; i < names.Count; i++)
            {
                Console.WriteLine("Loading " + names[i]);
                if (names[i] == "Program.cs")
                    File.WriteAllText(names[i], programStart + codes[(i + 1) * 2] + programEnd);
                else
                    File.WriteAllText(names[i], classStart + codes[(i + 1) * 2] + classEnd);
            }

        }

        private static void Init(string projectName = "SFDProject")
        {
            Console.WriteLine("creating " + projectName);
            Directory.CreateDirectory(projectName);
            File.WriteAllText(Path.Combine(projectName, projectName + ".csproj"), GetCsprojString());
            File.WriteAllText(Path.Combine(projectName, "Program.cs"), programStart + "\r\n\r\n" + programEnd);
        }
    }
}
