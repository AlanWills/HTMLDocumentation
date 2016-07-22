﻿using System.IO;

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
        public static DirectoryInfo DocsDirectoryInfo { get; set; }

        /// <summary>
        /// The root directory for the directory tree of the .cs files in the assembly
        /// </summary>
        public static DirectoryInfo CodeDirectoryInfo { get; set; }

        #endregion

        public HTMLWriter(string path) :
            base(path, false)
        {

        }

        #region Virtual Functions

        /// <summary>
        /// WRites the whole html file by calling through to the virtual functions WriteHead, WriteBody etc.
        /// </summary>
        public virtual void Write()
        {
            
        }

        /// <summary>
        /// Override this function to specify extra content for the documents header.
        /// No need to include <head></head> tags.
        /// </summary>
        protected virtual void WriterHead()
        {
            
        }

        /// <summary>
        /// Override this function to specify extra content for the documents body.
        /// No need to include <body></body> tags.
        /// </summary>
        protected virtual void WriteBody()
        {
            
        }

        /// <summary>
        /// Called in the head section of the document.
        /// Override to implement the running of javascript before the rest of the document is loaded.
        /// </summary>
        protected virtual void WritePreScripts()
        {
            
        }

        /// <summary>
        /// Called right at the end of the html document.
        /// Override to implement the running of javascript before the rest of the document is loaded.
        /// </summary>
        protected virtual void WritePostScripts()
        {
            
        }

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
