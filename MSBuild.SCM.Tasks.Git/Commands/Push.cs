﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks.Git.Commands
{
    public class Push
    {
        public static List<string> ExecCommand(string branchOrigin, string branch)
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
            return Client.Instance.ExecCommand(command);
        }
    }
}
