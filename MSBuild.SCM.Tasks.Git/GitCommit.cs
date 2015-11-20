﻿using MSBuild.SCM.Tasks.Git.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks.Git
{
    public class GitCommit:Microsoft.Build.Utilities.Task
    {
        public bool Add { get; set; }
        public string Message { get; set; }

        public override bool Execute()
        {
            List<string> output = Commit.ExecCommand(Add, Message);
            foreach (string line in output)
            {
                Log.LogMessage(line);
            }
            return true;
        }
    }
}
