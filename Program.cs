using System;
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
            Console.WriteLine("Enter the relative paths of the '.dll's you wish to document separated by a space");

            //string projectsString = Console.ReadLine();
            //string[] projects = projectsString.Split(' ');

            //foreach (string projectsPath in projects)
            //{
            //    DocumentAssembly(projectsPath);
            //}

            DocumentAssembly("");
        }

        private static void DocumentAssembly(string projectPath)
        {
            // The full path to the dll
            DirectoryInfo codeDirectoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\..\\..");
            string dllPath = Path.Combine(codeDirectoryInfo.FullName, projectPath);
            Assembly assembly = Assembly.Load("HTMLDocumentation");
            //Assembly assembly = Assembly.LoadFile(dllPath);

            // The full path to the root directory of the code - we will use this to mirror the directory tree on our webpage
            string codeRootDirectory = codeDirectoryInfo.FullName;
            HTMLWriter.CodeDirectoryInfo = codeDirectoryInfo;

            // The directory where we will create the docs
            string docsDirectory = Path.Combine(dllPath, assembly.GetName().Name);

            if (Directory.Exists(docsDirectory))
            {
                // Deletes the existing documentation folder so we create a fresh one
                Directory.Delete(docsDirectory, true);
            }

            DirectoryInfo docsDirectoryInfo = Directory.CreateDirectory(docsDirectory);
            HTMLWriter.DocsDirectoryInfo = docsDirectoryInfo;

            // Create a directory in the docs directory for the Style sheets
            string stylesDirectory = Path.Combine(docsDirectory, "Styles");
            Directory.CreateDirectory(stylesDirectory);

            // Copy the style sheets into the documentation directory and override any existing ones
            File.Copy(Path.Combine(dllPath, "class.css"), Path.Combine(stylesDirectory, "class.css"), true);

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
                        writer.WriteType();
                    }
                }
            }


            // REWRITE THIS - THINK WE CAN PASS IN THE FULL PATH TO THE DOCS DIRECTORY ONLY NOW THAT ALL THE HTML FILES ARE THERE
            // DON'T NEED THE CS DIRECTORY
            foreach (DirectoryInfo directoryInfo in docsDirectoryInfo.GetDirectories("*", SearchOption.AllDirectories))
            {
                // Don't write certain pre-known directories or hidden directories.
                if (directoryInfo.Name == "bin" ||
                    directoryInfo.Name == "obj" ||
                    directoryInfo.Name == "Properties" ||
                    (directoryInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    continue;
                }

                using (HTMLDirectoryLinkerWriter writer = new HTMLDirectoryLinkerWriter())
                {
                    writer.WriteDirectory();
                }
            }
            
            // Launch the docs in Chrome
            string lastName = Path.Combine(docsDirectory, docsDirectoryInfo.Name + " Linker.html");
            lastName = lastName.Replace(" ", "%20");
            Process chrome = Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", lastName);

            // Set the focus to be the Chrome window
            string processName = chrome.ProcessName;
            string mainWindowTitle = chrome.MainWindowTitle;
            SetFocus(new HandleRef(null, chrome.MainWindowHandle));
        }

        // Need assembly info but also file structure
        // Have a skip attribute
        // Work on C++ too?
        // For each directory, add a "link" page as it were to all the directories and files directly in it
    }
}