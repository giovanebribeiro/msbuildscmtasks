using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using MSBuild.SCM.Tasks.Git.Client;
using System.Collections.Generic;

namespace MSBuild.SCM.Tasks.Git.Test
{
    [TestClass]
    public class PushTest
    {
        private static string dummyRepo = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\DummyRepo_Push";
        private static string dummyRepoRemote = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\DummyRepo_PushRemote";
        [ClassInitialize]
        public static void CreateDummyRepo(TestContext context)
        {
            if (Directory.Exists(dummyRepo))
            {
                TasksHelper.DeleteDirectory(dummyRepo, true);
            }

            if (Directory.Exists(dummyRepoRemote))
            {
                TasksHelper.DeleteDirectory(dummyRepoRemote, true);
            }

            // create remote repo
            Directory.CreateDirectory(dummyRepoRemote);
            ClientGit.Instance.ExecCommand("init --bare " + dummyRepoRemote);

            //init empty repo
            ClientGit.Instance.ExecCommand("init " + dummyRepo);
            Directory.SetCurrentDirectory(dummyRepo);

            //add remote repo in dummy repo
            ClientGit.Instance.ExecCommand("remote add origin file://" + dummyRepoRemote);
        }

        [TestMethod]
        public void PushDefaultValues()
        {
            string contentFile1 = dummyRepo + "\\testFile1.txt";
            using (var writer = new StreamWriter(contentFile1))
            {
                writer.WriteLine("File content for file 1");
                writer.Close();
            }
            List<string> output = Add.ExecCommand(false, new string[] { contentFile1 });

            output = Commit.ExecCommand(false, null, null);
            output = Push.ExecCommand(null, null, false);
            output = Status.ExecCommand();
            Assert.IsTrue(output[1].Contains("nothing to commit, working directory clean"));
        }

        [TestMethod]
        public void PushToAnotherBranch()
        {
            // create a new branch
            List<string> output = ClientGit.Instance.ExecCommand("checkout -b another");
            //Assert.IsTrue(output.Count > 1);

            // create the file
            string contentFile3 = dummyRepo + "\\testFile3.txt";
            using (var writer = new StreamWriter(contentFile3))
            {
                writer.WriteLine("File content for file 3");
                writer.Close();
            }
            output = Add.ExecCommand(false, new string[] { contentFile3 });

            // commit...
            output = Commit.ExecCommand(false, "add file", null);

            // push to this branch
            output = Push.ExecCommand(null, "another", false);

            // check result
            output = Status.ExecCommand();
            Assert.IsTrue(output[0].Contains("another"));
            Assert.AreEqual("nothing to commit, working directory clean",output[1]);
        }

        [TestMethod]
        public void PushTask()
        {
            string contentFile2 = dummyRepo + "\\testFile2.txt";
            using (var writer = new StreamWriter(contentFile2))
            {
                writer.WriteLine("File content for file 2");
                writer.Close();
            }
            List<string> output = Add.ExecCommand(false, new string[] { contentFile2 });

            GitPush task = new GitPush();
            task.BuildEngine = new DummyBuildEngine();

            bool result = task.Execute();
            Assert.IsTrue(result, "execution failed.");
        }
    }
}
