using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TKOM.Interfaces;
using TKOM.CharReaders;

namespace TKOMTests
{
    [TestClass]
    public class FileReaderTests
    {
        [TestMethod]
        public void CreateFileReaderCorectly()
        {
            File.Create("test.txt");
            StreamWriter streamWriter = new StreamWriter("test.txt");
            streamWriter.Write("int abc; \n abc = 123;");
            streamWriter.Close();

            try
            {
                FileReader fileReader = new FileReader("test.txt");
                Assert.AreEqual('\0', fileReader.CurrentChar);
            }
            catch
            {
                Assert.Fail("No error is nead");
            }
        }
    }
}
