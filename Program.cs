﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace HTMLDocumentation
{
    class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr SetFocus(HandleRef hWnd);

        static void Main(string[] args)
        {
            ContentManager.CheckW3CSS();

            Console.WriteLine("Enter the relative paths of the '.dll's you wish to document separated by a space");

            //string projectsString = Console.ReadLine();
            //string[] projects = projectsString.Split(' ');

            //foreach (string projectsPath in projects)
            //{
            //    DocumentAssembly(projectsPath);
            //}
            DocumentAssembly("");
        }

        /// <summary>
        /// Take the path to the assembly and load it.
        /// Then create appropriate directories before iterating over each class and directory and creating pages for them.
        /// </summary>
        /// <param name="assemblyPath"></param>
        private static void DocumentAssembly(string assemblyPath)
        {
            string dllPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..");
            Assembly assembly = Assembly.Load("HTMLDocumentation");
            //Assembly assembly = Assembly.LoadFile(dllPath);
            Console.WriteLine("Assembly loaded successfully");

            // The directory where we will create the docs - maybe convert to user input
            string docsDirectory = Path.Combine(dllPath, assembly.GetName().Name);

            ContentManager.SetupDirectories(docsDirectory, dllPath, assembly.GetName().Name);
            DirectoryInfo codeDirectoryInfo = ContentManager.CodeDirectory;
            DirectoryInfo docsDirectoryInfo = ContentManager.DocsDirectory;

            /*
             * ALGORITHM:
             * 
             * Get all the files in the code directory and convert to a List<FileInfo>
             * Iterate over all the types.
             * For each type, find the FileInfo corresponding to it.
             * Replace the code directory with the docs directory in the file's full name to get the location of where our html file goes in the docs directory structure.
             * Then iterate over all the directories and create a linker for each directory (no recursion in linker writer needed).
             */

            List<FileInfo> files = codeDirectoryInfo.GetFiles("*.cs", SearchOption.AllDirectories).ToList();
            foreach (Type type in assembly.GetTypes())
            {
                if (files.Exists(x => x.GetExtensionlessFileName() == type.Name))
                {
                    FileInfo fileInfoForType = files.Find(x => x.GetExtensionlessFileName() == type.Name);

                    string htmlFileName = Path.Combine(fileInfoForType.Directory.FullName.Replace(codeDirectoryInfo.FullName, docsDirectoryInfo.FullName), fileInfoForType.GetExtensionlessFileName() + ".html");
                    if (!File.Exists(htmlFileName))
                    {
                        // Create the directory for the html file just in case
                        Directory.CreateDirectory(new FileInfo(htmlFileName).DirectoryName);

                        FileStream fileStream = File.Create(htmlFileName);
                        fileStream.Close();
                    }

                    using (HTMLTypeWriter writer = new HTMLTypeWriter(type, htmlFileName))
                    {
                        writer.Write();
                        Console.WriteLine("Wrote " + type.Name + " class page");
                    }
                }
            }

            // Writer root directory linker
            using (HTMLDirectoryLinkerWriter writer = new HTMLDirectoryLinkerWriter(docsDirectoryInfo))
            {
                writer.Write();
                Console.WriteLine("Wrote " + docsDirectoryInfo.Name + " directory page");
            }

            // Write linkers for all sub directories
            foreach (DirectoryInfo directoryInfo in docsDirectoryInfo.GetDirectories("*", SearchOption.AllDirectories))
            {
                // Don't write certain pre-known directories, hidden directories or directories with no pages in
                if (directoryInfo.ShouldIgnoreHTMLDirectory())
                {
                    continue;
                }

                using (HTMLDirectoryLinkerWriter writer = new HTMLDirectoryLinkerWriter(directoryInfo))
                {
                    writer.Write();
                    Console.WriteLine("Wrote " + docsDirectoryInfo.Name + " directory page");
                }
            }
            
            // Launch the docs in Chrome
            string lastName = Path.Combine(docsDirectory, docsDirectoryInfo.Name + HTMLDirectoryLinkerWriter.LinkerString);
            lastName = lastName.Replace(" ", "%20");
            Process chrome = Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", lastName);

            // Set the focus to be the Chrome window
            SetFocus(new HandleRef(null, chrome.MainWindowHandle));
        }
    }
}