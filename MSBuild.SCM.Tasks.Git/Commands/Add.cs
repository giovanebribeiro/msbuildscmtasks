using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks.Git.Commands
{
    public class Add
    {
        public static List<string> ExecCommand(bool all, string[] files)
        {
            string command = "add";
            if (all)
            {
                command += " --all";
            }
            else if(!all && files !=null && files.Length>0)
            {
                foreach (string file in files)
                {
                    command += " "+file;
                }
            }

            return Client.Instance.ExecCommand(command);
        }
    }
}
