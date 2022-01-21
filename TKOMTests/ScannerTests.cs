using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TKOM.Common;
using TKOM.Interfaces;
using TKOM.CharReaders;
using TKOM.Scanners;
using TKOM.Errors;

namespace TKOMTests
{
    [TestClass]
    public class ScannerTests
    {
        [TestMethod]
        public void InitScannerCorectly()
        {
            ErrorsCollector errors = new ErrorsCollector();
            ICharReader charReader = new StringReader("if(i==0){a=2;}");
            try
            {
                Scanner scanner = new Scanner(charReader, errors);
                Assert.IsNotNull(scanner.CurrentToken);
                Assert.AreEqual(TokenType.EMPTY, scanner.CurrentToken.TokenType);
            }
            catch
            {
                Assert.Fail("No Exeption is correct");
            }
        }

        [TestMethod]
        public void InitScannerIncorectly()
        {
            ErrorsCollector errors = new ErrorsCollector();
            Assert.ThrowsException<ArgumentNullException>(() => { Scanner scanner = new Scanner(null,errors); });
        }

        [TestMethod]
        public void GetTokenCorectly()
        {
            ErrorsCollector errors = new ErrorsCollector();
            ICharReader charReader = new StringReader("  if( !\n i== 0) ");
            Scanner scanner = new Scanner(charReader, errors);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.IF, scanner.CurrentToken.TokenType);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.BRACKET_ENTER, scanner.CurrentToken.TokenType);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.NOT, scanner.CurrentToken.TokenType);

            scanner.MoveToNextToken();
            Assert.AreEqual(TokenType.IDENTIFIRE, scanner.CurrentToken.TokenType);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.EQUAL, scanner.CurrentToken.TokenType);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.NUMBER, scanner.CurrentToken.TokenType);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.BRACKET_END, scanner.CurrentToken.TokenType);

            Assert.AreEqual(false, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.EOF, scanner.CurrentToken.TokenType);

        }

        [TestMethod]
        public void WrongSignInToken()
        {
            ErrorsCollector errors = new ErrorsCollector();
            ICharReader charReader = new StringReader("  if( \n i^== 0) ");
            Scanner scanner = new Scanner(charReader, errors);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.IF, scanner.CurrentToken.TokenType);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.BRACKET_ENTER, scanner.CurrentToken.TokenType);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.IDENTIFIRE, scanner.CurrentToken.TokenType);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.UNKNOWN, scanner.CurrentToken.TokenType);
            Assert.AreEqual("^", scanner.CurrentToken.Text);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.EQUAL, scanner.CurrentToken.TokenType);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.NUMBER, scanner.CurrentToken.TokenType);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.BRACKET_END, scanner.CurrentToken.TokenType);

            Assert.AreEqual(false, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.EOF, scanner.CurrentToken.TokenType);
        }

        [TestMethod]
        public void ManyBracketToken()
        {
            ErrorsCollector errors = new ErrorsCollector();
            ICharReader charReader = new StringReader(" \n({( ");
            Scanner scanner = new Scanner(charReader, errors);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.BRACKET_ENTER, scanner.CurrentToken.TokenType);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.CURLY_BRACKET_ENTER, scanner.CurrentToken.TokenType);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.BRACKET_ENTER, scanner.CurrentToken.TokenType);
        }

        [TestMethod]
        public void WrongOperatorToken()
        {
            ErrorsCollector errors = new ErrorsCollector();
            ICharReader charReader = new StringReader("while a*=2");
            Scanner scanner = new Scanner(charReader, errors);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.WHILE, scanner.CurrentToken.TokenType);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.IDENTIFIRE, scanner.CurrentToken.TokenType);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.UNKNOWN, scanner.CurrentToken.TokenType);
            Assert.AreEqual("*=", scanner.CurrentToken.Text);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.NUMBER, scanner.CurrentToken.TokenType);
        }

        [TestMethod]
        public void WrongIdentifierToken()
        {
            ErrorsCollector errors = new ErrorsCollector();
            ICharReader charReader = new StringReader("try 2a2c=12345");
            Scanner scanner = new Scanner(charReader, errors);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.TRY, scanner.CurrentToken.TokenType);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.UNKNOWN, scanner.CurrentToken.TokenType);
            Assert.AreEqual("2a2c", scanner.CurrentToken.Text);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.ASSIGN, scanner.CurrentToken.TokenType);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.NUMBER, scanner.CurrentToken.TokenType);
        }

        [TestMethod]
        public void CorrectIdentifierToken()
        {
            ErrorsCollector errors = new ErrorsCollector();
            ICharReader charReader = new StringReader("try c2a2c=12345");
            Scanner scanner = new Scanner(charReader, errors);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.TRY, scanner.CurrentToken.TokenType);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.IDENTIFIRE, scanner.CurrentToken.TokenType);
            Assert.AreEqual("c2a2c", scanner.CurrentToken.Text);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.ASSIGN, scanner.CurrentToken.TokenType);

            Assert.AreEqual(true, scanner.MoveToNextToken());
            Assert.AreEqual(TokenType.NUMBER, scanner.CurrentToken.TokenType);
        }
    }
}
