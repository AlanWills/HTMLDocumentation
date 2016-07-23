using System.Collections.Generic;
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
        /// The valid html files in this directory that we wish to create links to
        /// </summary>
        private List<FileInfo> ValidHTMLFiles { get; set; }

        /// <summary>
        /// The valid directories in this directory that we wish to create links to (create link to their linker page)
        /// </summary>
        private List<DirectoryInfo> ValidDirectories { get; set; }

        public const string LinkerString = " Linker.html";

        #endregion

        public HTMLDirectoryLinkerWriter(DirectoryInfo directoryInfo) :
            base(Path.Combine(directoryInfo.FullName, directoryInfo.Name + LinkerString))
        {
            DirectoryInfo = directoryInfo;
        }

        #region Virtual Functions

        protected override void MarshalData()
        {
            base.MarshalData();

            // Get all files except the linker file
            ValidHTMLFiles = new List<FileInfo>(DirectoryInfo.GetFiles("*.html", SearchOption.TopDirectoryOnly));
            ValidHTMLFiles.RemoveAll(x => x.Name == DirectoryInfo.Name + LinkerString);

            // Ignore invalid directories
            ValidDirectories = new List<DirectoryInfo>(DirectoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly));
            ValidDirectories.RemoveAll(x => x.ShouldIgnoreDirectory());
        }

        /// <summary>
        /// Sets up the style sheets and title for the directory linker page.
        /// </summary>
        protected override void WriteHead()
        {
            base.WriteHead();

            WriteLine("<title>" + DirectoryInfo.Name + "</title>");
        }

        /// <summary>
        /// Writes links to each page in this directory, be it a Class or Directory page.
        /// </summary>
        protected override void WriteBody()
        {
            base.WriteBody();

            WriteLine("<header>");
            WriteLine("<h1 id=\"page_title\">" + DirectoryInfo.Name + " Directory</h1>");
            WriteLine("</header>");

            foreach (FileInfo file in ValidHTMLFiles)
            {
                // Write the links to the .html files - can use a relative path since it is in the same folder
                WriteLine("<a href=\"" + file.GetExtensionlessFileName() + ".html\">" + file.GetExtensionlessFileName() + "</a>");
                WriteLine("<br/>");
            }

            foreach (DirectoryInfo directory in ValidDirectories)
            {
                // Write the links to the directory linker .html files - can use relative paths since it is in a sub folder
                WriteLine("<a href=\"" + Path.Combine(directory.Name, directory.Name + LinkerString) + "\">" + directory.Name + " Directory" + "</a>");
                WriteLine("<br/>");
            }
        }

        #endregion
    }
}
