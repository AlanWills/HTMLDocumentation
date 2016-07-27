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
            ValidDirectories.RemoveAll(x => x.ShouldIgnoreHTMLDirectory());
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

            WriteSideBar();

            WriteLine("<div style=\"margin-left:200px\">");
            Indent();

            WritePageHeader();
            WriteNavBar();

            WriteLine("<h2 id=\"files\">Files</h2>");

            // Write the links to the .html files - can use a relative path since it is in the same folder
            foreach (FileInfo file in ValidHTMLFiles)
            {
                WriteFileLink(file);
            }

            WriteLine("<h2 id=\"directories\">Directories</h2>");

            // Write the links to the directory linker .html files - can use relative paths since it is in a sub folder
            foreach (DirectoryInfo directory in ValidDirectories)
            {
                WriteDirectoryLink(directory);
            }

            UnIndent();
            WriteLine("</div>");
        }

        #endregion

        #region Body Writing Utility Functions

        /// <summary>
        /// Write the sidebar for navigation of sections for this directory linker
        /// </summary>
        private void WriteSideBar()
        {
            // Write links to all the sections in this
            WriteLine("<nav class=\"w3-sidenav w3-white\" style=\"width:200px;\" id=\"pageSideBar\">");
            Indent();

            WriteLine("<h6 class=\"w3-center\">Sections</h6>");
            WriteLine("<a href=\"#files\" class=\"w3-pale-blue w3-border w3-border-blue w3-hover-white w3-margin w3-padding-left\">Files</a>");

            foreach (FileInfo file in ValidHTMLFiles)
            {
                WriteLine("<a class=\"w3-margin-left w3-small\" href=\"#" + file.GetExtensionlessFileName() + "\">" + file.GetExtensionlessFileName() + "</a>");
            }

            WriteLine("<a href=\"#files\" class=\"w3-pale-blue w3-border w3-border-blue w3-hover-white w3-margin w3-padding-left\">Directory</a>");

            foreach (DirectoryInfo directory in ValidDirectories)
            {
                WriteLine("<a class=\"w3-margin-left w3-small\" href=\"#" + directory.Name + "\">" + directory.Name + "</a>");
            }

            UnIndent();
            WriteLine("</nav>");
        }

        /// <summary>
        /// Write the header section of our body which holds the title of the page.
        /// </summary>
        private void WritePageHeader()
        {
            WriteLine("<header class=\"w3-container w3-green w3-center\">");
            WriteLine("<h1 id=\"page_title\">" + DirectoryInfo.Name + " Directory</h1>");
            WriteLine("</header>");
        }

        private void WriteNavBar()
        {
            // Write one up and everything in this directory
        }

        #endregion

        #region Linker Writing Utility Functions

        /// <summary>
        /// A utility function for writing a link to a file in this directory
        /// </summary>
        /// <param name="file"></param>
        private void WriteFileLink(FileInfo file)
        {
            WriteLine("<div class=\"w3-card-4 w3-margin-top w3-margin-bottom w3-pale-blue w3-hover-blue w3-leftbar w3-border-blue\">");
            Indent();
                WriteLine("<a href=\"" + file.Name + "\" id=\"" + file.GetExtensionlessFileName() + "\" class=\"w3-padding-left\">" + file.GetExtensionlessFileName() + "</a>");
            UnIndent();
            WriteLine("</div>");
        }

        /// <summary>
        /// A utility function for writing a link to a directory in this directory
        /// </summary>
        /// <param name="directory"></param>
        private void WriteDirectoryLink(DirectoryInfo directory)
        {
            WriteLine("<div class=\"w3-card-4 w3-margin-top w3-margin-bottom w3-pale-green w3-hover-green w3-leftbar w3-border-green\">");
            Indent();
                WriteLine("<a href=\"" + Path.Combine(directory.Name, directory.Name + LinkerString) + "\" id=\"" + directory.Name + "\" class=\"w3-padding-left\">" + directory.Name + "</a>");
            UnIndent();
            WriteLine("</div>");

            // Show links to items in this directory
        }

        #endregion
    }
}
