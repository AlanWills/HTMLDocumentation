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

        public const string LinkerString = " Linker.html";

        #endregion

        public HTMLDirectoryLinkerWriter(DirectoryInfo directoryInfo) :
            base(Path.Combine(directoryInfo.FullName, directoryInfo.Name + LinkerString))
        {
            DirectoryInfo = directoryInfo;
        }

        /// <summary>
        /// Creates an HTML page with links to the pages for classes and directories on this level.
        /// </summary>
        public void WriteDirectory()
        {
            WriteLine("<!DOCTYPE html/>");
            WriteLine("<html>");

            WriteLine("<head>");
            Indent();
            WriteLine("<link rel=\"stylesheet\" href=\"" + DocsDirectoryInfo.FullName + "\\Styles\\class.css\">");
            WriteLine("<title>" + DirectoryInfo.Name + "</title>");
            UnIndent();
            WriteLine("</head>");

            WriteLine("<body>");
            Indent();
            WriteLine("<header>");
            WriteLine("<h1 id=\"page_title\">" + DirectoryInfo.Name + " Directory</h1>");
            WriteLine("</header>");

            foreach (FileInfo file in DirectoryInfo.GetFiles("*.html", SearchOption.TopDirectoryOnly))
            {
                // Don't write the linker file
                if (file.Name == DirectoryInfo.Name + LinkerString)
                {
                    continue;
                }

                // Write the links to the .html files - can use a relative path since it is in the same folder
                WriteLine("<a href=\"" + file.GetExtensionlessFileName() + ".html\">" + file.GetExtensionlessFileName() + "</a>");
            }

            foreach (DirectoryInfo directory in DirectoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                // Don't write links to invalid directories
                if (directory.ShouldIgnoreDirectory())
                {
                    continue;
                }

                // Write the links to the directory linker .html files - can use relative paths since it is in a sub folder
                WriteLine("<a href=\"" + Path.Combine(directory.Name, directory.Name + LinkerString) + "\">" + directory.Name + " Directory" + "</a>");
            }

            WriteLine("</body>");

            WriteLine("</html>");
        }
    }
}
