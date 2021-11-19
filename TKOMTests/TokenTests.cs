using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TKOM.Common;

namespace TKOMTests
{
    [TestClass]
    public class TokenTests
    {
        [TestMethod]
        public void CreateTokenCorectly()
        {
            Token token = new Token(TokenType.AND, "&&", (2, 3));

            Assert.AreEqual(TokenType.AND, token.TokenType);
            Assert.AreEqual("&&", token.Text);
            Assert.AreEqual((2,3), token.Position);
        }

        [TestMethod]
        public void GetIntValueCorectly()
        {
            Token token = new Token(TokenType.NUMBER, "123", (2, 3));

            Assert.AreEqual(123, token.GetIntValue());
        }
        
        [TestMethod]
        public void GetIntValueIncorectly()
        {
            Token token = new Token(TokenType.IDENTIFIRE, "123", (2, 3));

            Assert.AreEqual(null, token.GetIntValue());
        }
    }
}
