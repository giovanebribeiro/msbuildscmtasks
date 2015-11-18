using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks.BumpVersion
{
    class BumpVersion:Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// Path to AssemblyInfo.cs file
        /// </summary>
        [Required]
        public string AssemblyInfoPath { get; set; }
        /// <summary>
        /// Bump option. Can be:
        /// - patch (default option)
        /// - minor
        /// - major
        /// </summary>
        public string Option { get; set; }

        public override bool Execute()
        {
            Bump.Calc(AssemblyInfoPath, Option);
            return true;
        }

    }
}
