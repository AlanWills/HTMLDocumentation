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

        #endregion

        public HTMLWriter(string path) :
            base(path, false)
        {
        }

        #region Virtual Functions

        /// <summary>
        /// Writes the whole html file by calling through to the virtual functions WriteHead, WriteBody etc.
        /// </summary>
        public void Write()
        {
            MarshalData();

            WriteLine("<!DOCTYPE html/>");
            WriteLine("<html>");

            WriteLine("<head>");
            Indent();
            {
                WriteHead();
            }
            UnIndent();
            WriteLine("</head>");

            WriteLine("<body class=\"w3-container w3-margin-0\" id=\"page_body\">");
            Indent();
            {
                WriteBody();
            }
            UnIndent();

            WritePostScripts();

            WriteLine("</body>");
            WriteLine("</html>");
        }

        /// <summary>
        /// Called at the start of the Write function.
        /// Used to populate class data for use in the Write function rather than doing it in the Write function itself.
        /// </summary>
        protected virtual void MarshalData() { }

        /// <summary>
        /// Override this function to specify extra content for the documents header.
        /// Base function specifies the style sheet from w3schools as well as basic layout information.
        /// No need to include <head></head> tags.
        /// </summary>
        protected virtual void WriteHead()
        {
            WriteLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />");
            WriteLine("<link rel=\"stylesheet\" href=\"http://www.w3schools.com/lib/w3.css\" />");
            WriteLine("<link rel=\"stylesheet\" href=\"" + ContentManager.StyleFileLocation + "\" />");
        }

        /// <summary>
        /// Override this function to specify extra content for the documents body.
        /// No need to include <body></body> tags.
        /// </summary>
        protected virtual void WriteBody() { }

        /// <summary>
        /// Called in the head section of the document.
        /// Override to implement the running of javascript before the rest of the document is loaded.
        /// </summary>
        protected virtual void WritePreScripts() { }

        /// <summary>
        /// Called right at the end of the html document.
        /// Override to implement the running of javascript before the rest of the document is loaded.
        /// </summary>
        protected virtual void WritePostScripts() { }

        #endregion

        #region Utility Functions

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

        #endregion
    }
}
