using System;

using TKOM.CharReaders;
using TKOM.Scanners;
using TKOM.Parsers;
using TKOM.Errors;
using TKOM.TreeNodes;
using TKOM.Visitors;

namespace TKOM
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start Debug program.txt");
            Console.WriteLine();

            // File reader
            FileReader reader = new FileReader("./Files/program.txt");

            // Scaner
            ErrorsCollector scannerError = new ErrorsCollector();
            scannerError.AddSource(reader);
            Scanner scanner = new Scanner(reader, scannerError);

            // Get all token and check errors
            int tokenNum = 0;
            while(scanner.MoveToNextToken())
            {
                tokenNum++;
            }
            Console.WriteLine("Total token number: " + tokenNum.ToString());
            Console.WriteLine();

            if(scannerError.ErrorsList.Count == 0)
            {
                Console.WriteLine("Succec: No scaner error!");
            }
            else
            {
                Console.WriteLine("Fail: There is " + scannerError.ErrorsList.Count + " scaner error!");
                Console.WriteLine();
                scannerError.PrintErrors();
                Console.WriteLine();
                Console.WriteLine("Correct and run again!");
                return;
            }

            // Parser
            Console.WriteLine();

            scanner.Restart();
            ErrorsCollector parserError = new ErrorsCollector();
            parserError.AddSource(reader);
            Parser parser = new Parser(scanner, parserError);

            // Generate tree and check errors
            ProgramNode program = parser.GenerateProgramTree();

            if (parserError.ErrorsList.Count == 0)
            {
                Console.WriteLine("Succec: No parser error!");
            }
            else
            {
                Console.WriteLine("Fail: There is " + parserError.ErrorsList.Count + " parser error!");
                Console.WriteLine();
                parserError.PrintErrors();
                Console.WriteLine();
                Console.WriteLine("Correct and run again!");
                return;
            }

            Console.WriteLine();

            Interpreter interpreter = new Interpreter();

            Console.WriteLine("Start program");
            (int?, int?) result;
            try
            {
                result = interpreter.InterpreteProgramTree(program);
            }
            catch(InterpreterExeption e)
            {
                Console.WriteLine("Program fail. There is error inside!");
                Console.WriteLine(e.Show(reader));
                Console.WriteLine("Correct and run again!");
                return;
            }
            catch(Exception e)
            {
                Console.WriteLine("System error!");
                Console.WriteLine(e.Message);
                Console.WriteLine("Correct and run again!");
                return;
            }
            Console.WriteLine("Program end");
            Console.WriteLine();
            if(result.Item2 != null)
            {
                Console.WriteLine("Program throw uncatch exeption: " + result.Item2.Value.ToString());
            }
            else
            {
                Console.WriteLine("Program return value: " + result.Item1.Value.ToString());
            }
            
        }


    }
}
