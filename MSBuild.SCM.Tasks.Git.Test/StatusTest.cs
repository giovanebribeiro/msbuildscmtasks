using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using MSBuild.SCM.Tasks.Git.Commands;
using System.IO;

namespace MSBuild.SCM.Tasks.Git.Test
{
    [TestClass]
    public class StatusTest
    {
        private static string dummyRepo = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\DummyRepo_Status";
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
            //Directory.Delete(dummyRepo, true);
        }


        [TestMethod]
        public void LibraryTest()
        {
            List<string> output = Status.ExecCommand();
            Assert.AreEqual("# On branch master", output[0]);
            Assert.AreEqual("#", output[1]);
            Assert.AreEqual("# Initial commit", output[2]);
            Assert.AreEqual("#", output[3]);
            bool condition = output[4].Equals("nothing to commit(create / copy files and use \"git add\" to track)") 
                || output[4].Equals("nothing to commit (create/copy files and use \"git add\" to track)");
            Assert.IsTrue(condition);
        }

        [TestMethod]
        public void TaskTest()
        {
            GitStatus t = new GitStatus();
            t.BuildEngine = new DummyBuildEngine();

            bool result = t.Execute();

            Assert.IsTrue(result, "Execution failed");
        }
    }
}
