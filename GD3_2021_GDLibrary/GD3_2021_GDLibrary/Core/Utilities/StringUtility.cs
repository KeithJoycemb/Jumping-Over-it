using System.Text.RegularExpressions;

namespace GD.Utilities
{
    /// <summary>
    /// Provide useful string related functions (e.g. parse, regex)
    /// </summary>
    public class FileUtility

    {
        /// <summary>
        /// Parse a file name from a path + name string
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ParseName(string fullPath)
        {
            return Regex.Match(fullPath, @"[^\\/]*$").Value;
        }

        /// <summary>
        /// Parse a file name from a path + name string
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ParsePath(string fullPath)
        {
            return Regex.Match(fullPath, @"^(.*) / ([^/] *)$").Groups[0];
        }
    }
}