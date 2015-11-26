using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using MSBuild.SCM.Tasks.Git.Commands;
using System.Collections.Generic;
using System.Diagnostics;

namespace MSBuild.SCM.Tasks.Git.Test
{
    [TestClass]
    public class CommitTest
    {
        private static string dummyRepo = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\DummyRepo_Commit";
        [ClassInitialize]
        public static void CreateDummyRepo(TestContext context)
        {
            Directory.CreateDirectory(dummyRepo);
            Directory.SetCurrentDirectory(dummyRepo);

            //init empty repo
            Client.Instance.ExecCommand("init " + dummyRepo);
        }

        [ClassCleanup]
        public static void DeleteDummyRepo()
        {
            Directory.Delete(dummyRepo, true);
        }

        [TestMethod]
        public void WithoutMessageAddFalse()
        {
            string contentFile1 = dummyRepo + "\\testFile1.txt";
            using (var writer = new StreamWriter(contentFile1))
            {
                writer.WriteLine("File content for file 1");
                writer.Close();
            }

            List<string> output = Add.ExecCommand(false, new string[] { contentFile1 });

            output = Commit.ExecCommand(false, null, null);
            Assert.IsTrue(output[0].StartsWith("[")); //the first line is the branch line
            Assert.IsTrue(output[2].Contains("create mode") && output[2].Contains("testFile1.txt"));
        }

        [TestMethod]
        public void WithMessageAddTrue()
        {
            string contentFile2 = dummyRepo + "\\testFile2.txt";
            using (var writer = new StreamWriter(contentFile2))
            {
                writer.WriteLine("File content for file 2");
                writer.Close();
            }

            List<string> output = Add.ExecCommand(true, null);
            Commit.ExecCommand(false, null, null); // create new file

            //update the file content
            using (var writer = new StreamWriter(contentFile2))
            {
                writer.WriteLine("Update content of file 2");
                writer.Close();
            }

            string msg = "test commit custom message";
            output = Commit.ExecCommand(true, msg, null);
            Assert.IsTrue(output[0].StartsWith("[") && output[0].Contains(msg)); //the first line is the branch line
            Assert.IsTrue(output[1].Contains("1 file changed"));
        }

        [TestMethod]
        public void AddTwoFilesButCommitOne()
        {
            string contentFile3 = dummyRepo + "\\testFile3.txt";
            string contentFile4 = dummyRepo + "\\testFile4.txt";
            using (var writer = new StreamWriter(contentFile3))
            using (var writer2 = new StreamWriter(contentFile4))
            {
                writer.WriteLine("File content for file 3");
                writer2.WriteLine("File content for file 4");
                writer.Close();
                writer2.Close();
            }
            List<string> output = Add.ExecCommand(false, new string[] { contentFile3, contentFile4 });

            output = Commit.ExecCommand(false, "commit message", new string[] { contentFile3 });
            Assert.IsTrue(output[2].Contains("create mode"));
        }

        [TestMethod]
        public void TaskCommitTest()
        {
            // create a file to commit
            string contentFile4 = dummyRepo + "\\testFile4.txt";
            using (var writer = new StreamWriter(contentFile4))
            {
                writer.WriteLine("File content for file 4");
                writer.Close();
            }

            List<string> output = Add.ExecCommand(true, null);

            GitCommit t = new GitCommit
            {
                Message = "Init commit"
            };
            t.BuildEngine = new DummyBuildEngine();

            bool result = t.Execute();
            Assert.IsTrue(result, "Execution failed");
        }
    }
}
