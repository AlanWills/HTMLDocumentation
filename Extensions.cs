using System.IO;

namespace HTMLDocumentation
{
    public static class Extensions
    {
        public static string GetExtensionlessFileName(this FileInfo fileInfo)
        {
            return fileInfo.Name.Replace(fileInfo.Extension, "");
        }
    }
}
