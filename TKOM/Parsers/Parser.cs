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
        IErrors _errors;

        public Parser(IScanner scanner, IErrors errors)
        {
            if(scanner == null || errors == null)
            {
                throw new ArgumentNullException();
            }

            _scanner = scanner;
            _errors = errors;
        }

        public ProgramNode GenerateProgramTree()
        {
            _scanner.Restart();

            while(_scanner.CurrentToken.TokenType == TokenType.EMPTY)
            {
                _scanner.MoveToNextToken();
            }

            ProgramNode program = TryToParseProgram();

            if(_scanner.CurrentToken.TokenType != TokenType.EOF)
            {
                // Exeption
                _errors.ReportError(_scanner.CurrentToken.Position, "There is enother text in this file which cannot be parser!");
            }

            return program;
        }

        void CheckIsAndConsume(TokenType tokenType)
        {
            if (_scanner.CurrentToken.TokenType != tokenType)
            {
                //Exeption
                _errors.ReportError(_scanner.PrevToken.Position, "A token \"" + tokenType.ToString("g") + "\" is missing after this!");
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
            FunctionNode function = TryToParseFunction();
            if (function == null)
            {
                // Exeption
                _errors.ReportError((1, 0), 
                    "In program must be at least one function!");
                return null;
            }
            ProgramNode program = new ProgramNode();
            program.Functions.Add(function.Identyfire, function);

            while ((function = TryToParseFunction()) != null)
            {
                if(program.Functions.ContainsKey(function.Identyfire))
                {
                    // Exception
                    _errors.ReportError(function.IdentyfirePosition,
                        "You have enother function name like \"" + function.Identyfire + "\"!");
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
        /// ListaParametrow       =   DefinicjaZmiennej {"," DefinicjaZmiennej} ;
        /// </summary>
        /// <returns></returns>
        FunctionNode TryToParseFunction()
        {
            if(_scanner.CurrentToken.TokenType != TokenType.INT)
            {
                return null;
            }
            _scanner.MoveToNextToken();

            if(_scanner.CurrentToken.TokenType != TokenType.IDENTIFIRE)
            {
                _errors.ReportError(_scanner.CurrentToken.Position,
                    "This function must have a name hear!");
                return null;
            }

            FunctionNode function = new FunctionNode(_scanner.CurrentToken.Text, _scanner.CurrentToken.Position);
            _scanner.MoveToNextToken();

            CheckIsAndConsume(TokenType.BRACKET_ENTER);

            TryToCreateParametrList(function.ParametrList);

            CheckIsAndConsume(TokenType.BRACKET_END);

            function.BlockInstruction = TryToParseBlockInstruction();
            if(function.BlockInstruction == null)
            {
                // Exeption
                _errors.ReportError(function.IdentyfirePosition,
                    "Function \"" + function.Identyfire + "\" has to have block instruction inside");
            }

            return function;
        }

        /// <summary>
        /// ListaParametrow       =   DefinicjaZmiennej {"," DefinicjaZmiennej} ;
        /// </summary>
        /// <returns></returns>
        private void TryToCreateParametrList(List<VariableDefinitionNode> list)
        {
            VariableDefinitionNode variable = TryToParseVariableDefinition(false);
            if(variable == null)
            {
                return;
            }

            list.Add(variable);

            while(_scanner.CurrentToken.TokenType == TokenType.COMMA)
            {
                _scanner.MoveToNextToken();

                variable = TryToParseVariableDefinition(false);
                if (variable == null)
                {
                    // Exeption
                    _errors.ReportError(_scanner.PrevToken.Position,
                        "There is unnecessary ',' sign!");
                }
                else
                {
                    list.Add(variable);
                }
            }
        }

        
    }
}
