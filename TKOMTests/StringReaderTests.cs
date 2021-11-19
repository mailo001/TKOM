using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TKOM.CharReaders;

namespace TKOMTests
{
    [TestClass]
    public class StringReaderTests
    {
        [TestMethod]
        public void CreateCorrectly()
        {
            try
            {
                StringReader reader = new StringReader("ABC");
            }
            catch
            {
                Assert.Fail("No exception is occure");
            }
        }

        [TestMethod]
        public void CreateWithNullString()
        {
            StringReader reader;

            Assert.ThrowsException<ArgumentNullException>(() => reader = new StringReader(null));
        }

        [TestMethod]
        public void CreateWithEmptyString()
        {
            StringReader reader;

            Assert.ThrowsException<ArgumentException>(() => reader = new StringReader(""));
        }

        [TestMethod]
        public void GetFirstCharWithoutMove()
        {
            StringReader reader = new StringReader("ABC");

            Assert.AreEqual((char)0, reader.CurrentChar);
        }

        [TestMethod]
        public void GetNexCharCorrectly()
        {
            StringReader reader = new StringReader("ABC");
            
            Assert.AreEqual(true, reader.MoveToNextChar());

            Assert.AreEqual('A', reader.CurrentChar);
        }

        [TestMethod]
        public void GetPositonCorrectly()
        {
            StringReader reader = new StringReader("ABC");

            Assert.AreEqual(true, reader.MoveToNextChar());
            Assert.AreEqual(true, reader.MoveToNextChar());

            Assert.AreEqual((1,2), reader.CurrentPosition);
        }

        [TestMethod]
        public void MoveOutOfRange()
        {
            StringReader reader = new StringReader("ABC");

            Assert.AreEqual(true, reader.MoveToNextChar());
            Assert.AreEqual(true, reader.MoveToNextChar());
            Assert.AreEqual(true, reader.MoveToNextChar());

            Assert.AreEqual(false, reader.MoveToNextChar());

            Assert.AreEqual((char)0, reader.CurrentChar);
        }

        [TestMethod]
        public void ResetCorrectly()
        {
            StringReader reader = new StringReader("ABC");

            Assert.AreEqual(true, reader.MoveToNextChar());

            reader.Restart();

            Assert.AreEqual((char)0, reader.CurrentChar);
            Assert.AreEqual(true, reader.MoveToNextChar());
            Assert.AreEqual('A', reader.CurrentChar);
        }

        [TestMethod]
        public void CuntingLineCorrectly()
        {
            StringReader reader = new StringReader("A\nB\nC");

            for(int i=0;i<5;i++)
            {
                Assert.AreEqual(true, reader.MoveToNextChar());
            }

            Assert.AreEqual((3,1), reader.CurrentPosition);
        }

        [TestMethod]
        public void GetTextFromPositionCorrectly()
        {
            StringReader reader = new StringReader("A1\nB2\nC33\nD4");

            Assert.AreEqual("33", reader.GetStringFromPosition((3,2),2));
        }

        [TestMethod]
        public void GetTextFromNonExistingLine()
        {
            StringReader reader = new StringReader("A\nB\nC\nD");

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => reader.GetStringFromPosition((10, 1), 1));
        }

        [TestMethod]
        public void GetTooLongText()
        {
            StringReader reader = new StringReader("A\nB\nC\nD");

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => reader.GetStringFromPosition((2, 10), 1));
        }
    }
}
