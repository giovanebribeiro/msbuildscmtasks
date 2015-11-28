using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks.Git.Client
{
    public class AddTag
    {
        public static List<string> ExecCommand(string tagPattern, string version, string tagMessage)
        {   
            if (version == null)
            {
                throw new InvalidOperationException("Version can't be null");
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
