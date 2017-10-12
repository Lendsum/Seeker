using System;
using System.Globalization;
using System.IO;

namespace Lendsum.Crosscutting.Common
{
    /// <summary>
    /// Helper to build paths.
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// Gets the full path when we want to divided files in folders per day. It will also creates de directory
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="rootPath">The root path.</param>
        /// <returns></returns>
        public static string GetFullPathForDayClassification(DateTime currentDate, string fileName, string rootPath)
        {
            Check.NotNullOrEmpty(() => fileName);
            Check.NotNullOrEmpty(() => rootPath);

            CreateAllDirectories(rootPath);

            string year = currentDate.Year.ToString("D4", CultureInfo.InvariantCulture);
            string month = currentDate.Month.ToString("D2", CultureInfo.InvariantCulture);
            string day = currentDate.Day.ToString("D2", CultureInfo.InvariantCulture);

            var path = Path.Combine(rootPath, year, month, day);
            CreateAllDirectories(path);

            return Path.Combine(path, fileName);
        }

        /// <summary>
        /// Creates all directories to form the root path.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        public static void CreateAllDirectories(string rootPath)
        {
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
                //Stack<string> subDirs = new Stack<string>();
                //DirectoryInfo info = new DirectoryInfo(rootPath);

                //bool run = true;
                //while (run)
                //{
                //    if (info.Parent == null || string.IsNullOrWhiteSpace(info.Parent.Name) || Directory.Exists(info.FullName))
                //    {
                //        run = false;
                //    }
                //    else
                //    {
                //        subDirs.Push(info.Name);
                //        info = info.Parent;
                //    }
                //}

                //while (subDirs.Any())
                //{
                //    info = new DirectoryInfo(Path.Combine(info.FullName, subDirs.Pop()));
                //    if (!Directory.Exists(info.FullName))
                //    {
                //        Directory.CreateDirectory(info.FullName);
                //    }
                //}
            }
        }
    }
}