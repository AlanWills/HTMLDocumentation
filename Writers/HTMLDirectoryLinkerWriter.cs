using System.IO;

namespace HTMLDocumentation
{
    /// <summary>
    /// An HTMLWriter which creates a page linking all files and directories in IT'S level only.
    /// Also recursively does the same thing for any directory in it's level.
    /// </summary>
    public class HTMLDirectoryLinkerWriter : HTMLWriter
    {
        #region Properties and Fields

        /// <summary>
        /// The directory info for the directory we are linking
        /// </summary>
        private DirectoryInfo DirectoryInfo { get; set; }

        #endregion

        public HTMLDirectoryLinkerWriter(DirectoryInfo directoryInfo) :
            base(Path.Combine(DocsDirectory, directoryInfo.Name + "Linker.html"))
        {
            DirectoryInfo = directoryInfo;
        }

        /// <summary>
        /// Creates an HTML page with links to any page for a class or link page for a directory in this level.
        /// Recursively calls to sub directories too.
        /// </summary>
        public void WriteDirectory()
        {
            WriteLine("<!DOCTYPE html/>");
            WriteLine("<html>");

            WriteLine("<head>");
            Indent();
            WriteLine("<link rel=\"stylesheet\" href=\"" + DocsDirectory + "\\Styles\\class.css\">");
            WriteLine("<title>" + DirectoryInfo.Name + "</title>");
            UnIndent();
            WriteLine("</head>");

            WriteLine("<body>");
            Indent();
            WriteLine("<header>");
            WriteLine("<h1 id=\"page_title\">" + DirectoryInfo.Name + " Directory</h1>");
            WriteLine("</header>");

            // THESE LINKS ARE WRONG - THEY ARE POINTING TO THE .CS DIRECTORY STRUCTURE WHEN REALLY THEY SHOULD BE POINTING TO THE DOCUMENTATION STRUCTURE

            foreach (FileInfo file in DirectoryInfo.GetFiles("*.cs", SearchOption.TopDirectoryOnly))
            {
                // Write the links to the .html files
                WriteLine("<a href=\"" + file.FullName + ".html\"/>");
            }

            foreach (DirectoryInfo directory in DirectoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                // Write the links to the directory .html files
                WriteLine("<a href=\"" + Path.Combine(directory.FullName, directory.Name + "Linker.html") + "\"/>");

                // Create linker pages for any sub directories on this level
                using (HTMLDirectoryLinkerWriter directoryLinker = new HTMLDirectoryLinkerWriter(directory))
                {
                    directoryLinker.WriteDirectory();
                }
            }

            WriteLine("</body>");

            WriteLine("</html>");
        }
    }
}
