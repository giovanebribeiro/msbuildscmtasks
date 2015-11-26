using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using MSBuild.SCM.Tasks.Git.Commands;
using System.Collections.Generic;

namespace MSBuild.SCM.Tasks.Git.Test
{
    [TestClass]
    public class AddTest
    {
        private static string dummyRepo = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\DummyRepo_Add";
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
            Client.Instance.ExecCommand("init " + dummyRepo);
        }

        [TestMethod]
        public void AddFile()
        {
            string contentFile1 = dummyRepo + "\\testFile1.txt";
            using (var writer = new StreamWriter(contentFile1))
            {
                writer.WriteLine("File content for file 1");
                writer.Close();
            }
            List<string> output = Add.ExecCommand(false, new string[] { contentFile1 });

            // check output
            output = Status.ExecCommand();
            Assert.IsTrue((output.Single(s=> s!=null && s.Contains("On branch master"))) != null);
            Assert.IsTrue((output.Single(s => s != null && s.Contains("Initial commit"))) != null);
            Assert.IsTrue((output.Single(s => s != null && s.Contains("testFile1.txt"))) != null);            
        }

        [TestMethod]
        public void AddAllFiles()
        {
            string contentFile2 = dummyRepo + "\\testFile2.txt";
            string contentFile3 = dummyRepo + "\\testFile3.txt";
            string contentFile4 = dummyRepo + "\\testFile4.txt";
            string contentFile5 = dummyRepo + "\\testFile5.txt";
            using(var writer2 = new StreamWriter(contentFile2))
            using(var writer3 = new StreamWriter(contentFile3))
            using(var writer4 = new StreamWriter(contentFile4))
            using (var writer5 = new StreamWriter(contentFile5))
            {
                writer2.WriteLine("File content for file 2");
                writer3.WriteLine("File content for file 3");
                writer4.WriteLine("File content for file 4");
                writer5.WriteLine("File content for file 5");

                writer5.Close();
                writer4.Close();
                writer3.Close();
                writer2.Close();
            }
            List<string> output = Add.ExecCommand(true, null);

            output = Status.ExecCommand();
            Assert.IsTrue((output.Single(s => s != null && s.Contains("On branch master"))) != null);
            Assert.IsTrue((output.Single(s => s != null && s.Contains("Initial commit"))) != null);
            Assert.IsTrue((output.Single(s => s != null && s.Contains("testFile2.txt"))) != null);
            Assert.IsTrue((output.Single(s => s != null && s.Contains("testFile3.txt"))) != null);
            Assert.IsTrue((output.Single(s => s != null && s.Contains("testFile4.txt"))) != null);
            Assert.IsTrue((output.Single(s => s != null && s.Contains("testFile5.txt"))) != null);
        }

        [TestMethod]
        public void TaskAddTest()
        {
            string contentFile1 = dummyRepo + "\\testFile1.txt";
            using (var writer = new StreamWriter(contentFile1))
            {
                writer.WriteLine("File content for file 6");
                writer.Close();
            }

            GitAdd t = new GitAdd
            {
                All = true
            };
            t.BuildEngine = new DummyBuildEngine();

            bool result = t.Execute();

            Assert.IsTrue(result, "Execution failed");
        }
    }
}
