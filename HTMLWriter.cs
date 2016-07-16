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

        public void WriteType()
        {
            WriteLine("<!DOCTYPE html/>");
            WriteLine("<html>");

            WriteLine("<head>");
            Indent();
                WriteLine("<title>" + Type.Name + "</title>");
            UnIndent();
            WriteLine("</head>");

            WriteLine("<body>");
            Indent();
                WriteLine("<header>");
                WriteLine("<h1>" + Type.Name + "</h1>");
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
             */
            
            // Methods
            {
                // Write public instance methods declared by this type
                WriteMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

                // Write non public instance methods declared by this type
                WriteMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            }

            UnIndent();
            WriteLine("</body>");

            WriteLine("</html>");
        }

        /// <summary>
        /// Utility function for determining whether a method should be written to this type's HTML page.
        /// Returns true if the method was declared inside the type we are writing and is not a getter or setter for a property.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
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
            WriteLine("<h4>" + method.Name + "</h4>");
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
