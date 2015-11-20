using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks.Git.Commands
{
    public class Commit
    {
        public static List<string> ExecCommand(bool add, string message)
        {
            string command = "commit ";

            if (add)
            {
                command += "-a ";
            }

            if (message != null)
            {
                command += "-m " + message;
            }

            return Client.Instance.ExecCommand(command);
        }
    }
}
