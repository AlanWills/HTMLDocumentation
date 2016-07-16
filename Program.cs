using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace HTMLDocumentation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the relative paths of the '.dll's you wish to document separated by a space");

            string projectsString = Console.ReadLine();
            string[] projects = projectsString.Split(' ');

            foreach (string projectsPath in projects)
            {
                DocumentAssembly(projectsPath);
            }
        }

        private static void DocumentAssembly(string projectPath)
        {
            string absoluteDirectory = Directory.GetCurrentDirectory() + "\\" + projectPath;
            Assembly assembly = Assembly.Load("HTMLDocumentation");
            //Assembly assembly = Assembly.LoadFile(absoluteDirectory);

            string docsDirectory = @"C:\Users\alawi\Documents\\Documentation\\" + assembly.GetName().Name;
            Directory.CreateDirectory(docsDirectory);
            docsDirectory += "\\";

            string lastName = "";
            foreach (Type type in assembly.GetTypes())
            {
                // Overwrite the file if it exists
                using (HTMLWriter writer = new HTMLWriter(docsDirectory, type))
                {
                    writer.WriteType();
                    lastName = docsDirectory + type.Name + ".html";
                }
            }

            Process.Start(@"C:\Program Files\Google\Chrome\Application\chrome.exe", lastName);
        }

        // Need assembly info but also file structure
        // Have a skip attribute
        // Document properties, fields, events
        // Document methods and their parameters, return types etc. templated arguments?
        // Only methods for this class or ones that override virtual ones. 
        // Work on C++ too?
        // For each directory, add a "link" page as it were to all the directories and files directly in it
    }
}