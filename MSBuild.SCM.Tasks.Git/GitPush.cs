using MSBuild.SCM.Tasks.Git.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks.Git
{
    public class GitPush : Microsoft.Build.Utilities.Task
    {
        public string RemoteName { get; set; }
        public string BranchName { get; set; }
        public bool Tags { get; set; }

        public override bool Execute()
        {
            List<string> output = Push.ExecCommand(RemoteName, BranchName, Tags);
            foreach(string line in output)
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
