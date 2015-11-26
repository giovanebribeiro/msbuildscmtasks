using Microsoft.Build.Framework;
using MSBuild.SCM.Tasks.Git.Commands;
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
        public ITaskItem[] FileList { get; set; }
        public string Message { get; set; }

        public override bool Execute()
        {
            // putting the file list on command
            string[] _fileList = null;
            if (FileList != null && FileList.Length > 0)
            {
                _fileList = new string[FileList.Count()];
                for (int i=0; i < FileList.Count(); i++)
                {
                    _fileList[i] = FileList[i].ItemSpec;
                }
            }

            List<string> output = Commit.ExecCommand(Add, Message, _fileList);
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
