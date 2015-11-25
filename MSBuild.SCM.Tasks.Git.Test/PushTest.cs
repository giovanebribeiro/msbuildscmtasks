using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using MSBuild.SCM.Tasks.Git.Commands;

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
            // create remote repo
            Directory.CreateDirectory(dummyRepoRemote);
            Directory.SetCurrentDirectory(dummyRepoRemote);
            Client.Instance.ExecCommand("init --bare " + dummyRepoRemote);

            //init empty repo
            Client.Instance.ExecCommand("init " + dummyRepo);
        }

        [ClassCleanup]
        public static void DeleteDummyRepo()
        {
            Directory.Delete(dummyRepo, true);
        }


        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
