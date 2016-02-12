using System;
using NUnit.Framework;
using System.IO;
using MSBuild.SCM.Tasks.Git.Client;
using System.Collections.Generic;
using System.Diagnostics;

namespace MSBuild.SCM.Tasks.Test
{
    [TestFixture]
    public class CommitTest
    {
        private static string dummyRepo = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\DummyRepo_Commit";

        [OneTimeSetUp]
        public static void CreateDummyRepo()
        {
            if (Directory.Exists(dummyRepo))
            {
                TasksHelper.DeleteDirectory(dummyRepo, true);
            }


            Directory.CreateDirectory(dummyRepo);
            Directory.SetCurrentDirectory(dummyRepo);

            //init empty repo
            ClientGit.Instance.ExecCommand("init " + dummyRepo);
        }

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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
