using System.IO;

namespace HTMLDocumentation
{
    /// <summary>
    /// Utility wrapper around a standard stream writer for providing useful functionality when writing HTML
    /// </summary>
    public class HTMLWriter : StreamWriter
    {
        #region Properties and Fields

        private string Indentation { get; set; }

        /// <summary>
        /// The root directory for the directory tree our documentation will be created in.
        /// </summary>
        public static string DocsDirectory { get; set; }

        #endregion

        public HTMLWriter(string path) :
            base(path, false)
        {

        }

        /// <summary>
        /// Overwrite the writeline function with our own implementation that uses the indentation
        /// </summary>
        /// <param name="line"></param>
        private new void WriteLine(string line)
        {
            base.WriteLine(Indentation + line);
        }

        protected void Indent()
        {
            Indentation += "\t";
        }

        protected void UnIndent()
        {
            Indentation = Indentation.Remove(Indentation.Length - 1);
        }
    }
}
