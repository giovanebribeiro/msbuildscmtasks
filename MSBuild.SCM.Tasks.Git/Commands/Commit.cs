﻿using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks.Git.Client
{
    public class Commit
    {
        public static List<string> ExecCommand(bool add, string message, string[] fileList)
        {
            string command = "commit ";

            if (add)
            {
                command += "-a ";
            }

            if (message == null)
            {
                message = "Adding new files to repo";
            }

            command += "-m \"" + message + "\" ";

            // putting the file list on command
            if (fileList!=null && fileList.Length > 0)
            {
                foreach (string file in fileList)
                {
                    command += file + " ";
                }
            }


            return ClientGit.Instance.ExecCommand(command);
        }
    }
}
