using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks.Git.Client
{
    public class Push
    {
        public static List<string> ExecCommand(string branchOrigin, string branch, bool pushTags)
        {
            string command = "push ";

            if (branchOrigin == null)
            {
                branchOrigin = "origin";
            }

            if(branch == null)
            {
                branch = "master";
            }

            if (pushTags)
            {
                command += branchOrigin+" --tags ";
            }
            else
            {
                command += branchOrigin + " " + branch;
            }

            Console.WriteLine(command);

            return ClientGit.Instance.ExecCommand(command);
        }
    }
}
