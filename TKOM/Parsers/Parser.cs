using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.Interfaces;
using TKOM.TreeNodes;
using TKOM.Common;

namespace TKOM.Parsers
{
    public partial class Parser : IParser
    {
        IScanner _scanner;

        public Parser(IScanner scanner)
        {
            if(scanner == null)
            {
                throw new ArgumentNullException();
            }

            _scanner = scanner;
        }

        public ProgramNode GenerateProgramTree()
        {
            while(_scanner.CurrentToken.TokenType != TokenType.EMPTY)
            {
                _scanner.MoveToNextToken();
            }

            ProgramNode program = TryToParseProgram();

            return program;
        }

        void CheckIsAndConsume(TokenType tokenType)
        {
            if (_scanner.CurrentToken.TokenType != tokenType)
            {
                // TODO : Exeption
            }
            else
            {
                _scanner.MoveToNextToken();
            }
        }

        /// <summary>
        /// Program               =   {Funkcja} ;
        /// </summary>
        /// <returns></returns>
        ProgramNode TryToParseProgram()
        {
            ProgramNode program = new ProgramNode();

            while(_scanner.CurrentToken.TokenType != TokenType.EOF)
            {
                FunctionNode function = TryToParseFunction();
                if(function == null)
                {
                    // TODO : Error - wrong token
                
                    _scanner.MoveToNextToken();
                    continue;
                }

                if(program.Functions.ContainsKey(function.Identyfire))
                {
                    // TODO : Exception
                }
                else
                {
                    program.Functions.Add(function.Identyfire, function);
                }
            }

            return program;
        }

        /// <summary>
        /// Funkcja               =   NazwaTypu identyfikator "(" [ListaParametrow] ")" InstrukcjaBlokowa;
        /// </summary>
        /// <returns></returns>
        FunctionNode TryToParseFunction()
        {
            CheckIsAndConsume(TokenType.INT);

            if(_scanner.CurrentToken.TokenType != TokenType.IDENTIFIRE)
            {
                return null;
            }
            FunctionNode function = new FunctionNode(_scanner.CurrentToken);
            _scanner.MoveToNextToken();

            CheckIsAndConsume(TokenType.BRACKET_ENTER);

            function.ParametrList = TryToParseParametrList();

            CheckIsAndConsume(TokenType.BRACKET_END);

            function.BlockInstruction = TryToParseBlockInstruction();
            if(function.BlockInstruction == null)
            {
                // TODO : Exeption
            }

            return function;
        }

        /// <summary>
        /// ListaParametrow       =   DefinicjaZmiennej {"," DefinicjaZmiennej} ;
        /// </summary>
        /// <returns></returns>
        private ParametrListNode TryToParseParametrList()
        {
            VariableDefinitionNode variable = TryToParseVariableDefinition();
            if(variable == null)
            {
                return null;
            }

            ParametrListNode parametrList = new ParametrListNode();
            parametrList.Variables.Add(variable);

            while(_scanner.CurrentToken.TokenType == TokenType.COMMA)
            {
                _scanner.MoveToNextToken();

                variable = TryToParseVariableDefinition();
                if (variable == null)
                {
                    // TODO : Exeption
                }
                else
                {
                    parametrList.Variables.Add(variable);
                }
            }

            return parametrList;
        }

        
    }
}
