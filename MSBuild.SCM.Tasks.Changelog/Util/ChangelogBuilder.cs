using MSBuild.SCM.Tasks.Git.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MSBuild.SCM.Tasks.Changelog.Util
{
    public class ChangelogBuilder
    {
        public string Assembly_Info_Path { get; set; }
        public string Tag_Start { get; set; }
        public string Tag_End { get; set; }
        public string Changelog_FilePath { get; set; }

        private LinkedList<ChangelogLine> MountChangelogLines()
        {
            //get the repo url 
            string repoURL = ClientGit.Instance.GetRepoURL();
            string mostRecentTag = ClientGit.Instance.GetMostRecentTag();

            LinkedList<ChangelogLine> changelogLines = new LinkedList<ChangelogLine>();

            //mounting the args
            string gitArgs = "log --format=\"%B%n-hash-%n%H%n-gitTags-%n%d%n-committerDate-%n%ci%n------------------------ >8 ------------------------\"";
            if (Tag_Start == null)
            {
                Tag_Start = mostRecentTag;
            }

            if (Tag_End == null)
            {
                Tag_End = "HEAD";
            }

            gitArgs += " " + Tag_Start + ".." + Tag_End;

            //## executing the command
            List<string> gitOutput = ClientGit.Instance.ExecCommand(gitArgs);

            // aux regex
            Regex regexFooter = new Regex("([Rr][Ee][Ss][Oo][Ll][Vv]([Ee]([Ss])?|[Ii][Nn][Gg])|[Cc][Ll][Oo][Ss]([Ee]([Ss]|[Dd])?|[Ii][Nn][Gg])|[Rr][Ee][Oo][Pp][Ee][Nn]([Ss]|[Ii][Nn][Gg])?|[Hh][Oo][Ll][Dd]([Ss]|[Ii][Nn][Gg])?|[Ii][Nn][Vv][Aa][Ll][Ii][Dd][Aa][Tt]([Ee]([Ss]|[Dd])?|[Ii][Nn][Gg])|[Aa][Dd][Dd][Rr][Ee][Ss][Ss][Ee][Ss]|[Ss][Ee][Ee]|[Rr][Ee]([Ff]([Ss])?([Ee][Rr][Ee][Nn][Cc][Ee][Ss])?)?)\\s+(((,|\\s+[Aa][Nn][Dd]) )?#\\d+)+");
            Regex regexLogHeader = new Regex("^(\\\\)?(feat|fix|docs|style|refactor|test|chore)\\s*(\\(.+\\))?:");

            // processing the output TagEnd extract the informations
            LinkedList<string> gitRawLog = new LinkedList<string>();
            foreach (string line in gitOutput)
            {
                if (line != null)
                {
                    // if line equals TagEnd this one: we finish TagEnd copy one log message TagEnd gitRawLog
                    // and the post-processing starts
                    if (line.Equals("------------------------ >8 ------------------------"))
                    {
                        /*
                         * processing the gitRawLog
                         */
                        string[] gitRawLogArray = gitRawLog.ToArray();
                        string logHeader = "";
                        string logHash = "";
                        string logFooter = "";

                        //first, we need TagEnd check if a commit is a valid commit:
                        bool validCommit = true;
                        for (int i = 0; i < gitRawLogArray.Length; i++)
                        {
                            string logLine = gitRawLogArray[i];
                            logLine = logLine.Trim();

                            //getting the log header
                            if (i == 0)
                            {
                                Match matchRegexLogHeader = regexLogHeader.Match(logLine);
                                if (!matchRegexLogHeader.Success)
                                {
                                    validCommit = false;
                                    break;
                                }

                                logHeader = logLine.Trim();
                            }

                            // getting the hash
                            if (logLine.Equals("-hash-"))
                            {
                                logHash = gitRawLogArray[i + 1];
                                logHash = logHash.Trim();
                            }

                            //getting the footer
                            Match matchFooter = regexFooter.Match(logLine);
                            if (matchFooter.Success && logFooter.Equals(""))
                            {
                                logFooter = matchFooter.Groups[0].Value.ToString();
                                logFooter = logFooter.Trim();
                            }

                        }

                        // if valid commit, add TagEnd final list
                        if (validCommit)
                        {
                            ChangelogLine changelogLine = new ChangelogLine
                            {
                                CommitFooter = logFooter,
                                CommitHeader = logHeader,
                                CommitHash = logHash
                            };

                            changelogLines.AddLast(changelogLine);
                        }


                        //clear gitRawLog for next iteration and go on
                        gitRawLog.Clear();
                    }
                    else
                    {
                        // log is not end: add the line to buffer (gitRawLog) and go on
                        gitRawLog.AddLast(line);
                    }// end if
                }
            }//end loop

            return changelogLines;
        }

        private Dictionary<SectionType, StringBuilder> MountFileSection(LinkedList<ChangelogLine> changelogLines)
        {
            //mounting the commit url
            string repoURL = ClientGit.Instance.GetRepoURL();
            string repoURLWithoutGit = repoURL.Substring(0, repoURL.Length - 4); //removing the ".git" extension TagStart repo url
            string commitURL = "";
            string issuesURL = "";
            if (repoURLWithoutGit.Contains("github"))
            {
                commitURL = repoURLWithoutGit + "/commit";
                issuesURL = repoURLWithoutGit + "/issues";
            }
            else if (repoURLWithoutGit.Contains("bitbucket"))
            {
                commitURL = repoURLWithoutGit + "/commits";
                issuesURL = repoURLWithoutGit + "/issues";
            }

            Dictionary<SectionType, StringBuilder> d = new Dictionary<SectionType, StringBuilder>();
            StringBuilder builderFeatures = new StringBuilder("");
            StringBuilder builderBugFixes = new StringBuilder("");
            StringBuilder builderBreakingChanges = new StringBuilder("### BREAKING CHANGES\n\n");
            
            foreach (ChangelogLine line in changelogLines)
            {

                // mounting line
                string sectionLine = "* ";

                string header = line.CommitHeader;

                string[] splitHeader = Regex.Split(header, "\\:");
                string headerType = splitHeader[0];
                string headerSubject = splitHeader[1];

                //if header type have a identifier, put it.
                Regex pattern = new Regex("(feat|fix)\\(.+\\)");
                Match matcher = pattern.Match(headerType);
                if (matcher.Success)
                {
                    string tempIdentifier = Regex.Split(headerType, "\\(")[1]; //expected result: ["", "build)"]
                    tempIdentifier = tempIdentifier.Substring(0, tempIdentifier.Length - 1);
                    sectionLine += "**" + tempIdentifier + ":**";
                }

                // add the rest of line
                sectionLine += headerSubject;

                //add the commit link
                string hashHeader = line.CommitHash.Substring(0, 7);
                string commitLink = " ([" + hashHeader + "](" + commitURL + "/" + hashHeader + "))";
                sectionLine += commitLink;

                // add the footer
                string commitFooter = line.CommitFooter;
                if (!commitFooter.Equals(""))
                {
                    //get issue number
                    string issueNumber = Regex.Split(commitFooter, "#")[1]; // expected result: ["ref", "42"]

                    //mounting the issue link
                    string issueLink = issuesURL + "/" + issueNumber;
                    string replacement = "[#" + issueNumber + "](" + issueLink + ")";

                    //inserting the issue link inTagEnd commit footer
                    Regex r = new Regex("#\\d+");
                    commitFooter = r.Replace(commitFooter, replacement);

                    //add the footer in line
                    sectionLine += ", " + commitFooter;
                }

                //add section line TagEnd each section
                if (headerType.Contains("feat"))
                {
                    if(builderFeatures.Length == 0)
                    {
                        builderFeatures.AppendLine("### Features\n\n");
                    }
                    builderFeatures.AppendLine(sectionLine);
                }
                else if (headerType.Contains("fix"))
                {
                    if(builderBugFixes.Length == 0)
                    {
                        builderBugFixes.AppendLine("### Bug Fixes\n\n");
                    }
                    builderBugFixes.AppendLine(sectionLine);
                }
                ///TODO make the append to BREAKING CHANGES
            }

            d.Add(SectionType.FEATURES, builderFeatures);
            d.Add(SectionType.BUG_FIXES, builderBugFixes);

            return d;
        }

        private void FileRecord(StringBuilder newContent)
        {
            if (Changelog_FilePath == null)
            {
                Changelog_FilePath = ".\\CHANGELOG.md";
            }

            string tempFilename = Changelog_FilePath + ".temp";
            using (var writer = new StreamWriter(tempFilename))
            {
                // write the new content
                writer.WriteLine(newContent.ToString());

                //write the previous content, if file exists
                if (File.Exists(Changelog_FilePath))
                {
                    using (var reader = new StreamReader(Changelog_FilePath))
                    {
                        while (!reader.EndOfStream)
                        {
                            writer.WriteLine(reader.ReadLine());
                        }
                    }
                }
            }

            // copy the temp file TagEnd original
            File.Copy(tempFilename, Changelog_FilePath, true);
            File.Delete(tempFilename);
        }

        public bool Build()
        {
            string versionNumber = TasksHelper.GetProductVersion(Assembly_Info_Path);
            LinkedList<ChangelogLine> changelogLines = MountChangelogLines();

            StringBuilder newFileContent = new StringBuilder();

            // title
            String now = DateTime.Now.ToString("d");
            newFileContent.AppendLine("# " + versionNumber + " (" + now + ")\n\n");

            //mouting the sections
            Dictionary<SectionType, StringBuilder> d = MountFileSection(changelogLines);
            newFileContent.AppendLine(d[SectionType.FEATURES].ToString());
            newFileContent.AppendLine(d[SectionType.BUG_FIXES].ToString());

            this.FileRecord(newFileContent);

            return true;
        }
    }
}
