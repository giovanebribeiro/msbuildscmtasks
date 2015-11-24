using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks.Git.Commands
{
    public class Commit
    {
        public static List<string> ExecCommand(bool add, string message, ITaskItem[] fileList)
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

            command += "-m " + message + " ";

            // putting the file list on command
            if (fileList!=null && fileList.Length > 0)
            {
                foreach (ITaskItem file in fileList)
                {
                    command += file.ItemSpec + " ";
                }
            }


            return Client.Instance.ExecCommand(command);
        }
    }
}
