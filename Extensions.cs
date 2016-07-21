using System.IO;

namespace HTMLDocumentation
{
    public static class Extensions
    {
        public static string GetExtensionlessFileName(this FileInfo fileInfo)
        {
            return fileInfo.Name.Replace(fileInfo.Extension, "");
        }

        /// <summary>
        /// Returns true if this directory is one of: bin, obj, Properties, is hidden or has no .html files inside it
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <returns></returns>
        public static bool ShouldIgnoreDirectory(this DirectoryInfo directoryInfo)
        {
            return directoryInfo.Name == "bin" ||
                   directoryInfo.Name == "obj" ||
                   directoryInfo.Name == "Properties" ||
                   (directoryInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden ||
                   directoryInfo.GetFiles("*.html", SearchOption.TopDirectoryOnly).Length == 0;
        }
    }
}
