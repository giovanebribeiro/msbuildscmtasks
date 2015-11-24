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
            //init empty repo
            Client.Instance.ExecCommand("init " + dummyRepo);
        }

        [ClassCleanup]
        public static void DeleteDummyRepo()
        {
            //Directory.Delete(dummyRepo, true);
        }

        [TestMethod]
        public void WithoutMessageAddFalse()
        {
            string contentFile1 = dummyRepo + "\\testFile1.txt";
            using (var writer = new StreamWriter(contentFile1))
            {
                writer.WriteLine("File content for file 1");
            }

            Add.ExecCommand(false, new string[] { contentFile1 });

            List<string> output = Commit.ExecCommand(false, null, null);
            Assert.IsTrue(output[0].StartsWith("[")); //the first line is the branch line
        }
    }
}
