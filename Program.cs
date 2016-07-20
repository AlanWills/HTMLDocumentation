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
            DirectoryInfo codeRootDirectoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\..\\..");
            string dllPath = Path.Combine(codeRootDirectoryInfo.FullName, projectPath);
            Assembly assembly = Assembly.Load("HTMLDocumentation");
            //Assembly assembly = Assembly.LoadFile(dllPath);

            // The full path to the root directory of the code - we will use this to mirror the directory tree on our webpage
            string codeRootDirectory = codeRootDirectoryInfo.FullName;
            HTMLWriter.CodeDirectoryInfo = codeRootDirectoryInfo;

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
             * NEW ALGORITHM:
             * 
             * Get all the files in the code directory and convert to a List<FileInfo>
             * Iterate over all the types.
             * For each type, find the FileInfo corresponding to it.
             * Replace the code directory with the docs directory in the file's full name to get the location of our html file.
             * Then iterate over all the directories and create a linker for each directory (no recursion in linker writer needed).
             * 
             * Means no movement of html files.
             * Should simplify directory linking.
             */

            List<FileInfo> files = codeRootDirectoryInfo.GetFiles(".cs", SearchOption.AllDirectories).ToList();
            foreach (Type type in assembly.GetTypes())
            {
                Debug.Assert(files.Exists(x => x.GetExtensionlessFileName() == type.Name));
                FileInfo fileInfoForType = files.Find(x => x.GetExtensionlessFileName() == type.Name);

                using (HTMLTypeWriter writer = new HTMLTypeWriter(type, fileInfoForType))
                {
                    writer.WriteType();
                }
            }

            foreach (DirectoryInfo directoryInfo in docsDirectoryInfo.GetDirectories("*", SearchOption.AllDirectories))
            {
                using (HTMLDirectoryLinkerWriter writer = new HTMLDirectoryLinkerWriter())
                {
                    writer.WriteDirectory();
                }
            }

            /*
             * OLD ALGORITHM
             */

            // Create the hmtl page for each class
            //foreach (Type type in assembly.GetTypes())
            //{
            //    // Overwrite the file if it exists
            //    using (HTMLTypeWriter writer = new HTMLTypeWriter(type))
            //    {
            //        writer.WriteType();
            //    }
            //}

            //// Copy the directory structure from the assembly for our html pages
            //DirectoryInfo assemblyDirectory = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\..\\..\\");

            //// Look through the directories in the first level of our code structure
            //foreach (DirectoryInfo directory in assemblyDirectory.GetDirectories("*", SearchOption.TopDirectoryOnly))
            //{
            //    if (directory.Name == "obj" || 
            //        directory.Name == "bin" || 
            //        directory.Name == "Properties" ||
            //        (directory.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
            //    {
            //        // Skip bin and obj - work out how to do this via regex in GetDirectories
            //        // Skip hidden directories
            //        continue;
            //    }

            //    // Now iterate through all the files in our valid folders
            //    foreach (FileInfo file in directory.GetFiles("*.cs", SearchOption.AllDirectories))
            //    {
            //        // We want to extract the relative path from the code root directory and create that relative structure inside the docs directory
            //        string dirName = directory.FullName.Replace(codeRootDirectory, "");

            //        if (!Directory.Exists(docsDirectory + dirName))
            //        {
            //            // Create the directory if it does not already exist
            //            Directory.CreateDirectory(docsDirectory + dirName);
            //        }

            //        // Move the html file from the flat documentation directory to the new folder mirroring the code directory structure
            //        string oldFileHTMLPath = Path.Combine(docsDirectory, file.Name.Replace(file.Extension, "") + ".html");
            //        string newFileHTMLPath = Path.Combine(docsDirectory + dirName, file.Name.Replace(file.Extension, "") + ".html");
            //        Debug.Assert(File.Exists(oldFileHTMLPath));

            //        // Copy and delete is easier than moving because we can just overwrite the file if it exists already
            //        File.Copy(oldFileHTMLPath, newFileHTMLPath, true);
            //        File.Delete(oldFileHTMLPath);
            //    }
            //}

            //// Now that all of the files have been moved to the correct created directories, we can create linking pages for each directory
            //// Do this by calling a recursive function on each directory starting with our top level
            //using (HTMLDirectoryLinkerWriter writer = new HTMLDirectoryLinkerWriter())
            //{
            //    writer.WriteDirectory();
            //}

            // Launch the docs in Chrome
            string lastName = Path.Combine(docsDirectory, "Writers", "HTMLWriter.html");
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