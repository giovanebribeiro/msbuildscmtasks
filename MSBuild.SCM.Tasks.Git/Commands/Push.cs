﻿using System;
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

            command += branchOrigin + " " + branch;

            if (pushTags)
            {
                command += " --tags";
            }            

            return ClientGit.Instance.ExecCommand(command);
        }
    }
}
