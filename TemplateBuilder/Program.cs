using System;
using System.Collections.Generic;
using System.IO;

namespace TemplateBuilder
{
    internal class Program
    {
        private static readonly List<Template> m_TemplateList = new List<Template>
                                                                {
                                                                    new Template { Name = "Mvc Website", Path = @"E:\Projects\Forumz\TemplateBuilder\TemplateBuilder\Templates\Forumz", Replace = "Forumz" }
                                                                };

        private static readonly List<string> m_Replacable = new List<string>
                                                            {
                                                                ".asax",
                                                                ".config",
                                                                ".cs",
                                                                ".cshtml",
                                                                ".csproj",
                                                                ".css",
                                                                ".css",
                                                                ".js",
                                                                ".less",
                                                                ".sln"
                                                            };

        private static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the template generator!\n\n");

            #region Which Template

            Console.WriteLine("Available Templates are:\n");

            var count = 1;
            foreach (var item in m_TemplateList)
                Console.WriteLine("{0}. {1}", count++, item.Name);

            Console.WriteLine("\nPlease select which template to create.");
            Console.Write("Enter a numnber (1-{0}): ", --count);

            var selectedValue = 0;
            while (true)
            {
                var value = Console.ReadLine();

                if (int.TryParse(value, out selectedValue))
                {
                    if (selectedValue > 0 && selectedValue <= m_TemplateList.Count)
                    {
                        selectedValue--;
                        break;
                    }
                }

                Console.WriteLine("I'm sorry, I don't understand.");
                Console.Write("Enter a numnber (1-{0}): ", count);
            }

            #endregion

            #region Output Path

            Console.Write("\nPlease enter a path to a directory to spit this out to\n->");
            var selectedPath = string.Empty;

            while (true)
            {
                selectedPath = Console.ReadLine();
                if (Directory.Exists(selectedPath))
                    break;

                Console.WriteLine("I'm sorry, That path doesn't exist.");
                Console.Write("\nPlease enter a path to a directory to spit this out to\n->");
            }

            #endregion

            #region Namespace

            Console.Write("Please enter the root namespace:");
            var ns = string.Empty;

            while (true)
            {
                ns = Console.ReadLine();
                Console.WriteLine("Example would be '{0}.Common'.  Are you sure you want to use this?: (Y/N)", ns);
                var key = Console.ReadKey();
                if (key.KeyChar == 'Y' || key.KeyChar == 'y')
                    break;

                Console.Write("Please enter the root namespace:");
            }

            #endregion

            ProcessTemplate(ns, m_TemplateList[selectedValue], selectedPath);
        }

        private static void ProcessTemplate(string ns, Template template, string outputPath)
        {
            var files = Directory.GetFiles(template.Path, "*.*", SearchOption.AllDirectories);
            var sourceRoot = template.Path;
            var destRoot = outputPath;
            if (!sourceRoot.EndsWith("\\"))
                sourceRoot += "\\";
            if (!destRoot.EndsWith("\\"))
                destRoot += "\\";

            foreach (var file in files)
            {
                var info = new FileInfo(file);
                var sourcePath = file;
                var destPath = info.FullName.Replace(sourceRoot, destRoot);
                destPath = destPath.Replace(template.Replace, ns);

                var dirName = Path.GetDirectoryName(destPath);
                if (!Directory.Exists(dirName))
                    Directory.CreateDirectory(dirName);

                if (m_Replacable.Contains(info.Extension))
                {
                    var contents = File.ReadAllText(sourcePath);
                    contents = contents.Replace(template.Replace, ns);
                    File.WriteAllText(destPath, contents);
                }
                else
                {
                    File.Copy(sourcePath, destPath, true);
                }
            }
        }
    }

    internal class Template
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Replace { get; set; }
    }
}