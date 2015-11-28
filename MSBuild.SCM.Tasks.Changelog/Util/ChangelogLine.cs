using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks.Changelog.Util
{
    /// <summary>
    /// 
    /// </summary>
    class ChangelogLine
    {
        public string CommitHeader { get; set; }
        public string CommitFooter { get; set; }
        public string CommitHash { get; set; }

        public string GetCommitType()
        {
            string temp = CommitHeader;
            Regex reg = new Regex("\\(.+\\)");
            temp = reg.Replace(temp, "");

            return temp.Trim();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commitURL"></param>
        /// <returns>The commit line in markdown-style, ready TagEnd be put in changelog files</returns>
        public string MountLine(string commitURL)
        {
            string line = "";

            string commitLink = CommitHash.Substring(0, 7); // the seven first chars are enough.
            commitLink = "([" + commitLink + "](" + commitURL + "/" + CommitHash + "))";

            // the header format: chore(build): Criar a task de listagem
            string[] headerSplit = CommitHeader.Split(':');

            string temp = "";
            if (headerSplit[0].Contains("("))
            {
                temp = headerSplit[0];
                Regex reg = new Regex(".+\\(");
                temp = reg.Replace(temp, "");
                temp = temp.Remove(temp.Length - 1, 1);
                temp = " **" + temp + ":**";               
            }

            line = "-"+temp + " " + headerSplit[1] + " " + commitLink + ", " + CommitFooter;

            return line;
        }
    }
}
