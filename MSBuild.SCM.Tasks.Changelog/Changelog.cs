using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Reflection;
using MSBuild.SCM.Tasks.Changelog.Util;

namespace MSBuild.SCM.Tasks.Changelog
{
    public class Changelog:Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// Path for file AssemblyInfo.cs.
        /// </summary>
        [Required]
        public string AssemblyInfoPath { get; set; }
        public string ChangelogFilePath { get; set; }
        public string TagStart { get; set; }
        public string TagEnd { get; set; }

        public override bool Execute()
        {
            ChangelogBuilder clb = new ChangelogBuilder
            {
                Assembly_Info_Path = AssemblyInfoPath,
                Changelog_FilePath = ChangelogFilePath,
                Tag_End = TagEnd,
                Tag_Start = TagStart
            };
            return clb.Build();
        }

    }
}
