using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace nuspec_tool
{
    class Program
    {
        public static void ShowLogo()
        {
            Console.WriteLine("nuspec_tool v{0}", Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine("Copyright ©-2021 Topten Software. All Rights Reserved");
            Console.WriteLine();
        }

        static void ShowHelp()
        {
            Console.WriteLine("Usage: nuspec_tool <nuspecfiles> <inputfiles>");
            Console.WriteLine();
            Console.WriteLine("  Updates the version attribute of any <dependency> elements in the ");
            Console.WriteLine("  nuspec files by reading the version numbers from the <PackageReference>");
            Console.WriteLine("  elements in the input files.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --help         show this help, or help for a command");
            Console.WriteLine("  --version      show version information");
            Console.WriteLine();
            Console.WriteLine("eg: To update a .nuspec file with package versions from a .csproj files:");
            Console.WriteLine();
            Console.WriteLine("  > nuspec_tool myproj.csproj myproj.nuspec");
        }

        static int Main(string[] args)
        {
            try
            {
                var nuspecFiles = new List<string>();
                var inputFiles = new List<string>();

                foreach (var a in args)
                {
                    if (CommandLineUtils.IsSwitch(a, out var switchName, out var switchValue))
                    {
                        if (switchName == "version")
                        {
                            ShowLogo();
                            return 0;
                        }
                        if (switchName == "help")
                        {
                            ShowLogo();
                            ShowHelp();
                            return 0;
                        }
                        throw new InvalidOperationException($"Unknown switch '--{switchName}'");
                    }
                    else
                    {
                        switch (Path.GetExtension(a).ToLower())
                        {
                            case ".nuspec":
                                nuspecFiles.Add(Path.GetFullPath(a));
                                break;

                            default:
                                inputFiles.Add(Path.GetFullPath(a));
                                break;
                        }
                    }
                }

                var mapIdToVersion = new Dictionary<string, string>();

                // Process all input files
                foreach (var input in inputFiles)
                {
                    var inputText = File.ReadAllText(input);

                    var rx = new Regex("<PackageReference\\sInclude\\s?=\\s?\"(.*?)\"\\sVersion\\s?=\\s?\"(.*?)\"");
                    foreach (Match m in rx.Matches(inputText))
                    {
                        var id = m.Groups[1].Value;
                        var version = m.Groups[2].Value;

                        // Check for duplicate mismatch
                        if (mapIdToVersion.TryGetValue(id, out var currentVersion))
                        {
                            if (currentVersion != version)
                            {
                                Console.Error.WriteLine($"Warning: multiple versions found for '{id}' - '{version}' and '{currentVersion}'");
                            }
                        }

                        // Store version
                        mapIdToVersion[id] = version;
                    }
                }

                foreach (var nuspec in nuspecFiles)
                {
                    // Read the file
                    var nuspecText = File.ReadAllText(nuspec);

                    var rx = new Regex("<dependency\\sid\\s?=\\s?\"(.*?)\"\\sversion\\s?=\\s?\"(.*?)\"");

                    var updated = rx.Replace(nuspecText, (m) =>
                    {
                        var id = m.Groups[1].Value;
                        var version = m.Groups[2].Value;

                        // Ignore dependencies with variables
                        if (version.Contains('$'))
                            return m.ToString();

                        // Look up new version
                        if (!mapIdToVersion.TryGetValue(id, out var updateVersion) || updateVersion == version)
                            return m.ToString();

                        Console.WriteLine($"Updating '{id}' from '{version}' to '{updateVersion}'");

                        return m.ToString().Substring(0, m.Groups[2].Index - m.Index) +
                                updateVersion +
                               m.ToString().Substring(m.Groups[2].Index + m.Groups[2].Length - m.Index);
                    });

                    File.WriteAllText(nuspec, updated);
                }

                return 0;
            }
            catch (Exception x)
            {
                Console.Error.WriteLine(x.Message);
                return 7;
            }
        }
    }
}
