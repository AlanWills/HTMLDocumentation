using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace HTMLDocumentation
{
    /// <summary>
    /// This class is responsible for managing information passed to it from the user.
    /// This includes the directory where the code is and the directory for the documentation.
    /// It is also responsible for making sure that any third party resources (scripts, .css files etc.) are downloaded.
    /// Provides utility functions for accessing all of this information, as well as functions for bootstrapping third party resources.
    /// </summary>
    public static class ContentManager
    {
        private static string styleFileLocation;

        /// <summary>
        /// The full file path of the w3.css style file
        /// </summary>
        public static string StyleFileLocation
        {
            get
            {
                if (styleFileLocation == null)
                {
                    styleFileLocation = Path.Combine(Directory.GetCurrentDirectory(), "w3.css"); ;
                }

                return styleFileLocation;
            }
        }

        /// <summary>
        /// The root directory for the directory tree our documentation will be created in.
        /// </summary>
        public static DirectoryInfo DocsDirectory { get; set; }

        /// <summary>
        /// The root directory for the directory tree of the .cs files in the assembly
        /// </summary>
        public static DirectoryInfo CodeDirectory { get; set; }

        #region Bootstrapping Functions

        /// <summary>
        /// This program heavily uses styles in w3.css and if not present the output will be degraded.
        /// We should check if we are connected to the internet and if the w3css folder does not exist, offer the option to download it (with a warning about quality loss).
        /// If we are not connected to the internet, we attempt to find it in a predetermined location and if it does not exist display a warning about quality loss.
        /// </summary>
        public static void CheckW3CSS()
        {
            // Check internet connection
            bool connectedToInternet = false;
            using (WebClient client = new WebClient())
            {
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    if (stream != null)
                    {
                        connectedToInternet = true;
                    }
                }
            }

            bool styleFileExists = File.Exists(StyleFileLocation);
            if (!styleFileExists)
            {
                if (connectedToInternet)
                {
                    Console.WriteLine("w3.css styling is used extensively in this program.  It cannot be detected in location: " + styleFileLocation);
                    Console.WriteLine("Do you wish to download it now (this will allow offline previewing)? (Y/N)");

                    ConsoleKeyInfo result = Console.ReadKey();
                    if (result.KeyChar == 'Y')
                    {
                        using (var client = new WebClient())
                        {
                            client.DownloadFile("http://www.w3schools.com/lib/w3.css", "w3.css");
                        }

                        Debug.Assert(File.Exists(styleFileLocation));
                        Console.WriteLine("File downloaded successfully and offline previewing now available");
                    }
                    else
                    {
                        Console.WriteLine("WARNING!");
                        Console.WriteLine("Offline previewing will show extremely poor results.");
                    }
                }
                else
                {
                    Console.WriteLine("WARNING!");
                    Console.WriteLine("w3.css styling is used extensively in this program.");
                    Console.WriteLine("With no internet connection and no local w3.css style file, previewing will show extremely poor results.");
                }
            }
        }

        #endregion

        #region Directory Functions

        /// <summary>
        /// Performs creation (and cleaning if necessary) of the documentation directory and sets up some information about the code and docs directories.
        /// </summary>
        /// <param name="docsDirectoryPath">The full path to the desired directory to create the documentation in</param>
        /// <param name="codeDirectoryPath">The full path to the root directory of the codebase</param>
        /// <param name="assemblyName">The name of the assembly we are documenting</param>
        public static void SetupDirectories(string docsDirectoryPath, string codeDirectoryPath, string assemblyName)
        {
            CodeDirectory = new DirectoryInfo(codeDirectoryPath);

            if (Directory.Exists(docsDirectoryPath))
            {
                // Log our deletion of the existing directory
                Console.WriteLine("Existing documentation folder found.  Deleting and rebuilding");

                // Deletes the existing documentation folder so we create a fresh one
                Directory.Delete(docsDirectoryPath, true);
            }

            // Log the creation of the new documentation directory
            Console.WriteLine("Creating documentation directory: '" + assemblyName + "'");
            DocsDirectory = Directory.CreateDirectory(docsDirectoryPath);
        }

        #endregion
    }
}
