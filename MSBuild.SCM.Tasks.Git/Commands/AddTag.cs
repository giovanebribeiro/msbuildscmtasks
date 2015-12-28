using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks.Git.Client
{
    public class AddTag
    {
        public static List<string> ExecCommand(string tagPattern, string version, string tagMessage, string AssemblyInfoPath)
        {
            if (version == null)
            {
                if (AssemblyInfoPath == null)
                {
                    throw new InvalidOperationException("If version is null, the AssemblyInfoPath must be set.");
                }

                using (var reader = new StreamReader(AssemblyInfoPath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("[assembly: AssemblyVersion"))
                        {
                            version = TasksHelper.GetVersionNumber(line);
                            break;
                        }
                    }

                    reader.Close();
                }
            }

            if (tagPattern == null)
            {
                tagPattern = "v%VERSION%";
            }

            if(tagMessage == null)
            {
                tagMessage = "Release version %VERSION%";
            }

            tagPattern = Regex.Replace(tagPattern, "%VERSION%", version);
            tagMessage = Regex.Replace(tagMessage, "%VERSION%", version);

            string command = "tag -a " + tagPattern + " -m \"" + tagMessage + "\"";
            List<string> output = ClientGit.Instance.ExecCommand(command);
            return output;
        }

    }
}
