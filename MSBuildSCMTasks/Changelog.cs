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

namespace MSBuildSCMTasks
{
    public class Changelog:Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// Path for file AssemblyInfo.cs.
        /// </summary>
        [Required]
        public string AssemblyInfoPath { get; set; }
        public string FilePath { get; set; }
        public string GitPath { get; set; }
        private string From { get; set; }
        private string To { get; set; }

        #region if GitPath is not informed, we have to find it.
        private void PrepareGitPath()
        {
            if (GitPath == null)
            {
                //Option 1: In registry
                bool gitIsInRegistry = true;
                string registryKeyString = @"SOFTWARE";
                Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registryKeyString);
                if (registryKey == null)
                {
                    registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryKeyString);
                    if (registryKey == null)
                    {
                        gitIsInRegistry = false;
                    }
                }

                // Option 2: In Program Files
                bool gitIsInProgramFiles = true;
                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "Git"))
                {
                    gitIsInProgramFiles = false;
                }

                // Option 3: In ProgramFilesx86
                bool gitIsInProgramFilesx86 = true;
                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "Git"))
                {
                    gitIsInProgramFilesx86 = false;
                }


                if (gitIsInRegistry)
                {
                    registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryKeyString);
                    RegistryKey subKey = registryKey.OpenSubKey("Git-Cheetah");
                    GitPath = subKey.GetValue("PathToMsys").ToString();
                }
                else if (gitIsInProgramFiles)
                {
                    GitPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "Git";
                }
                else if (gitIsInProgramFilesx86)
                {
                    GitPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "Git";
                }
                else
                {
                    throw new InvalidOperationException("The git path wasn't informed and not present in registry or ProgramFiles folder. Please install git or inform the correct git path using the 'GitPath' attribute in task.");
                }
            }
        }
        #endregion

        #region reading the previous changelog file if exists
        private StringBuilder PreparePreviousContentFile()
        {
            StringBuilder previousContentFile = null;
            if (FilePath == null)
            {
                FilePath = ".\\CHANGELOG.md";
            }

            if (File.Exists(FilePath))
            {
                previousContentFile = new StringBuilder();
                StreamReader sr = new StreamReader(FilePath);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    previousContentFile.AppendLine(line);
                }
                sr.Close();
            }

            return previousContentFile;
        }
        #endregion

        #region get the newest version number from provided assembly info
        private string GetProductVersion()
        {
            string versionNumber = "";
#if DEBUG
            Log.LogMessage("AssemblyInfoPath = " + AssemblyInfoPath);
#endif
            if (File.Exists(AssemblyInfoPath))
            {
#if DEBUG
                Log.LogMessage("File exists: OK. Reading...");
#endif
                string[] readText = File.ReadAllLines(AssemblyInfoPath);
                var versionInfoLines = readText.Where(t => t.StartsWith("[assembly: AssemblyVersion"));
                foreach (string item in versionInfoLines)
                {
                    versionNumber = item.Substring(item.IndexOf('(') + 2, item.LastIndexOf(')') - item.IndexOf('(') - 3);
                    break; //only the forst line matters
                }

            }
#if DEBUG
            Log.LogMessage("versionNumber = " + versionNumber);
#endif
            return versionNumber;
        }
        #endregion

        #region execute the git command to extract the git tag information
        private string GetLatestTag(string gitClient)
        {
            string mostRecentTag = "";

            Process gitProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = gitClient,
                    Arguments = "log --tags --simplify-by-decoration --pretty=format:\"%d\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            string firstLine = "";
            gitProcess.Start();
            while (!gitProcess.StandardOutput.EndOfStream)
            {
                firstLine = gitProcess.StandardOutput.ReadLine();
                break;
            }

            Regex pattern = new Regex("v\\d{1,}\\.\\d{1,}\\.\\d{1,}");
            Match match = pattern.Match(firstLine);
            mostRecentTag = match.Groups[0].Value.ToString();

#if DEBUG
            Log.LogMessage("Most recent tag: " + mostRecentTag);
#endif
            return mostRecentTag;
        }
        #endregion

        #region execute the git command to extract the log information
        private List<String> GetRawLogs(string gitClient, string mostRecentTag)
        {
            List<string> gitLogs = new List<string>();
            string gitArgs = "log --format=\"%B%n-hash-%n%H%n-gitTags-%n%d%n-committerDate-%n%ci%n------------------------ >8 ------------------------\"";

            if (!mostRecentTag.Equals(""))
            {
                if (From == null)
                {
                    From = mostRecentTag;
                }
            }

            if (To == null)
            {
                To = "HEAD";
            }

            gitArgs += " " + From + ".." + To;
            //Log.LogMessage("gitArgs = " + gitArgs);
            Process gitProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = gitClient,
                    Arguments = gitArgs,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            gitProcess.Start();
            StringBuilder gitOutput = new StringBuilder();
            while (!gitProcess.StandardOutput.EndOfStream)
            {
                string line = gitProcess.StandardOutput.ReadLine();
                if (line.Equals("------------------------ >8 ------------------------"))
                {
                    gitLogs.Add(gitOutput.ToString());
                    gitOutput.Clear(); //clear for next log
                    continue; //next iteration
                }

                gitOutput.AppendLine(line);
            }
#if DEBUG
            Log.LogMessage("gitLogs = " + gitLogs.First());
#endif

            return gitLogs;
        }
        #endregion

        #region execute git command to get the repo URL:
        private string GetRepoURL(string gitClient)
        {
            string repoURL = "";

            Process gitProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = gitClient,
                    Arguments = "remote -v",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            gitProcess.Start();
            string line = gitProcess.StandardOutput.ReadLine(); //only the first line matters.

            //if line contains 
            //sstring temp
            if (line.Contains("git"))
            {

            }
            string [] lineSplitted = line.Split(new char[]{' ', ':'});

#if DEBUG
            foreach(string lineSplit in lineSplitted){
                Log.LogMessage("lineSplitted = " + lineSplit);
            }
#endif

            return repoURL;
        }
        #endregion

        public override bool Execute()
        {
            PrepareGitPath();

            StringBuilder previousContentFile = PreparePreviousContentFile();            

            string gitClient = this.GitPath + "\\bin\\git.exe";

            string versionNumber = GetProductVersion();

            string mostRecentTag = GetLatestTag(gitClient);

            List<string> gitLogs = GetRawLogs(gitClient, mostRecentTag);

            string repoURL = GetRepoURL(gitClient);
            
            #region mounting body
            
            string regexFooter = "([Rr][Ee][Ss][Oo][Ll][Vv]([Ee]([Ss])?|[Ii][Nn][Gg])|[Cc][Ll][Oo][Ss]([Ee]([Ss]|[Dd])?|[Ii][Nn][Gg])|[Rr][Ee][Oo][Pp][Ee][Nn]([Ss]|[Ii][Nn][Gg])?|[Hh][Oo][Ll][Dd]([Ss]|[Ii][Nn][Gg])?|[Ii][Nn][Vv][Aa][Ll][Ii][Dd][Aa][Tt]([Ee]([Ss]|[Dd])?|[Ii][Nn][Gg])|[Aa][Dd][Dd][Rr][Ee][Ss][Ss][Ee][Ss]|[Ss][Ee][Ee]|[Rr][Ee]([Ff]([Ss])?([Ee][Rr][Ee][Nn][Cc][Ee][Ss])?)?)\\s+(((,|\\s+[Aa][Nn][Dd]) )?#\\d+)+";
            
            StringBuilder bugFeaturesSection = new StringBuilder();
            foreach (string log in gitLogs)
            {
                //string logFirstLine = log.Split('\n')[0];
                string[] logParts = log.Split('\n');
                string logHeader = "";
                string logHash = "";
                string logFooter = "";
                Regex patternFooter = new Regex(regexFooter);
                for (int i=0; i < logParts.Count(); i++){
                    string part = logParts[i];
                    part = part.Trim();
                    
                    if (i == 0)
                    {
                        logHeader = part;
                    }

                    if (part.Equals("-hash-"))
                    {
                        logHash = logParts[i+1];
                    }

                    Match matchFooter = patternFooter.Match(part);
                    if (matchFooter.Success && logFooter.Equals(""))
                    {
                        logFooter = matchFooter.Groups[0].Value.ToString();
                    }
                }

                Log.LogMessage("LogHeader=" + logHeader);
                Log.LogMessage("logHash=" + logHash);
                Log.LogMessage("logFooter=" + logFooter);
                Log.LogMessage("------------");

                // preparing logFooter
                logFooter = "(" + logFooter + ")";

                // preparing logHash. The hash must link to commit page
            }
            #endregion

            /*StringBuilder newFileContent = new StringBuilder();
            String now = DateTime.Now.ToString("d");
            newFileContent.AppendLine("# " + versionNumber + " (" + now + ")");
            newFileContent.AppendLine("");*/

            return true;
        }

    }
}
