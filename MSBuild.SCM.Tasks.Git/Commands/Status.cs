using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks.Git.Client
{
    public class Status
    {
        public static List<string> ExecCommand()
        {
            return ClientGit.Instance.ExecCommand("status");
        }
    }
}
