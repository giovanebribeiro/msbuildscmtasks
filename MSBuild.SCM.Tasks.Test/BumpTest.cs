using System;
using NUnit.Framework;
using System.IO;
using MSBuild.SCM.Tasks.BumpVersion;

namespace MSBuild.SCM.Tasks.Test
{
    [TestFixture]
    public class BumpTest
    {
        private static string assemblyInfoPath = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%")+"\\AssemblyTest.txt";

        private string GetVersionNumber(string line)
        {
            return line.Substring(line.IndexOf('(') + 2, line.LastIndexOf(')') - line.IndexOf('(') - 3);
        }

        [SetUp]
        public void InitTestFile()
        {
            using (var writer = new StreamWriter(assemblyInfoPath))
            {
                writer.WriteLine("[assembly: AssemblyVersion(\"0.0.0.0\")]");                
            }
            

            // move file to correct place
            //
            //File.Copy(assemblyInfoTemp, assemblyInfoPath, true);
        }

        [TearDown]
        public void DestroyTestFile()
        {
            File.Delete(assemblyInfoPath);
        }

        [Test]
        public void BuildOption()
        {
            BumpCalculator.Calc(assemblyInfoPath, "build");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("0.0.0.1", versionNumber);
            }
        }

        [Test]
        public void PatchOption()
        {
            BumpCalculator.Calc(assemblyInfoPath, "patch");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("0.0.1.0", versionNumber);
            }
        }

        public void PatchAgainOption()
        {
            BumpCalculator.Calc(assemblyInfoPath, "patch");
            BumpCalculator.Calc(assemblyInfoPath, "patch");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("0.0.2.0", versionNumber);
            }
        }

        [Test]
        public void MinorOption()
        {
            BumpCalculator.Calc(assemblyInfoPath, "minor");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("0.1.0.0", versionNumber);
            }
        }

        [Test]
        public void PatchAfterMinor()
        {
            BumpCalculator.Calc(assemblyInfoPath, "minor");
            BumpCalculator.Calc(assemblyInfoPath, "patch");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("0.1.1.0", versionNumber);
            }
        }

        [Test]
        public void MajorOption()
        {
            BumpCalculator.Calc(assemblyInfoPath, "major");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("1.0.0.0", versionNumber);
            }
        }

        [Test]
        public void MinorAfterMajor()
        {
            BumpCalculator.Calc(assemblyInfoPath, "major");
            BumpCalculator.Calc(assemblyInfoPath, "minor");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("1.1.0.0", versionNumber);
            }
        }

        [Test]
        public void PatchAfterMinor2()
        {
            BumpCalculator.Calc(assemblyInfoPath, "major");
            BumpCalculator.Calc(assemblyInfoPath, "minor");
            BumpCalculator.Calc(assemblyInfoPath, "patch");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("1.1.1.0", versionNumber);
            }
        }

        [Test]
        public void PatchAfterMinorAgain()
        {
            BumpCalculator.Calc(assemblyInfoPath, "major");
            BumpCalculator.Calc(assemblyInfoPath, "minor");
            BumpCalculator.Calc(assemblyInfoPath, "patch");
            BumpCalculator.Calc(assemblyInfoPath, "patch");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("1.1.2.0", versionNumber);
            }
        }

        [Test]
        public void MinorAfterPatch()
        {
            BumpCalculator.Calc(assemblyInfoPath, "major");
            BumpCalculator.Calc(assemblyInfoPath, "minor");
            BumpCalculator.Calc(assemblyInfoPath, "patch");
            BumpCalculator.Calc(assemblyInfoPath, "patch");
            BumpCalculator.Calc(assemblyInfoPath, "minor");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("1.2.0.0", versionNumber);
            }
        }

        [Test]
        public void MajorTo2()
        {
            BumpCalculator.Calc(assemblyInfoPath, "major");
            BumpCalculator.Calc(assemblyInfoPath, "minor");
            BumpCalculator.Calc(assemblyInfoPath, "patch");
            BumpCalculator.Calc(assemblyInfoPath, "patch");
            BumpCalculator.Calc(assemblyInfoPath, "minor");
            BumpCalculator.Calc(assemblyInfoPath, "major");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("2.0.0.0", versionNumber);
            }
        }

        [Test]
        public void MajorTo2Build1()
        {
            BumpCalculator.Calc(assemblyInfoPath, "major");
            BumpCalculator.Calc(assemblyInfoPath, "minor");
            BumpCalculator.Calc(assemblyInfoPath, "patch");
            BumpCalculator.Calc(assemblyInfoPath, "patch");
            BumpCalculator.Calc(assemblyInfoPath, "minor");
            BumpCalculator.Calc(assemblyInfoPath, "major");
            BumpCalculator.Calc(assemblyInfoPath, "build");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("2.0.0.1", versionNumber);
            }
        }

        [Test]
        public void TaskTest()
        {
            Bump t = new Bump
            {
                AssemblyInfoPath = assemblyInfoPath
            };
            t.BuildEngine = new DummyBuildEngine();

            bool result = t.Execute();

            Assert.IsTrue(result, "Execution failed");
        }

        /*
        [ClassCleanup]
        public void RestoreOldVersion()
        {
            string assemblyInfoTemp = assemblyInfoPath + ".temp";
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                using (var writer = new StreamWriter(assemblyInfoTemp))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("[assembly: AssemblyVersion"))
                        {
                            string versionNumber = line.Substring(line.IndexOf('(') + 2, line.LastIndexOf(')') - line.IndexOf('(') - 3);
                            oldVersion = versionNumber;
                            line = line.Replace(versionNumber,oldVersion);
                        }

                        writer.WriteLine(line);
                    }
                }
            }

            // move file to correct place
            File.Delete(assemblyInfoPath);
            File.Copy(assemblyInfoTemp, assemblyInfoPath, true);
        }
        */

    }
}
