using System;
using System.Diagnostics;
using System.IO;
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
            string absoluteDirectory = Directory.GetCurrentDirectory() + "\\..\\..\\" + projectPath;
            Assembly assembly = Assembly.Load("HTMLDocumentation");
            //Assembly assembly = Assembly.LoadFile(absoluteDirectory);

            string docsDirectory = absoluteDirectory + assembly.GetName().Name;
            Directory.CreateDirectory(docsDirectory);
            docsDirectory += "\\";

            // Create a directory in the docs directory for the Style sheets
            Directory.CreateDirectory(docsDirectory + "Styles");

            // Copy the style sheets into the documentation directory and override any existing ones
            File.Copy(absoluteDirectory + "class.css", docsDirectory + "Styles\\class.css", true);

            foreach (Type type in assembly.GetTypes())
            {
                // Overwrite the file if it exists
                using (HTMLWriter writer = new HTMLWriter(docsDirectory, type))
                {
                    writer.WriteType();
                }
            }

            // Launch the docs in Chrome
            string lastName = docsDirectory + "HTMLWriter.html";
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