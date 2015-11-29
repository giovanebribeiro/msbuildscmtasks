using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using MSBuild.SCM.Tasks.Git.Client;
using System.IO;

namespace MSBuild.SCM.Tasks.Git.Test
{
    [TestClass]
    public class StatusTest
    {
        private static string dummyRepo = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\DummyRepo_Status2";
        [ClassInitialize]
        public static void CreateDummyRepo(TestContext context)
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

        [TestMethod]
        public void LibraryTest()
        {
            List<string> output = Status.ExecCommand();
            Assert.IsTrue(output.Single(s => s != null && s.Contains("On branch master"))!=null);
            Assert.IsTrue(output.Single(s => s != null && s.Contains("Initial commit")) != null);
            Assert.IsTrue(output.Single(s => s != null && s.Contains("nothing to commit")) != null);
        }

        [TestMethod]
        public void TaskStatusTest()
        {
            GitStatus t = new GitStatus();
            t.BuildEngine = new DummyBuildEngine();

            bool result = t.Execute();

            Assert.IsTrue(result, "Execution failed");
        }
    }
}
