using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using MSBuild.SCM.Tasks.Git.Client;
using System.IO;

namespace MSBuild.SCM.Tasks.Git.Test
{
    [TestFixture]
    public class StatusTest
    {
        private static string dummyRepo = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\DummyRepo_Status2";

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
        public void LibraryTest()
        {
            List<string> output = Status.ExecCommand();
            Assert.IsTrue(output.Single(s => s != null && s.Contains("On branch master"))!=null);
            Assert.IsTrue(output.Single(s => s != null && s.Contains("Initial commit")) != null);
            Assert.IsTrue(output.Single(s => s != null && s.Contains("nothing to commit")) != null);
        }

        [Test]
        public void TaskStatusTest()
        {
            GitStatus t = new GitStatus();
            t.BuildEngine = new DummyBuildEngine();

            bool result = t.Execute();

            Assert.IsTrue(result, "Execution failed");
        }
    }
}
