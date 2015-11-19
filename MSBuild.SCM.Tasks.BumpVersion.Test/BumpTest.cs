using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace MSBuild.SCM.Tasks.BumpVersion.Test
{
    [TestClass]
    public class BumpTest
    {
        private static string assemblyInfoPath = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%")+"\\AssemblyTest.txt";
        private string oldVersion = "";

        private string GetVersionNumber(string line)
        {
            return line.Substring(line.IndexOf('(') + 2, line.LastIndexOf(')') - line.IndexOf('(') - 3);
        }

        [TestInitialize]
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

        [TestCleanup]
        public void DestroyTestFile()
        {
            File.Delete(assemblyInfoPath);
        }

        [TestMethod]
        public void PatchOption()
        {
            Bump.Calc(assemblyInfoPath, "patch");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("0.0.1", versionNumber);
            }
        }

        public void PatchAgainOption()
        {
            Bump.Calc(assemblyInfoPath, "patch");
            Bump.Calc(assemblyInfoPath, "patch");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("0.0.2", versionNumber);
            }
        }

        [TestMethod]
        public void MinorOption()
        {
            Bump.Calc(assemblyInfoPath, "minor");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("0.1.0", versionNumber);
            }
        }

        [TestMethod]
        public void PatchAfterMinor()
        {
            Bump.Calc(assemblyInfoPath, "minor");
            Bump.Calc(assemblyInfoPath, "patch");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("0.1.1", versionNumber);
            }
        }

        [TestMethod]
        public void MajorOption()
        {
            Bump.Calc(assemblyInfoPath, "major");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("1.0.0", versionNumber);
            }
        }

        [TestMethod]
        public void MinorAfterMajor()
        {
            Bump.Calc(assemblyInfoPath, "major");
            Bump.Calc(assemblyInfoPath, "minor");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("1.1.0", versionNumber);
            }
        }

        [TestMethod]
        public void PatchAfterMinor2()
        {
            Bump.Calc(assemblyInfoPath, "major");
            Bump.Calc(assemblyInfoPath, "minor");
            Bump.Calc(assemblyInfoPath, "patch");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("1.1.1", versionNumber);
            }
        }

        [TestMethod]
        public void PatchAfterMinorAgain()
        {
            Bump.Calc(assemblyInfoPath, "major");
            Bump.Calc(assemblyInfoPath, "minor");
            Bump.Calc(assemblyInfoPath, "patch");
            Bump.Calc(assemblyInfoPath, "patch");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("1.1.2", versionNumber);
            }
        }

        [TestMethod]
        public void MinorAfterPatch()
        {
            Bump.Calc(assemblyInfoPath, "major");
            Bump.Calc(assemblyInfoPath, "minor");
            Bump.Calc(assemblyInfoPath, "patch");
            Bump.Calc(assemblyInfoPath, "patch");
            Bump.Calc(assemblyInfoPath, "minor");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("1.2.0", versionNumber);
            }
        }

        [TestMethod]
        public void MajorTo2()
        {
            Bump.Calc(assemblyInfoPath, "major");
            Bump.Calc(assemblyInfoPath, "minor");
            Bump.Calc(assemblyInfoPath, "patch");
            Bump.Calc(assemblyInfoPath, "patch");
            Bump.Calc(assemblyInfoPath, "minor");
            Bump.Calc(assemblyInfoPath, "major");
            using (var reader = new StreamReader(assemblyInfoPath))
            {
                string line = reader.ReadLine();
                string versionNumber = GetVersionNumber(line);
                Assert.AreEqual("2.0.0", versionNumber);
            }
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
