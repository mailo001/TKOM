using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.TreeNodes;
using TKOM.Common;

namespace TKOM.Parsers
{
    public partial class Parser
    {
        /// <summary>
        /// DefinicjaZmiennej     =   NazwaTypu identyfikator ["=" Wyrazenie];
        /// </summary>
        /// <returns></returns>
        private VariableDefinitionNode TryToParseVariableDefinition()
        {
            CheckIsAndConsume(TokenType.INT);

            if (_scanner.CurrentToken.TokenType != TokenType.IDENTIFIRE)
            {
                return null;
            }

            VariableDefinitionNode variable = new VariableDefinitionNode(_scanner.CurrentToken);
            _scanner.MoveToNextToken();
            if (_scanner.CurrentToken.TokenType == TokenType.ASSIGN)
            {
                _scanner.MoveToNextToken();
                variable.Value = TryToParseExpression();
                if (variable.Value == null)
                {
                    // TODO : Exeption
                }
            }

            return variable;
        }

        /// <summary>
        /// InstrukcjaBlokowa     =   "{" {Instrukcja} "}" ;
        /// </summary>
        /// <returns></returns>
        private BlockInstructionNode TryToParseBlockInstruction()
        {
            if (_scanner.CurrentToken.TokenType != TokenType.CURLY_BRACKET_ENTER)
            {
                return null;
            }
            _scanner.MoveToNextToken();

            BlockInstructionNode blockInstruction = new BlockInstructionNode();

            while(_scanner.CurrentToken.TokenType != TokenType.CURLY_BRACKET_END)
            {
                Instruction instruction = null;

                switch(_scanner.CurrentToken.TokenType)
                {
                    case TokenType.RETURN:
                        {
                            instruction = TryToParseReturn();
                            CheckIsAndConsume(TokenType.SEMICOLON);
                        }
                        break;
                    case TokenType.THROW:
                        {
                            instruction = TryToParseThrow();
                            CheckIsAndConsume(TokenType.SEMICOLON);
                        }
                        break;
                    case TokenType.INT:
                        {
                            instruction = TryToParseVariableDefinition();
                            CheckIsAndConsume(TokenType.SEMICOLON);
                        }
                        break;
                    case TokenType.IDENTIFIRE:
                        {
                            instruction = TryToParseIdentyfikator();
                            CheckIsAndConsume(TokenType.SEMICOLON);
                        }
                        break;
                    case TokenType.IF:
                        {
                            instruction = TryToParseIfElse();
                        }
                        break;
                    case TokenType.WHILE:
                        {
                            instruction = TryToParseWhile();
                        }
                        break;
                    case TokenType.TRY:
                        {
                            instruction = TryToParseTryCatch();
                        }
                        break;
                }

                if(instruction == null)
                {
                    if (_scanner.CurrentToken.TokenType == TokenType.EOF)
                    {
                        // TODO : Exeption
                        break;
                    }

                    // TODO : Exeption
                    _scanner.MoveToNextToken();
                }
                else
                {
                    blockInstruction.Instructions.Add(instruction);
                }
            }
            CheckIsAndConsume(TokenType.CURLY_BRACKET_END);

            return blockInstruction;
        }

        /// <summary>
        /// TryCatch              =   "try" InstrukcjaBlokowa CatchList ;
        /// CatchList             =   Catch {Catch};
        /// Catch                 =   "catch" "(" Wyrazenie ")" InstrukcjaBlokowa ;
        /// </summary>
        /// <returns></returns>
        private Instruction TryToParseTryCatch()
        {
            if (_scanner.CurrentToken.TokenType != TokenType.TRY)
            {
                return null;
            }
            _scanner.MoveToNextToken();

            TryCatchNode tryCatch = new TryCatchNode();

            tryCatch.TryBlock = TryToParseBlockInstruction();

            while(_scanner.CurrentToken.TokenType == TokenType.CATCH)
            {
                _scanner.MoveToNextToken();

                CheckIsAndConsume(TokenType.BRACKET_ENTER);

                Expression condition = TryToParseExpression();
                if(condition == null)
                {
                    // TODO : Exeption
                }

                CheckIsAndConsume(TokenType.BRACKET_END);

                BlockInstructionNode block = TryToParseBlockInstruction();
                if(block == null)
                {
                    // TODO : Exeption
                }

                tryCatch.CatchList.Add((condition, block));
            }

            return tryCatch;
        }

        /// <summary>
        /// While                 =   "while" "(" Wyrazenie ")" InstrukcjaBlokowa ;
        /// </summary>
        /// <returns></returns>
        private Instruction TryToParseWhile()
        {
            if (_scanner.CurrentToken.TokenType != TokenType.WHILE)
            {
                return null;
            }
            _scanner.MoveToNextToken();

            WhileNode whileNode = new WhileNode();

            CheckIsAndConsume(TokenType.BRACKET_ENTER);

            whileNode.Condition = TryToParseExpression();
            if(whileNode.Condition == null)
            {
                // TODO : Exeption
            }

            CheckIsAndConsume(TokenType.BRACKET_END);

            whileNode.Block = TryToParseBlockInstruction();
            if (whileNode.Condition == null)
            {
                // TODO : Exeption
            }

            return whileNode;
        }

        /// <summary>
        /// IfElse                =   "if" "(" Wyrazenie ")" InstrukcjaBlokowa [Else];
        /// Else                  =   "else" InstrukcjaBlokowa ;
        /// </summary>
        /// <returns></returns>
        private Instruction TryToParseIfElse()
        {
            if(_scanner.CurrentToken.TokenType != TokenType.IF)
            {
                return null;
            }
            _scanner.MoveToNextToken();

            IfElseNode ifElse = new IfElseNode();

            CheckIsAndConsume(TokenType.BRACKET_ENTER);

            ifElse.Condition = TryToParseExpression();
            if(ifElse.Condition == null)
            {
                // TODO : Exeption
            }

            CheckIsAndConsume(TokenType.BRACKET_END);

            ifElse.IfBlock = TryToParseBlockInstruction();
            if (ifElse.Condition == null)
            {
                // TODO : Exeption
            }

            if(_scanner.CurrentToken.TokenType == TokenType.ELSE)
            {
                _scanner.MoveToNextToken();

                ifElse.ElseBlock = TryToParseBlockInstruction();
                if (ifElse.Condition == null)
                {
                    // TODO : Exeption
                }
            }

            return ifElse;
        }

        /// <summary>
        /// IdentyfikatorZFunkcja =   identyfikator
        ///                         | identyfikator "=" Wyrazenie
        ///                         | identyfikator "(" [ListaArgumentow] ")" ;
        /// </summary>
        /// <returns></returns>
        private Expression TryToParseIdentyfikator()
        {
            if (_scanner.CurrentToken.TokenType != TokenType.IDENTIFIRE)
            {
                return null;
            }
            Token identyfire = _scanner.CurrentToken;
            _scanner.MoveToNextToken();

            if(_scanner.CurrentToken.TokenType == TokenType.ASSIGN)
            {
                _scanner.MoveToNextToken();

                AssigmentNode assigmentNode = new AssigmentNode(identyfire);
                assigmentNode.Expression = TryToParseExpression();
                if(assigmentNode.Expression == null)
                {
                    // TODO : Exeption
                }
                return assigmentNode;
            }
            if(_scanner.CurrentToken.TokenType == TokenType.BRACKET_ENTER)
            {
                FunctionInvocationNode functionInvocation = new FunctionInvocationNode(identyfire);

                do
                {
                    _scanner.MoveToNextToken();
                    if (_scanner.CurrentToken.TokenType == TokenType.BRACKET_END)
                    {
                        _scanner.MoveToNextToken();
                        return functionInvocation;
                    }
                    Expression expression = TryToParseExpression();
                    if (expression == null)
                    {
                        // TODO : Exeption
                    }
                    else
                    {
                        functionInvocation.Arguments.Add(expression);
                    }
                } while (_scanner.CurrentToken.TokenType == TokenType.COMMA);

                CheckIsAndConsume(TokenType.BRACKET_END);
                return functionInvocation;
            }

            VariableNode variable = new VariableNode(identyfire);
            return variable;
        }

        /// <summary>
        /// Throw                 =   "throw" Wyrazenie ;
        /// </summary>
        /// <returns></returns>
        private Instruction TryToParseThrow()
        {
            if (_scanner.CurrentToken.TokenType != TokenType.THROW)
            {
                return null;
            }
            _scanner.MoveToNextToken();

            ThrowNode throwNode = new ThrowNode();
            throwNode.Value = TryToParseExpression();
            if (throwNode.Value == null)
            {
                // TODO : Exeption
            }

            return throwNode;
        }

        /// <summary>
        /// Return                =   "return" Wyrazenie ;
        /// </summary>
        /// <returns></returns>
        private Instruction TryToParseReturn()
        {
            if(_scanner.CurrentToken.TokenType != TokenType.RETURN)
            {
                return null;
            }
            _scanner.MoveToNextToken();

            ReturnNode returnNode = new ReturnNode();
            returnNode.Value = TryToParseExpression();
            if(returnNode.Value == null)
            {
                // TODO : Exeption
            }

            return returnNode;
        }
    }
}
