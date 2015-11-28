using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Build.Utilities;

namespace MSBuild.SCM.Tasks.Git.Client
{
    /// <summary>
    /// Represents a Git client
    /// </summary>
    public class ClientGit
    {
        private static ClientGit instance;
        private static List<string> Stdout;
        private string GitPath { get; set; }

        #region singleton implementation
        public static ClientGit Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ClientGit();
                }
                return instance;
            }
        }

        private ClientGit()
        {
            DefineGitPath();
        }
        #endregion

        private void DefineGitPath()
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

                string complement = "\\bin\\git.exe";
                if (gitIsInRegistry)
                {
                    registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryKeyString);
                    RegistryKey subKey = registryKey.OpenSubKey("Git-Cheetah");
                    GitPath = subKey.GetValue("PathToMsys").ToString()+complement;
                }
                else if (gitIsInProgramFiles)
                {
                    GitPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "Git" + complement;
                }
                else if (gitIsInProgramFilesx86)
                {
                    GitPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "Git" + complement;
                }
                else
                {
                    throw new InvalidOperationException("The git path wasn't informed and not present in registry or ProgramFiles folder. Please install git or inform the correct git path using the 'GitPath' attribute in task.");
                }
            }
        }

        #region Start process to exec command
        static void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Stdout.Add(e.Data);
        }

        /// <summary>
        /// Execute a general git command
        /// </summary>
        /// <param name="args">The command arguments</param>
        /// <returns>A list of strings with the command output</returns>
        public List<string> ExecCommand(string args)
        {
#if DEBUG 
            Console.WriteLine("gitPath = "+GitPath);
            Console.WriteLine("args = " + args);
#endif
            ProcessStartInfo psinfo = new ProcessStartInfo
            {
                FileName = GitPath,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            Stdout = new List<string>();// clear the buffer.

            using (Process gitProcess = new Process())
            {
                gitProcess.StartInfo = psinfo;
                gitProcess.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);

                bool started = gitProcess.Start();

                gitProcess.BeginOutputReadLine();
                gitProcess.WaitForExit();

                return Stdout;
            };
        }
        #endregion

        #region get the url of repository
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gitClient"></param>
        /// <returns></returns>
        public string GetRepoURL()
        {
            string repoURL = "";

            string line = this.ExecCommand("remote -v").First();

            // first possibility: http/https protocol
            if (line.Contains("http"))
            {
                //line example: "origin  https://github.com/giovanebribeiro/politex.git (fetch)"
                repoURL = Regex.Split(line, "\\s+")[1].Trim();
            }
            else if (line.Contains("git"))
            {
                // line example: "origin  git@github.com:giovanebribeiro/politex.git (fetch)"

                // 1 - get the domain
                Regex pattern = new Regex("@[a-z0-9]+\\.[a-z0-9]+:");
                Match match = pattern.Match(line);
                string domain = match.Groups[0].Value.ToString();
                domain = domain.Remove(0, 1); //remove the '@'
                domain = domain.Remove(domain.Length - 1, 1); // remote the ':'

                // 2 - get the username and repo
                pattern = new Regex(":[a-zA-Z0-9\\./]+\\s*\\(");
                match = pattern.Match(line);
                string usernameAndRepo = match.Groups[0].Value.ToString();
                usernameAndRepo = usernameAndRepo.Remove(0, 1); //remove the ':'
                usernameAndRepo = usernameAndRepo.Remove(usernameAndRepo.Length - 1, 1); // remote the '('
                usernameAndRepo = usernameAndRepo.Trim();

                // 3 - join all
                repoURL = "https://" + domain + "/" + usernameAndRepo;
            }
            else if (line.Contains("file://"))
            {
                repoURL = Regex.Split(line, "\\s+")[1].Trim();
            }

            return repoURL;
        }
        #endregion

        public string GetMostRecentTag()
        {
            string mostRecentTag = this.ExecCommand("log --tags --simplify-by-decoration --pretty=format:\"%d\"").First();
            Regex pattern = new Regex("v\\d{1,}\\.\\d{1,}\\.\\d{1,}");
            Match match = pattern.Match(mostRecentTag);
            mostRecentTag = match.Groups[0].Value.ToString();

            return mostRecentTag;
        }
    }
}
