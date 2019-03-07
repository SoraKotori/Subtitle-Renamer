using Microsoft.VisualStudio.TestTools.UnitTesting;
using Subtitle_Renamer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subtitle_Renamer.Tests
{
    [TestClass()]
    public class FileDictionaryTests
    {
        [TestMethod()]
        public void GetFileNamesWithoutExtensionTest()
        {

            Assert.Fail();
        }

        [TestMethod()]
        public void GetFilePathTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddFilesTest()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    MediaList.Items.Add(filename);
                }
            }

            var MediaFiles = new MediaDictionary();
            string[] FilePaths =
            {

            };

            MediaFiles.AddFiles()
            Assert.Fail();
        }

        [TestMethod()]
        public void RemoveFilesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ClearFilesTest()
        {
            Assert.Fail();
        }
    }
}