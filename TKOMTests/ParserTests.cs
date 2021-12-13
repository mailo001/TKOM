using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TKOM.CharReaders;
using TKOM.Interfaces;
using TKOM.Scanners;
using TKOM.Parsers;
using TKOM.TreeNodes;
using TKOM.Common;

namespace TKOMTests
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public void InitParserCorectly()
        {
            ICharReader charReader = new StringReader("abc");
            IScanner scanner = new Scanner(charReader);
            try
            {
                Parser parser = new Parser(scanner);
                Assert.IsNotNull(parser);
            }
            catch
            {
                Assert.Fail("No Exeption is correct");
            }
        }

        [TestMethod]
        public void CreateSimpleTree()
        {
            ICharReader charReader = new StringReader("int main() { int a; }");
            IScanner scanner = new Scanner(charReader);
            Parser parser = new Parser(scanner);

            ProgramNode program = parser.GenerateProgramTree();

            Assert.AreEqual(1, program.Functions.Count);

            Assert.AreEqual(true, program.Functions.ContainsKey("main"));

            if(program.Functions.ContainsKey("main"))
            {
                FunctionNode func = program.Functions["main"];

                Assert.IsNull(func.ParametrList);

                Assert.IsNotNull(func.BlockInstruction);

                Assert.AreEqual(1, func.BlockInstruction.Instructions.Count);

                Assert.AreEqual(NodeType.VariableDefinition, func.BlockInstruction.Instructions[0].NodeType);
            }
        }

        [TestMethod]
        public void ManyFunction()
        {
            ICharReader charReader = new StringReader("int main() { int a; } int add(int a, int b) { return a + b; }");
            IScanner scanner = new Scanner(charReader);
            Parser parser = new Parser(scanner);

            ProgramNode program = parser.GenerateProgramTree();

            Assert.AreEqual(2, program.Functions.Count);

            Assert.AreEqual(true, program.Functions.ContainsKey("main"));

            if (program.Functions.ContainsKey("main"))
            {
                FunctionNode func = program.Functions["main"];

                Assert.IsNull(func.ParametrList);

                Assert.IsNotNull(func.BlockInstruction);

                Assert.AreEqual(1, func.BlockInstruction.Instructions.Count);

                Assert.AreEqual(NodeType.VariableDefinition, func.BlockInstruction.Instructions[0].NodeType);
            }

            Assert.AreEqual(true, program.Functions.ContainsKey("add"));

            if (program.Functions.ContainsKey("add"))
            {
                FunctionNode func = program.Functions["add"];

                Assert.IsNotNull(func.ParametrList);

                Assert.AreEqual(2, func.ParametrList.Variables.Count);

                Assert.IsNotNull(func.BlockInstruction);

                Assert.AreEqual(1, func.BlockInstruction.Instructions.Count);

                Assert.AreEqual(NodeType.Return, func.BlockInstruction.Instructions[0].NodeType);
            }
        }

        [TestMethod]
        public void Expression()
        {
            ICharReader charReader = new StringReader("int main() { int a; a = 1 + 2 - 4 * (3 - 1); }");
            IScanner scanner = new Scanner(charReader);
            Parser parser = new Parser(scanner);

            ProgramNode program = parser.GenerateProgramTree();

            Assert.AreEqual(1, program.Functions.Count);

            Assert.AreEqual(true, program.Functions.ContainsKey("main"));

            if (program.Functions.ContainsKey("main"))
            {
                FunctionNode func = program.Functions["main"];

                Assert.IsNull(func.ParametrList);

                Assert.IsNotNull(func.BlockInstruction);

                Assert.AreEqual(2, func.BlockInstruction.Instructions.Count);

                Assert.AreEqual(NodeType.VariableDefinition, func.BlockInstruction.Instructions[0].NodeType);

                Assert.AreEqual(NodeType.Assigment, func.BlockInstruction.Instructions[1].NodeType);

                AssigmentNode assigment = func.BlockInstruction.Instructions[1] as AssigmentNode;

                Assert.IsNotNull(assigment);
                if(assigment != null)
                {
                    Assert.IsNotNull(assigment.Expression);

                    Assert.AreEqual(NodeType.PlusMinus, assigment.Expression.NodeType);

                    Assert.AreEqual(TokenType.MINUS, assigment.Expression.Token.TokenType);
                }
            }
        }

        [TestMethod]
        public void IfElse()
        {
            ICharReader charReader = new StringReader("int main() { if(a > b) { throw 2; } else { func(); } }");
            IScanner scanner = new Scanner(charReader);
            Parser parser = new Parser(scanner);

            ProgramNode program = parser.GenerateProgramTree();

            Assert.AreEqual(1, program.Functions.Count);

            Assert.AreEqual(true, program.Functions.ContainsKey("main"));

            if (program.Functions.ContainsKey("main"))
            {
                FunctionNode func = program.Functions["main"];

                Assert.IsNull(func.ParametrList);

                Assert.IsNotNull(func.BlockInstruction);

                Assert.AreEqual(1, func.BlockInstruction.Instructions.Count);

                Assert.AreEqual(NodeType.IfElse, func.BlockInstruction.Instructions[0].NodeType);

                IfElseNode ifelse = func.BlockInstruction.Instructions[0] as IfElseNode;

                Assert.IsNotNull(ifelse);
                if (ifelse != null)
                {
                    Assert.IsNotNull(ifelse.Condition);

                    Assert.AreEqual(NodeType.Compare, ifelse.Condition.NodeType);

                    Assert.IsNotNull(ifelse.IfBlock);

                    Assert.AreEqual(1, ifelse.IfBlock.Instructions.Count);

                    Assert.AreEqual(NodeType.Throw, ifelse.IfBlock.Instructions[0].NodeType);

                    Assert.IsNotNull(ifelse.ElseBlock);

                    Assert.AreEqual(1, ifelse.ElseBlock.Instructions.Count);

                    Assert.AreEqual(NodeType.FunctionInvocation, ifelse.ElseBlock.Instructions[0].NodeType);

                }
            }
        }

        [TestMethod]
        public void WithoutBracket()
        {
            ICharReader charReader = new StringReader("int main() { int a; ");
            IScanner scanner = new Scanner(charReader);
            Parser parser = new Parser(scanner);

            ProgramNode program = parser.GenerateProgramTree();

            Assert.AreEqual(1, program.Functions.Count);

            Assert.AreEqual(true, program.Functions.ContainsKey("main"));

            if (program.Functions.ContainsKey("main"))
            {
                FunctionNode func = program.Functions["main"];

                Assert.IsNull(func.ParametrList);

                Assert.IsNotNull(func.BlockInstruction);

                Assert.AreEqual(1, func.BlockInstruction.Instructions.Count);

                Assert.AreEqual(NodeType.VariableDefinition, func.BlockInstruction.Instructions[0].NodeType);
            }
        }


        [TestMethod]
        public void TryCatch()
        {
            ICharReader charReader = new StringReader("int main() " +
                "{ " +
                    "try { throw 2; } " +
                    "catch (a && 2) { int b = 3; } " +
                    "catch (x > 1) { int a = 2; } " +
                "}");
            IScanner scanner = new Scanner(charReader);
            Parser parser = new Parser(scanner);

            ProgramNode program = parser.GenerateProgramTree();

            Assert.AreEqual(1, program.Functions.Count);

            Assert.AreEqual(true, program.Functions.ContainsKey("main"));

            if (program.Functions.ContainsKey("main"))
            {
                FunctionNode func = program.Functions["main"];

                Assert.IsNull(func.ParametrList);

                Assert.IsNotNull(func.BlockInstruction);

                Assert.AreEqual(1, func.BlockInstruction.Instructions.Count);

                Assert.AreEqual(NodeType.TryCatch, func.BlockInstruction.Instructions[0].NodeType);

                TryCatchNode tryCatch = func.BlockInstruction.Instructions[0] as TryCatchNode;

                Assert.IsNotNull(tryCatch);
                if (tryCatch != null)
                {
                    Assert.IsNotNull(tryCatch.TryBlock);

                    Assert.AreEqual(1, tryCatch.TryBlock.Instructions.Count);

                    Assert.AreEqual(NodeType.Throw, tryCatch.TryBlock.Instructions[0].NodeType);

                    Assert.IsNotNull(tryCatch.CatchList);

                    Assert.AreEqual(2, tryCatch.CatchList.Count);

                    Assert.IsNotNull(tryCatch.CatchList[0].Item1);

                    Assert.AreEqual(NodeType.And, tryCatch.CatchList[0].Item1.NodeType);

                    Assert.IsNotNull(tryCatch.CatchList[0].Item2);

                    Assert.AreEqual(1, tryCatch.CatchList[0].Item2.Instructions.Count);

                    Assert.AreEqual(NodeType.VariableDefinition, tryCatch.CatchList[0].Item2.Instructions[0].NodeType);

                }
            }
        }

        [TestMethod]
        public void WhileNode()
        {
            ICharReader charReader = new StringReader("int main() { while (a = 2) { func(); } }");
            IScanner scanner = new Scanner(charReader);
            Parser parser = new Parser(scanner);

            ProgramNode program = parser.GenerateProgramTree();

            Assert.AreEqual(1, program.Functions.Count);

            Assert.AreEqual(true, program.Functions.ContainsKey("main"));

            if (program.Functions.ContainsKey("main"))
            {
                FunctionNode func = program.Functions["main"];

                Assert.IsNull(func.ParametrList);

                Assert.IsNotNull(func.BlockInstruction);

                Assert.AreEqual(1, func.BlockInstruction.Instructions.Count);

                Assert.AreEqual(NodeType.While, func.BlockInstruction.Instructions[0].NodeType);

                WhileNode whileNode = func.BlockInstruction.Instructions[0] as WhileNode;

                Assert.IsNotNull(whileNode);
                if (whileNode != null)
                {
                    Assert.IsNotNull(whileNode.Condition);

                    Assert.AreEqual(NodeType.Assigment, whileNode.Condition.NodeType);

                    Assert.IsNotNull(whileNode.Block);

                    Assert.AreEqual(1, whileNode.Block.Instructions.Count);

                    Assert.AreEqual(NodeType.FunctionInvocation, whileNode.Block.Instructions[0].NodeType);

                }
            }
        }
    }
}
