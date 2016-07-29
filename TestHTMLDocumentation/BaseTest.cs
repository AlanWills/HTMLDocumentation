using HTMLDocumentation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace TestHTMLDocumentation
{
    /// <summary>
    /// The base test for our unit tests.
    /// We require this class to set up the initial information about the code and docs directory.
    /// </summary>
    public class BaseTest
    {
        // Only want to do setup once per test run
        private static bool hasBeenSetup = false;

        [TestInitialize]
        public void SetupInformation()
        {
            string codeDirectory = Path.Combine(Directory.GetCurrentDirectory(), "DummyLibrary");

            if (!hasBeenSetup)
            {
                ContentManager.SetupDirectories(codeDirectory, Path.Combine(codeDirectory, "DummyLibrary"), "DummyLibrary");
                hasBeenSetup = true;
            }
        }
    }
}
