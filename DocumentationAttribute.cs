using System;

namespace HTMLDocumentation
{
    /// <summary>
    /// Stores a string which describes the object this attribute is attached to
    /// </summary>
    public class Documentation : Attribute
    {
        #region Properties and Fields

        /// <summary>
        /// The string containing the description for our documentation.
        /// </summary>
        public string DocString { get; private set; }

        #endregion

        public Documentation(string docString)
        {
            DocString = docString;
        }
    }
}
