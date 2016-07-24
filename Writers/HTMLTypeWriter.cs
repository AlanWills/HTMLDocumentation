﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.XPath;

namespace HTMLDocumentation
{
    /// <summary>
    /// An HTMLWriter which specifically writes a class
    /// </summary>
    public class HTMLTypeWriter : HTMLWriter
    {
        #region Properties and Fields

        /// <summary>
        /// The current type that we are writing out to a documentation page.
        /// </summary>
        private Type Type { get; set; }

        /// <summary>
        /// The documentation XML generated by visual studio
        /// </summary>
        private XPathDocument Documentation { get; set; }

        #endregion

        public HTMLTypeWriter(Type type, string documentationFilePath) :
            base(documentationFilePath)
        {
            Type = type;
        }

        #region Virtual Functions

        protected override void MarshalData()
        {
            base.MarshalData();

            // Ultimately I think we want to compile the docs ourselves for each class - do this with Process and then see the docs on msdn for compiling docs
            string xmlDocs = Directory.GetCurrentDirectory() + "\\" + Assembly.GetExecutingAssembly().GetName().Name + ".xml";
            Documentation = new XPathDocument(xmlDocs);
        }

        /// <summary>
        /// Set up the style sheet and title for the Class documentation page.
        /// </summary>
        protected override void WriteHead()
        {
            base.WriteHead();

            WriteLine("<title>" + Type.Name + "</title>");
        }

        /// <summary>
        /// Write each of the different type of methods in an accordion structure
        /// </summary>
        protected override void WriteBody()
        {
            base.WriteBody();

            // Sidebar
            WriteLine("<nav class=\"w3-sidenav w3-collapse w3-white w3-card-2 w3-animate-left\" style=\"width:200px;\" id=\"pageSideBar\">");
            Indent();

            WriteLine("<a href=\"#page_body\">Page Top</a>");
            WriteLine("<a href=\"#public_methods\">Public Methods</a>");
            WriteLine("<a href=\"#non_public_methods\">Non Public Methods</a>");

            UnIndent();
            WriteLine("</nav>");

            WriteLine("<div class=\"w3-main\" style=\"margin-left:205px\">");
            WriteLine("<span class=\"w3-opennav w3-hide-large\" onclick=\"w3_open()\">&#9776;</span>");

            WriteLine("<header class=\"w3-container w3-blue w3-center\">");
            WriteLine("<h1 id=\"page_title\">" + Type.Name + " Class</h1>");
            WriteLine("</header>");

            // Create a navbar for files in this directory
            WriteLine("<ul class=\"w3-navbar w3-border w3-light-grey\">");
            Indent();

            // Write a link back to the linker for the directory this type's .cs file is in
            FileInfo[] info = CodeDirectoryInfo.GetFiles(Type.Name + ".cs", SearchOption.AllDirectories);
            Debug.Assert(info.Length == 1);

            // Write the main link back to the directory linker
            FileInfo thisTypeFileInfo = info[0];
            WriteLine("<li><a class=\"w3-green\" href=\"" + thisTypeFileInfo.Directory.Name + HTMLDirectoryLinkerWriter.LinkerString + "\">" + thisTypeFileInfo.Directory.Name + "</a></li>");

            // Write a link to the other files in the directory
            // We cannot use the actual html files for this as they may not have been created yet
            // This also means that we will not write linker files by mistake
            List<FileInfo> filesInDir = thisTypeFileInfo.Directory.GetFiles("*.cs", SearchOption.TopDirectoryOnly).ToList();
            filesInDir.Remove(thisTypeFileInfo);

            foreach (FileInfo file in filesInDir)
            {
                WriteLine("<li><a href=\"" + file.GetExtensionlessFileName() + ".html\">" + file.GetExtensionlessFileName() + "</a></li>");
            }

            UnIndent();
            WriteLine("</ul>");

            //            < nav class="w3-sidenav w3-collapse w3-white w3-card-2 w3-animate-left" style="width:200px;" id="mySidenav">
            //  <a href = "javascript:void(0)" onclick="w3_close()"
            //  class="w3-closenav w3-large w3-hide-large">Close &times;</a>
            //  <a href = "#" > Link 1</a>
            //  <a href = "#" > Link 2</a>
            //  <a href = "#" > Link 3</a>
            //  <a href = "#" > Link 4</a>
            //  <a href = "#" > Link 5</a>
            //</nav>

             /*
             * Have headers + bookmarks for each section (Instance and static, then subsections of public & non-public)
             * Don't write property setters and getters - do that in properties - remove the setters and getters when we iterate over properties?
             * Template arguments etc.
             * Document properties, fields, events
             */

            // Methods
            {
                WriteLine("<h2 id=\"public_methods\">Public Methods</h2>");

                Indent();
                // Write public instance methods declared by this type
                WriteMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                UnIndent();

                WriteLine("<h2 id=\"non_public_methods\">Non Public Methods</h2>");

                Indent();
                // Write non public instance methods declared by this type
                WriteMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                UnIndent();
            }

            WriteLine("</div>");
        }

        protected override void WritePostScripts()
        {
            base.WritePostScripts();

            WriteLine("<script href=\"" + Path.Combine(DocsDirectoryInfo.FullName, "nav_bar.js") + "\" />");
        }

        #endregion

        #region Utility Functions

        /// <summary>
        /// Utility function for determining whether a method should be written to this type's HTML page.
        /// Returns true if the method was declared inside the type we are writing and is not a getter or setter for a property.
        /// </summary>
        /// <param name="method">The method we should check to write</param>
        /// <returns>True if we should write the method and false if we should not</returns>
        private bool ShouldWriteMethod(MethodInfo method)
        {
            return Type.Name == method.DeclaringType.Name &&
                   !method.Name.StartsWith("get_") &&
                   !method.Name.StartsWith("set_");
        }

        /// <summary>
        /// Iterates over all the methods of the current type and attempts to right any methods which satisfy the input
        /// flags and also the ShouldWriteMethod.
        /// </summary>
        /// <param name="filterFlags"></param>
        private void WriteMethods(BindingFlags filterFlags)
        {
            foreach (MethodInfo method in Type.GetMethods(filterFlags))
            {
                // Only write methods that are declared in this class and are not getters and setters for Properties
                if (ShouldWriteMethod(method))
                {
                    WriteMethod(method);
                }
            }
        }

        /// <summary>
        /// Writes all the information about a method, including:
        /// Parameters
        /// Return type
        /// Template arguments
        /// Whether it is virtual
        /// </summary>
        private void WriteMethod(MethodInfo method)
        {
            ParameterInfo[] parameters = method.GetParameters();

            // Construct the html for the parameters
            string parametersString = "(";
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];
                parametersString += "<span title=\"Parameter type\" class=\"parameter_type\">" + 
                    parameter.ParameterType.Name + "</span> <span title=\"Parameter name\">" + parameter.Name + "</span>";

                // Add the delimiter if we have arguments left
                if (i < parameters.Length - 1)
                {
                    parametersString += ", ";
                }
            }
            parametersString += ")";

            WriteLine("<div class=\"w3-card-4 w3-margin-top w3-margin-bottom\">");
            Indent();

            // Construct the html for the return type
            //string returnTypeString = "<span title=\"Return type\" class=\"return_type\">" + method.ReturnParameter.ParameterType.Name + "</span> ";

            //// Construct the html for the method name
            //string methodString = "<button type=\"button\" class=\"w3-btn-block w3-left-align\" onclick=\"expand('" + method.Name + "')\">" + returnTypeString + method.Name;
            //if (method.IsVirtual)
            //{
            //    methodString += " <span class=\"virtual\" title=\"Method is virtual or overrides a virtual function\">(Virtual)</span>";
            //}
            //methodString += "</button>";

            WriteLine("<header class=\"w3-container w3-pale-blue w3-leftbar w3-border-blue w3-hover-shadow\"");
            Indent();
                WriteLine("<h1>" + method.Name + "</h1>");
            UnIndent();
            WriteLine("</header>");

            WriteLine("<div class=\"w3-container\">");
            Indent();

            XPathNavigator methodNode = GetXMLDocNodeForMethod(method);
            if (methodNode != null)
            {
                // Construct the html for the comment on the method
                XPathNavigator clone;
                {
                    clone = methodNode.Clone();
                    clone.MoveToChild("summary", "");

                    WriteLine("<p>" + clone.InnerXml.Trim(' ', '\r', '\n') + "</p>");
                }

                // Construct the html for the comments on the parameters
                {
                    clone = methodNode.Clone();
                    XPathNodeIterator childParams = clone.SelectChildren("param", "");

                    while (childParams.MoveNext())
                    {
                        WriteLine("<p>" + childParams.Current.GetAttribute("name", "") + " - " + childParams.Current.InnerXml + "</p>");
                    }
                }

                // Construct the html for the comment on the return type (if it exists)
                {
                    clone = methodNode.Clone();
                    if (clone.MoveToChild("returns", ""))
                    {
                        WriteLine("<p>returns - " + clone.InnerXml + "</p>");
                    }
                }
            }

            UnIndent();
            WriteLine("</div>");

            UnIndent();
            WriteLine("</div>");
        }

        /// <summary>
        /// Uses the input method to find the appropriate node in the documentation XML file and
        /// returns the XPathNavigator at that position.
        /// </summary>
        /// <param name="method"></param>
        private XPathNavigator GetXMLDocNodeForMethod(MethodInfo method)
        {
            XPathNavigator nav = Documentation.CreateNavigator();

            nav.MoveToRoot();
            nav.MoveToFirstChild();
            nav.MoveToChild("members", "");

            string containsString = "contains (@name, '" + Type.Name + "') and contains(@name, '" + method.Name + "')";
            int index = 0;
            foreach (ParameterInfo parameter in method.GetParameters())
            {
                if (index < method.GetParameters().Length)
                {
                    containsString += " and ";
                }

                containsString += "contains(@name, '" + parameter.ParameterType.Name + "')";
                index++;
            }

            string xPath = "//member[" + containsString + "]";
            nav = nav.SelectSingleNode(xPath);

            return nav;
        }

        #endregion
    }
}
