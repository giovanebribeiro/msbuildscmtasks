using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using MSBuild.SCM.Tasks.Git.Client;
using System.Collections.Generic;
using MSBuild.SCM.Tasks.Changelog.Util;

namespace MSBuild.SCM.Tasks.Changelog.Test
{
    [TestClass]
    public class ChangelogTest
    {
        private static string dummyRepo = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\DummyRepo_Changelog";
        private static string dummyRepoRemote = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\DummyRepo_Changelog_Remote";
        private static string dummyAssemblyInfo = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\DummyAssemblyInfo.cs";
        private static string dummyChangelog = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\CHANGELOG.md";        

        [ClassInitialize]
        public static void CreateAndPopulateDummyRepo(TestContext context)
        {
            #region eliminate the trash from last execution
            if (Directory.Exists(dummyRepo))
            {
                TasksHelper.DeleteDirectory(dummyRepo, true);
            }

            if (Directory.Exists(dummyRepoRemote))
            {
                TasksHelper.DeleteDirectory(dummyRepoRemote, true);
            }

            if (File.Exists(dummyAssemblyInfo))
            {
                /*
                // Get the attributes of the file
                var attr = File.GetAttributes(dummyAssemblyInfo);
                // Is this file marked as 'read-only'?
                if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    // Yes... Remove the 'read-only' attribute, then
                    File.SetAttributes(dummyAssemblyInfo, attr ^ FileAttributes.ReadOnly);
                }
                */
                File.Delete(dummyAssemblyInfo);
            }

            if (File.Exists(dummyChangelog))
            {
                /*
                // Get the attributes of the file
                var attr = File.GetAttributes(dummyAssemblyInfo);
                // Is this file marked as 'read-only'?
                if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    // Yes... Remove the 'read-only' attribute, then
                    File.SetAttributes(dummyAssemblyInfo, attr ^ FileAttributes.ReadOnly);
                }
                */
                File.Delete(dummyChangelog);
            }
            #endregion

            #region create a dummy AssemblyInfo
            using (var writer = new StreamWriter(dummyAssemblyInfo))
            {
                writer.WriteLine("[assembly: AssemblyVersion(\"1.0.0\")]");
                writer.Close();
            }
            #endregion

            #region create repositories
            Directory.CreateDirectory(dummyRepoRemote);
            ClientGit.Instance.ExecCommand("init --bare " + dummyRepoRemote);

            Directory.CreateDirectory(dummyRepo);
            Directory.SetCurrentDirectory(dummyRepo);

            //init empty repo
            ClientGit.Instance.ExecCommand("init " + dummyRepo);
            ClientGit.Instance.ExecCommand("remote add origin file://" + dummyRepoRemote);
            #endregion

            // create some files and commit them
            #region commit file 0
            int fileNumber = 0;
            string contentFile = dummyRepo + "\\testFile"+fileNumber+".txt";
            using (var writer = new StreamWriter(contentFile))
            {
                writer.WriteLine("File content for file "+fileNumber);
                writer.Close();
            }
            List<string> output = Add.ExecCommand(false, new string[] { contentFile });
            output = Commit.ExecCommand(false, "init commit", null);
            #endregion

            #region create tag for initial commit
            output = AddTag.ExecCommand(null, "0.0.0", null);
            output = ClientGit.Instance.ExecCommand("tag -l");
            #endregion

            #region commit file 1
            fileNumber++;
            contentFile = dummyRepo + "\\testFile" + fileNumber + ".txt";
            using (var writer = new StreamWriter(contentFile))
            {
                writer.WriteLine("File content for file " + fileNumber);
                writer.Close();
            }
            output = Add.ExecCommand(false, new string[] { contentFile });
            output = Commit.ExecCommand(false, "feat: add file "+fileNumber, null);
            #endregion

            #region commit file 2
            fileNumber++;
            contentFile = dummyRepo + "\\testFile" + fileNumber + ".txt";
            using (var writer = new StreamWriter(contentFile))
            {
                writer.WriteLine("File content for file " + fileNumber);
                writer.Close();
            }
            output = Add.ExecCommand(false, new string[] { contentFile });
            output = Commit.ExecCommand(false, "feat: add file " + fileNumber, null);
            #endregion

            #region commit file 3
            fileNumber++;
            contentFile = dummyRepo + "\\testFile" + fileNumber + ".txt";
            using (var writer = new StreamWriter(contentFile))
            {
                writer.WriteLine("File content for file " + fileNumber);
                writer.Close();
            }
            output = Add.ExecCommand(false, new string[] { contentFile });
            output = Commit.ExecCommand(false, "feat: add file " + fileNumber, null);
            #endregion

            #region update last file
            using (var writer = new StreamWriter(contentFile))
            {
                writer.WriteLine("new content for file "+fileNumber);
                writer.Close();
            }
            output = Commit.ExecCommand(true, "fix: new content for file "+fileNumber, null);
            #endregion
        }

        [TestMethod]
        public void CreateChangelog()
        {
            ChangelogBuilder clb = new ChangelogBuilder
            {
                _AssemblyInfoPath = dummyAssemblyInfo,
                _ChangelogFilePath = dummyChangelog,
                _TagEnd = null,
                _TagStart = null
            };
            bool result = clb.Build();
            Assert.IsTrue(result, "changelog build execution failed");
            Assert.IsTrue(File.Exists(dummyChangelog), "file not created");
        }

        [TestMethod]
        public void ChangelogTaskTest()
        {
            // update dummy assemblyInfo
            File.Delete(dummyAssemblyInfo);
            using (var writer = new StreamWriter(dummyAssemblyInfo))
            {
                writer.WriteLine("[assembly: AssemblyVersion(\"2.0.0\")]");
                writer.Close();
            }

            Changelog task = new Changelog
            {
                AssemblyInfoPath = dummyAssemblyInfo,
                BuildEngine = new DummyBuildEngine()                
            };
            bool result = task.Execute();

            Assert.IsTrue(result);
            


        }
    }
}
