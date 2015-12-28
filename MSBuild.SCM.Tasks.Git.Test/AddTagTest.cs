using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using MSBuild.SCM.Tasks.Git.Client;
using System.Collections.Generic;

namespace MSBuild.SCM.Tasks.Git.Test
{

    [TestClass]
    public class AddTagTest
    {
        private static string dummyRepo = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\DummyRepo_AddTag";

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
        public void TagWithDefaultOptions()
        {
            // create a file and commit it:
            string contentFile1 = dummyRepo + "\\testFile1.txt";
            using (var writer = new StreamWriter(contentFile1))
            {
                writer.WriteLine("File content for file 1");
                writer.Close();
            }
            List<string> output = Add.ExecCommand(false, new string[] { contentFile1 });
            output = Commit.ExecCommand(true, "init commit", null);

            // create tag:
            output = AddTag.ExecCommand(null, "0.0.0", null, null);
            //check results
            output = ClientGit.Instance.ExecCommand("tag");
            Assert.AreEqual("v0.0.0", output.Single(s => (s != null && s.Contains("v0.0.0"))));
        }

        [TestMethod]
        public void TagWithCustomPattern()
        {
            // create a file and commit it:
            string contentFile2 = dummyRepo + "\\testFile2.txt";
            using (var writer = new StreamWriter(contentFile2))
            {
                writer.WriteLine("File content for file 2");
                writer.Close();
            }
            List<string> output = Add.ExecCommand(false, new string[] { contentFile2 });
            output = Commit.ExecCommand(true, "init commit", null);

            //create tag
            output = AddTag.ExecCommand("r%VERSION%", "0.0.1", null, null);
            output = ClientGit.Instance.ExecCommand("tag");
            Assert.AreEqual("r0.0.1", output.Single(s=>(s!=null && s.Contains("r0.0.1"))));
        }

        [TestMethod]
        public void TagWithCustomMessage()
        {
            // create a file and commit it:
            string contentFile2 = dummyRepo + "\\testFile2.txt";
            using (var writer = new StreamWriter(contentFile2))
            {
                writer.WriteLine("File content for file 2");
                writer.Close();
            }
            List<string> output = Add.ExecCommand(false, new string[] { contentFile2 });
            output = Commit.ExecCommand(true, "init commit", null);

            // create tag
            output = AddTag.ExecCommand(null, "0.0.2", "New Release with version %VERSION%", null);
            output = ClientGit.Instance.ExecCommand("tag -n1");
            string line = output.Single(s => (s!=null && s.Contains("New Release with version 0.0.2")));
            Assert.IsTrue(line != null);
        }

        [TestMethod]
        public void TagWithVersionFromAssemblyInfo()
        {
            string assemblyInfoPath = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\AssemblyTest.txt";

            using (var writer = new StreamWriter(assemblyInfoPath))
            {
                writer.WriteLine("[assembly: AssemblyVersion(\"0.1.0.0\")]");
            }

            // create a file and commit it:
            string contentFile3 = dummyRepo + "\\testFile3.txt";
            using (var writer = new StreamWriter(contentFile3))
            {
                writer.WriteLine("File content for file 3");
                writer.Close();
            }
            List<string> output = Add.ExecCommand(false, new string[] { contentFile3 });
            output = Commit.ExecCommand(true, "init commit", null);

            // create tag
            output = AddTag.ExecCommand(null, null, null, assemblyInfoPath);
            output = ClientGit.Instance.ExecCommand("tag -n1");
            string line = output.Single(s => (s != null && s.Contains("Release version 0.1.0.0")));
            Assert.IsTrue(line != null);

        }

        [TestMethod]
        public void AddTagTaskTest()
        {
            // create a file and commit it:
            string contentFile2 = dummyRepo + "\\testFile2.txt";
            using (var writer = new StreamWriter(contentFile2))
            {
                writer.WriteLine("File content for file 2");
                writer.Close();
            }
            List<string> output = Add.ExecCommand(false, new string[] { contentFile2 });
            output = Commit.ExecCommand(true, "init commit", null);

            GitAddTag task = new GitAddTag
            {
                BuildEngine = new DummyBuildEngine(),
                Version = "0.0.3"
            };

            bool result = task.Execute();
            Assert.IsTrue(result, "execution failed.");
        }
    }
}
