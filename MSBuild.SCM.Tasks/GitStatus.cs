using MSBuild.SCM.Tasks.Git.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks
{
    public class GitStatus:Microsoft.Build.Utilities.Task
    {
        public override bool Execute()
        {
            List<string> output = Status.ExecCommand();
            foreach (string line in output)
            {
                if (line != null)
                {
                    Log.LogMessage(line);
                }
            }

            return true;
        }
    }
}
