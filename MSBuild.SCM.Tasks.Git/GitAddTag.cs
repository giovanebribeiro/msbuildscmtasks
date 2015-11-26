﻿using Microsoft.Build.Framework;
using MSBuild.SCM.Tasks.Git.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks.Git
{
    public class GitAddTag : Microsoft.Build.Utilities.Task
    {
        [Required]
        public string Version { get; set; }
        public string TagPattern { get; set; }
        public string TagMessage { get; set; }
        public override bool Execute()
        {
            List<string> output = AddTag.ExecCommand(TagPattern, Version, TagMessage);

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
