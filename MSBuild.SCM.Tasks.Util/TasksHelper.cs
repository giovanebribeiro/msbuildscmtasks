using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Threading;

namespace MSBuild.SCM.Tasks
{
    public class TasksHelper
    {
        protected static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public static void DeleteDirectory(string path)
        {
            DeleteDirectory(path, false);
        }

        public static void DeleteDirectory(string path, bool recursive)
        {
            // Delete all files and sub-folders?
            if (recursive)
            {
                // Yep... Let's do this
                var subfolders = Directory.GetDirectories(path);
                foreach (var s in subfolders)
                {
                    DeleteDirectory(s, recursive);
                }
            }

            // Get all files of the folder
            var files = Directory.GetFiles(path);
            foreach (var f in files)
            {
                // Get the attributes of the file
                var attr = File.GetAttributes(f);

                // Is this file marked as 'read-only'?
                if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    // Yes... Remove the 'read-only' attribute, then
                    File.SetAttributes(f, attr ^ FileAttributes.ReadOnly);
                }

                // Delete the file, when no process is using it.
                FileInfo fi = new FileInfo(f);
                while (TasksHelper.IsFileLocked(fi))
                {
                    Thread.Sleep(1000);
                }
                File.Delete(f);                
            }

            // When we get here, all the files of the folder were
            // already deleted, so we just delete the empty folder
            try {
                Directory.Delete(path);
            }catch(IOException e)
            {
                Console.WriteLine("error message:" + e.ToString());
            }
        }

        public static string GetProductVersion(string assemblyInfoPath)
        {
            string versionNumber = "";
            if (File.Exists(assemblyInfoPath))
            {
                string[] readText = File.ReadAllLines(assemblyInfoPath);
                var versionInfoLines = readText.Where(t => t.StartsWith("[assembly: AssemblyVersion"));
                foreach (string item in versionInfoLines)
                {
                    versionNumber = item.Substring(item.IndexOf('(') + 2, item.LastIndexOf(')') - item.IndexOf('(') - 3);
                    break; //only the first line matters
                }

            }

            return versionNumber;
        }
    }
}
