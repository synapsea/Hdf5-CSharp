using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HDF5CSharp.UnitTests.Core
{
    [TestClass]
    public class FilesUnitTests : Hdf5BaseUnitTests
    {
        private static List<string> Errors { get; set; }

        public FilesUnitTests()
        {
            Errors = new List<string>();
        }
        //[TestMethod]
        public void TestReadStructure()
        {
            Hdf5.Settings.EnableErrorReporting(true);
            Hdf5Utils.LogWarning = (s) => Errors.Add(s);
            Hdf5Utils.LogError = (s) => Errors.Add(s);
            string fileName = @"recorder.hdf5";
            if (File.Exists(fileName))
            {
                var tree = Hdf5.ReadTreeFileStructure(fileName);
                var flat = Hdf5.ReadFlatFileStructure(fileName);
                File.Delete(fileName);
                if (Errors.Any())
                {
                    foreach (string error in Errors)
                    {
                        Console.WriteLine(error);
                    }
                }
                Assert.IsFalse(File.Exists(fileName));
                Assert.IsTrue(tree != null);
                Assert.IsTrue(flat != null);
            }
        }
    }
}

