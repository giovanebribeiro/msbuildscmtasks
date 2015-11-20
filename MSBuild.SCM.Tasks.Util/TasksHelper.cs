using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.SCM.Tasks
{
    public class TasksHelper
    {

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
