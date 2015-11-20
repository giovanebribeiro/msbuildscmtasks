using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks.Git.Commands
{
    public class Client
    {
        private static Client instance;
        private string GitPath { get; set; }

        public static Client Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Client();
                }
                return instance;
            }
        }

        private Client()
        {
            DefineGitPath();
        }

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


        public List<string> ExecCommand(string args)
        {
#if DEBUG 
            Console.WriteLine("gitPath = "+GitPath);
            Console.WriteLine("args = " + args);
#endif
            Process gitProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = GitPath,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            gitProcess.Start();
            List<string> gitOutput = null;
            if (gitProcess.StandardOutput != null)
            {
                gitOutput = new List<string>();
            }

            while (!gitProcess.StandardOutput.EndOfStream)
            {
                string line = gitProcess.StandardOutput.ReadLine();
                //if (line.Equals("------------------------ >8 ------------------------"))
                //{
                //    gitLogs.Add(gitOutput.ToString());
                //    gitOutput.Clear(); //clear for next log
                //    continue; //next iteration
                //}

                gitOutput.Add(line);
            }

            return gitOutput;
        }
    }
}
