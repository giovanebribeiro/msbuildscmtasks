using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
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
            Assert.AreEqual("# On branch master", output[0]);
            Assert.AreEqual("# Initial commit", output[2]);
            Assert.AreEqual("# Changes to be committed:", output[4]);
            Assert.IsTrue(output[7].Contains("testFile1.txt"));
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
            Assert.AreEqual("# On branch master", output[0]);
            Assert.AreEqual("# Initial commit", output[2]);
            Assert.AreEqual("# Changes to be committed:", output[4]);
            Assert.IsTrue(output[7].Contains("testFile1.txt"));
            Assert.IsTrue(output[8].Contains("testFile2.txt"));
            Assert.IsTrue(output[9].Contains("testFile3.txt"));
            Assert.IsTrue(output[10].Contains("testFile4.txt"));
            Assert.IsTrue(output[11].Contains("testFile5.txt"));
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
