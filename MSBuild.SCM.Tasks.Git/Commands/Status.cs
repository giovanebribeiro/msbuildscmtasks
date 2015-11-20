using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks.Git.Commands
{
    public class Status
    {
        public static List<string> ExecCommand()
        {
            return Client.Instance.ExecCommand("status");
        }
    }
}
