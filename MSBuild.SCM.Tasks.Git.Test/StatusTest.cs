using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using MSBuild.SCM.Tasks.Git.Commands;

namespace MSBuild.SCM.Tasks.Git.Test
{
    [TestClass]
    public class StatusTest
    {
        [TestMethod]
        public void LibraryTest()
        {
            List<string> output = Status.ExecCommand();
            string firstLine = output[0];
            bool condition = firstLine.StartsWith("# On branch") || firstLine.StartsWith("On branch");
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
