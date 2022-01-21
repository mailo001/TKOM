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
using TKOM.Errors;
using TKOM.Visitors;

namespace TKOMTests
{
    [TestClass]
    public class InterpreterTest
    {
        [TestMethod]
        public void InitInterpreterCorectly()
        {
            try
            {
                Interpreter interpreter = new Interpreter();
                Assert.IsNotNull(interpreter);
            }
            catch
            {
                Assert.Fail("No Exeption is correct");
            }
        }

        [TestMethod]
        public void InterpreteSimpleTree()
        {
            ErrorsCollector errors = new ErrorsCollector();
            ICharReader charReader = new StringReader("int main() { int a; return 0; }");
            IScanner scanner = new Scanner(charReader, errors);
            Parser parser = new Parser(scanner, errors);

            ProgramNode program = parser.GenerateProgramTree();

            Assert.AreEqual(0, errors.ErrorsList.Count);

            Interpreter interpreter = new Interpreter();

            try
            {
                var result = interpreter.InterpreteProgramTree(program);
                Assert.AreEqual((0, null), result);
            }
            catch
            {
                Assert.Fail("No Exeption is correct");
            }
        }

        [TestMethod]
        public void InterpreteFunctionInvocationCorrectly()
        {
            ErrorsCollector errors = new ErrorsCollector();
            ICharReader charReader = new StringReader("int main() { return abc(); } int abc() { return 1; }");
            IScanner scanner = new Scanner(charReader, errors);
            Parser parser = new Parser(scanner, errors);

            ProgramNode program = parser.GenerateProgramTree();

            Assert.AreEqual(0, errors.ErrorsList.Count);

            Interpreter interpreter = new Interpreter();

            try
            {
                var result = interpreter.InterpreteProgramTree(program);
                Assert.AreEqual((1, null), result);
            }
            catch(InterpreterExeption e)
            {
                Assert.Fail("No Exeption is correct");
            }
            catch(Exception e)
            {
                Assert.Fail("No Exeption is correct");
            }
        }

        [TestMethod]
        public void InterpreteFunctionInvocationWithArgument()
        {
            ErrorsCollector errors = new ErrorsCollector();
            ICharReader charReader = new StringReader("int main() { int a = 1; return abc(a, 2); } int abc(int a, int b) { return a + b; }");
            IScanner scanner = new Scanner(charReader, errors);
            Parser parser = new Parser(scanner, errors);

            ProgramNode program = parser.GenerateProgramTree();

            Assert.AreEqual(0, errors.ErrorsList.Count);

            Interpreter interpreter = new Interpreter();

            try
            {
                var result = interpreter.InterpreteProgramTree(program);
                Assert.AreEqual((3, null), result);
            }
            catch (InterpreterExeption e)
            {
                Assert.Fail("No Exeption is correct");
            }
            catch (Exception e)
            {
                Assert.Fail("No Exeption is correct");
            }
        }

        [TestMethod]
        public void InterpretePrintFunction()
        {
            ErrorsCollector errors = new ErrorsCollector();
            ICharReader charReader = new StringReader("int main() { int a = print(1, 2, 3); return a; }");
            IScanner scanner = new Scanner(charReader, errors);
            Parser parser = new Parser(scanner, errors);

            ProgramNode program = parser.GenerateProgramTree();

            Assert.AreEqual(0, errors.ErrorsList.Count);

            Interpreter interpreter = new Interpreter();

            try
            {
                var result = interpreter.InterpreteProgramTree(program);
                Assert.AreEqual((0, null), result);
            }
            catch (InterpreterExeption e)
            {
                Assert.Fail("No Exeption is correct");
            }
            catch (Exception e)
            {
                Assert.Fail("No Exeption is correct");
            }
        }

        [TestMethod]
        public void InterpreteThrowCorrectly()
        {
            ErrorsCollector errors = new ErrorsCollector();
            ICharReader charReader = new StringReader("int main() { try { abc(); } catch (Exception == 1) { return 1; } return 0; } int abc() { if(true) {throw 1; return 1; } return 0; }");
            IScanner scanner = new Scanner(charReader, errors);
            Parser parser = new Parser(scanner, errors);

            ProgramNode program = parser.GenerateProgramTree();

            Assert.AreEqual(0, errors.ErrorsList.Count);

            Interpreter interpreter = new Interpreter();

            try
            {
                var result = interpreter.InterpreteProgramTree(program);
                Assert.AreEqual((1, null), result);
            }
            catch
            {
                Assert.Fail("No Exeption is correct");
            }
        }

        [TestMethod]
        public void InterpreteVariableSameNameDef()
        {
            ErrorsCollector errors = new ErrorsCollector();
            ICharReader charReader = new StringReader("int main() { int a = 1; while(a = 1) { int a; return 1;} return 0; }");
            IScanner scanner = new Scanner(charReader, errors);
            Parser parser = new Parser(scanner, errors);

            ProgramNode program = parser.GenerateProgramTree();

            Assert.AreEqual(0, errors.ErrorsList.Count);

            Interpreter interpreter = new Interpreter();

            try
            {
                var result = interpreter.InterpreteProgramTree(program);
                Assert.Fail("Exeption is need");
            }
            catch (InterpreterExeption e)
            {
                Assert.AreEqual("Variable \"a\" exist in this scope!", e.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("No Exeption is correct");
            }
        }

        [TestMethod]
        public void InterpreteUndefineVariableAssigment()
        {
            ErrorsCollector errors = new ErrorsCollector();
            ICharReader charReader = new StringReader("int main() { a = 2; }");
            IScanner scanner = new Scanner(charReader, errors);
            Parser parser = new Parser(scanner, errors);

            ProgramNode program = parser.GenerateProgramTree();

            Assert.AreEqual(0, errors.ErrorsList.Count);

            Interpreter interpreter = new Interpreter();

            try
            {
                var result = interpreter.InterpreteProgramTree(program);
                Assert.Fail("Exeption is need");
            }
            catch (InterpreterExeption e)
            {
                Assert.AreEqual("Variable \"a\" does not exist in this scope!", e.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("No Exeption is correct");
            }
        }


        [TestMethod]
        public void InterpreteUndefineVariable()
        {
            ErrorsCollector errors = new ErrorsCollector();
            ICharReader charReader = new StringReader("int main() { int a = 2 * b; }");
            IScanner scanner = new Scanner(charReader, errors);
            Parser parser = new Parser(scanner, errors);

            ProgramNode program = parser.GenerateProgramTree();

            Assert.AreEqual(0, errors.ErrorsList.Count);

            Interpreter interpreter = new Interpreter();

            try
            {
                var result = interpreter.InterpreteProgramTree(program);
                Assert.Fail("Exeption is need");
            }
            catch (InterpreterExeption e)
            {
                Assert.AreEqual("Variable \"b\" does not exist in this scope!", e.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("No Exeption is correct");
            }
        }

        [TestMethod]
        public void InterpreteFunctionInvocationWithWrongParametrList()
        {
            ErrorsCollector errors = new ErrorsCollector();
            ICharReader charReader = new StringReader("int main() { return abc(); } int abc(int a) {return a;}");
            IScanner scanner = new Scanner(charReader, errors);
            Parser parser = new Parser(scanner, errors);

            ProgramNode program = parser.GenerateProgramTree();

            Assert.AreEqual(0, errors.ErrorsList.Count);

            Interpreter interpreter = new Interpreter();

            try
            {
                var result = interpreter.InterpreteProgramTree(program);
                Assert.Fail("Exeption is need");
            }
            catch (InterpreterExeption e)
            {
                Assert.AreEqual("Function \"abc\" has different number of argument!", e.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("No Exeption is correct");
            }
        }

        [TestMethod]
        public void InterpreteFunctionWrongName()
        {
            ErrorsCollector errors = new ErrorsCollector();
            ICharReader charReader = new StringReader("int main() { return ab(); } int abc() {return a;}");
            IScanner scanner = new Scanner(charReader, errors);
            Parser parser = new Parser(scanner, errors);

            ProgramNode program = parser.GenerateProgramTree();

            Assert.AreEqual(0, errors.ErrorsList.Count);

            Interpreter interpreter = new Interpreter();

            try
            {
                var result = interpreter.InterpreteProgramTree(program);
                Assert.Fail("Exeption is need");
            }
            catch (InterpreterExeption e)
            {
                Assert.AreEqual("Program dose not implement \"ab\" function!", e.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("No Exeption is correct");
            }
        }

    }
}
