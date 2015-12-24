﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks.BumpVersion
{
    public class Bump
    {
        /// <summary>
        /// Extract the version number from line present in "AssemblyInfo.cs" file.
        /// </summary>
        /// <param name="assemblyInfoLine">Line which contains the version. The line must be the format: [assembly: AssemblyVersion("X.X.X.X")]</param>
        /// <returns>The number version</returns>
        public static string GetVersionNumber(string assemblyInfoLine)
        {
            return assemblyInfoLine.Substring(assemblyInfoLine.IndexOf('(') + 2, assemblyInfoLine.LastIndexOf(')') - assemblyInfoLine.IndexOf('(') - 3);
        }

        public static string Calc(string assemblyInfoPath, string option)
        {
            string newVersion = null;
            string assemblyInfoTemp = assemblyInfoPath + ".temp";
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                using (var writer = new StreamWriter(assemblyInfoTemp))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("[assembly: AssemblyVersion"))
                        {
                            string versionNumber = GetVersionNumber(line);

                            string[] version = Regex.Split(versionNumber, "\\.");
                            int major = 0;
                            int minor = 0;
                            int patch = 0;
                            int build = 0;
                            if (version.Length >= 3 && version.Length <= 4)
                            {
                                major = Convert.ToInt16(version[0]);
                                minor = Convert.ToInt16(version[1]);
                                patch = Convert.ToInt16(version[2]);

                                if(version.Length == 4)
                                {
                                    build = Convert.ToInt16(version[3]);
                                }
                            }
                            else
                            {
                                throw new IndexOutOfRangeException("Version in wrong format!");
                            }
                            
                            option = (option == null) ? "patch" : option;
                            if (option.Equals("build"))
                            {
                                build++;
                            }
                            else if (option.Equals("patch"))
                            {
                                build = 0;
                                patch++;
                            }
                            else if (option.Equals("minor"))
                            {
                                build = 0;
                                patch = 0;
                                minor++;
                            }
                            else if (option.Equals("major"))
                            {
                                build = 0;
                                patch = 0;
                                minor = 0;
                                major++;
                            }
                            else
                            {
                                throw new InvalidDataException("Invalid option: " + option);
                            }

                            newVersion = major + "." + minor + "." + patch+"."+build;
                            line = line.Replace(versionNumber, newVersion);
                        }

                        writer.WriteLine(line);
                    }
                }
            }

            // move file to correct place
            File.Delete(assemblyInfoPath);
            File.Copy(assemblyInfoTemp, assemblyInfoPath, true);

            return newVersion;
        }
    }
}
