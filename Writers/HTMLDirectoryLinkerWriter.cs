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

        /// <summary>
        /// The relative path of our directory from the root of the code folder
        /// </summary>
        private string RelativePathFromCodeRoot { get; set; }

        #endregion

        /// <summary>
        /// Constructor used for the root directory
        /// </summary>
        public HTMLDirectoryLinkerWriter() :
            base(Path.Combine(DocsDirectoryInfo.FullName, DocsDirectoryInfo.Name + " Linker.html"))
        {
            DirectoryInfo = CodeDirectoryInfo;
        }

        /// <summary>
        /// Constructor used for a sub directory
        /// </summary>
        /// <param name="relativePathFromCodeRoot"></param>
        public HTMLDirectoryLinkerWriter(string relativePathFromCodeRoot) :
            base(Path.Combine(DocsDirectoryInfo.FullName, relativePathFromCodeRoot + "Linker.html"))
        {
            DirectoryInfo = new DirectoryInfo(Path.Combine(CodeDirectoryInfo.FullName, relativePathFromCodeRoot));
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
            WriteLine("<link rel=\"stylesheet\" href=\"" + DocsDirectoryInfo.FullName + "\\Styles\\class.css\">");
            WriteLine("<title>" + DirectoryInfo.Name + "</title>");
            UnIndent();
            WriteLine("</head>");

            WriteLine("<body>");
            Indent();
            WriteLine("<header>");
            WriteLine("<h1 id=\"page_title\">" + DirectoryInfo.Name + " Directory</h1>");
            WriteLine("</header>");

            foreach (FileInfo file in DirectoryInfo.GetFiles("*.cs", SearchOption.TopDirectoryOnly))
            {
                // Write the links to the .html files - can use a relative path since it is in the same folder
                WriteLine("<a href=\"" + file.GetExtensionlessFileName() + ".html\">" + file.Name + "</a>");
            }

            foreach (DirectoryInfo directory in DirectoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                // Write the links to the directory .html files - can use relative paths since it is in a sub folder
                WriteLine("<a href=\"" + Path.Combine(directory.Name, directory.Name + " Linker.html") + "\">" + directory.Name + " Directory" + "</a>");
            }

            WriteLine("</body>");

            WriteLine("</html>");
        }
    }
}
