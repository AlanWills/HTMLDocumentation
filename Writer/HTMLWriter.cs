﻿using System;
using System.IO;
using System.Reflection;
using System.Xml.XPath;

namespace HTMLDocumentation
{
    public class HTMLWriter : StreamWriter
    {
        #region Properties and Fields

        private Type Type { get; set; }

        private string Indentation { get; set; }

        #endregion

        private string DocsDirectory;

        public HTMLWriter(string docsDirectory, Type type) :
            base(Path.Combine(docsDirectory, type.Name + ".html"), false)
        {
            Type = type;
            DocsDirectory = docsDirectory;
        }

        public void WriteType()
        {
            WriteLine("<!DOCTYPE html/>");
            WriteLine("<html>");

            WriteLine("<head>");
            Indent();
                WriteLine("<link rel=\"stylesheet\" href=\"" + DocsDirectory + "\\Styles\\class.css\">");
                WriteLine("<title>" + Type.Name + "</title>");
            UnIndent();
            WriteLine("</head>");

            WriteLine("<body>");
            Indent();
                WriteLine("<header>");
                WriteLine("<h1 id=\"page_title\">" + Type.Name + " Class</h1>");
                WriteLine("</header>");

            //foreach (FieldInfo property in type.GetFields())
            //{
            //    if (type.Name == property.DeclaringType.Name)
            //    {
            //        WriteLine("<h4>" + property.Name + "</h4>");
            //    }
            //}

            /*
             * Have headers + bookmarks for each section (Instance and static, then subsections of public & non-public)
             * Going to need to write instance & static of public and non public methods declared in this class.
             * Also indicate if they are overriding virtual functions (if they are only)
             * Don't write property setters and getters - do that in properties - remove the setters and getters when we iterate over properties?
             * Parameters, return types, template arguments etc.
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

            UnIndent();
            WriteLine("</body>");

            WriteLine("</html>");
        }

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

            // Construct the html for the return type
            string returnTypeString = "<span title=\"Return type\" class=\"return_type\">" + method.ReturnParameter.ParameterType.Name + "</span>";

            // Construct the html for the method name
            string methodString = "<h4>" + returnTypeString + " <span title=\"Method name\">" + method.Name + "</span>" + parametersString;
            if (method.IsVirtual)
            {
                methodString += " <span class=\"virtual\" title=\"Method is virtual or overrides a virtual function\">(Virtual)</span>";
            }

            WriteLine(methodString + "</h4>");

            XPathNavigator methodNode = GetXMLDocNodeForMethod(method);
            if (methodNode != null)
            {
                // Construct the html for the comment on the method
                XPathNavigator clone;
                {
                    clone = methodNode.Clone();
                    clone.MoveToChild("summary", "");

                    WriteLine("<p>" + clone.InnerXml + "</p>");
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
        }

        /// <summary>
        /// Uses the input method to find the appropriate node in the documentation XML file and
        /// returns the XPathNavigator at that position.
        /// </summary>
        /// <param name="method"></param>
        private XPathNavigator GetXMLDocNodeForMethod(MethodInfo method)
        {
            // Ultimately I think we want to compile the docs ourselves for each class - do this with Process and then see the docs on msdn for compiling docs
            string xmlDocs = Directory.GetCurrentDirectory() + "\\" + Assembly.GetExecutingAssembly().GetName().Name + ".xml";
            XPathDocument documentationXML = new XPathDocument(xmlDocs);
            XPathNavigator nav = documentationXML.CreateNavigator();

            nav.MoveToRoot();
            nav.MoveToFirstChild();
            nav.MoveToChild("members", "");

            string containsString = "contains(@name, '" + method.Name + "')";
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

        /// <summary>
        /// Overwrite the writeline function with our own implementation that uses the indentation
        /// </summary>
        /// <param name="line"></param>
        private new void WriteLine(string line)
        {
            base.WriteLine(Indentation + line);
        }
        
        private void Indent()
        {
            Indentation += "\t";
        }

        private void UnIndent()
        {
            Indentation = Indentation.Remove(Indentation.Length - 1);
        }

        public override void Flush()
        {
            base.Flush();
        }
    }
}