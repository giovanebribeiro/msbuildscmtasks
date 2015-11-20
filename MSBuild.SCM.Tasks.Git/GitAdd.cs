using Microsoft.Build.Framework;
using MSBuild.SCM.Tasks.Git.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks.Git
{
    public class GitAdd:Microsoft.Build.Utilities.Task
    {
        public bool All { get; set; }
        public ITaskItem[] FileList { get; set; }

        public override bool Execute()
        {
            if (All == null && FileList == null)
            {
                Log.LogError("At least one of parameters must be set.");
                return false;
            }

            if(All == null)
            {
                All = false;
            }

            string[] _fileList = null;
            if (FileList == null)
            {
                _fileList = new string[] { };
            }
            else
            {
                _fileList = new string[FileList.Length];
                int i = 0;
                foreach (ITaskItem file in FileList)
                {
                    _fileList[i] = file.ItemSpec;
                }
            }

            List<string> output = Add.ExecCommand(All, _fileList);

            return true;
        }
    }
}
