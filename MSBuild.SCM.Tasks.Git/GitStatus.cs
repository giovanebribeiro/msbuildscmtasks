using MSBuild.SCM.Tasks.Git.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks.Git
{
    public class GitStatus:Microsoft.Build.Utilities.Task
    {
        public override bool Execute()
        {
            List<string> output = Status.ExecCommand();
            foreach (string line in output)
            {
                Log.LogMessage(line);
            }

            return true;
        }
    }
}
