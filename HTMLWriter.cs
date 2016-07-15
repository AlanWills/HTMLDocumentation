using System;
using System.IO;
using System.Reflection;

namespace HTMLDocumentation
{
    public class HTMLWriter : StreamWriter
    {
        #region Properties and Fields

        private Type Type { get; set; }

        private string Indentation { get; set; }

        #endregion

        public HTMLWriter(string docsDirectory, Type type) :
            base(docsDirectory + type.Name + ".html", false)
        {
            Type = type;
        }

        public void WriteType(Type type)
        {
            WriteLine("<!DOCTYPE html/>");
            WriteLine("<html>");

            WriteLine("<head>");
            Indent();
                WriteLine("<title>" + type.Name + "</title>");
            UnIndent();
            WriteLine("</head>");

            WriteLine("<body>");
            Indent();
                WriteLine("<header>");
                WriteLine("<h1>" + type.Name + "</h1>");
                WriteLine("</header>");

            foreach (FieldInfo property in type.GetFields())
            {
                if (type.Name == property.DeclaringType.Name)
                {
                    WriteLine("<h4>" + property.Name + "</h4>");
                }
            }

            /*
             * Have headers + bookmarks for each section (Instance and static, then subsections of public & non-public)
             * Going to need to write instance & static of public and non public methods declared in this class.
             * Also indicate if they are overriding virtual functions (if they are only)
             * Don't write property setters and getters - do that in properties - remove the setters and getters when we iterate over properties?
             * Parameters, return types, template arguments etc.
             */

            foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
            {
                if (type.Name == method.DeclaringType.Name && 
                   !method.Name.StartsWith("get_") &&
                   !method.Name.StartsWith("set_"))
                {
                    WriteLine("<h4>" + method.Name + "</h4>");
                }
            }

            foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                if (type.Name == method.DeclaringType.Name &&
                   !method.Name.StartsWith("get_") &&
                   !method.Name.StartsWith("set_"))
                {
                    WriteLine("<h4>" + method.Name + "</h4>");
                }
            }

            UnIndent();
            WriteLine("</body>");

            WriteLine("</html>");
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
