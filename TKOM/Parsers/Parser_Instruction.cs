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
        private VariableDefinitionNode TryToParseVariableDefinition(bool assigment = true)
        {
            if(_scanner.CurrentToken.TokenType != TokenType.INT)
            {
                return null;
            }
            _scanner.MoveToNextToken();

            if (_scanner.CurrentToken.TokenType != TokenType.IDENTIFIRE)
            {
                // Exeption
                _errors.ReportError(_scanner.PrevToken.Position,
                    "Variable must have name after int!");
                return null;
            }

            VariableDefinitionNode variable = new VariableDefinitionNode(_scanner.CurrentToken.Text, _scanner.CurrentToken.Position);
            _scanner.MoveToNextToken();
            if (_scanner.CurrentToken.TokenType == TokenType.ASSIGN)
            {
                if(!assigment)
                {
                    // Exeption
                    _errors.ReportError(_scanner.CurrentToken.Position,
                        "You cannot assigment variable in function definition!");
                }

                _scanner.MoveToNextToken();
                variable.Value = TryToParseExpression();
                if (variable.Value == null)
                {
                    // Exeption
                    _errors.ReportError(_scanner.PrevToken.Position,
                        "Expression is need after \"=\"!");
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

            Instruction instruction;
            while ((instruction = TryToParseInstruction()) != null)
            { 
                blockInstruction.Instructions.Add(instruction);
            }

            CheckIsAndConsume(TokenType.CURLY_BRACKET_END);

            return blockInstruction;
        }

        Instruction TryToParseInstruction()
        {
            Instruction instruction;
            if((instruction = TryToParseReturn()) != null
                || (instruction = TryToParseThrow()) != null
                || (instruction = TryToParseVariableDefinition()) != null
                || (instruction = TryToParseIdentyfikator()) != null)
            {
                CheckIsAndConsume(TokenType.SEMICOLON);
                return instruction;
            }
            if((instruction = TryToParseIfElse()) != null
                || (instruction = TryToParseTryCatch()) != null
                || (instruction = TryToParseWhile()) != null)
            {
                return instruction;
            }
            return null;
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
            if(tryCatch.TryBlock == null)
            {
                // Exeption
                _errors.ReportError(_scanner.PrevToken.Position,
                    "TryCatch instruction has to have block instruction there!");
            }

            while(_scanner.CurrentToken.TokenType == TokenType.CATCH)
            { 
                _scanner.MoveToNextToken();

                CheckIsAndConsume(TokenType.BRACKET_ENTER);

                Expression condition = TryToParseExpression();
                if(condition == null)
                {
                    // Exeption
                    _errors.ReportError(_scanner.PrevToken.Position,
                    "Catch instruction has to have conditions in brackets!");
                }

                CheckIsAndConsume(TokenType.BRACKET_END);

                BlockInstructionNode block = TryToParseBlockInstruction();
                if(block == null)
                {
                    // Exeption
                    _errors.ReportError(_scanner.PrevToken.Position,
                    "Catch instruction has to have block instruction after this");
                }

                tryCatch.CatchList.Add((condition, block));
            }
            if(tryCatch.CatchList.Count == 0)
            {
                // Exeption
                _errors.ReportError(_scanner.PrevToken.Position,
                    "TryCatch instruction has to have one Catch block after this");
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
                // Exeption
                _errors.ReportError(_scanner.PrevToken.Position,
                    "While instruction has to have condition inside brackets!");
            }

            CheckIsAndConsume(TokenType.BRACKET_END);

            whileNode.Block = TryToParseBlockInstruction();
            if (whileNode.Block == null)
            {
                // Exeption
                _errors.ReportError(_scanner.PrevToken.Position,
                    "While instruction has to have block instruction after this!");
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
                // Exeption
                _errors.ReportError(_scanner.PrevToken.Position,
                    "IF instruction has to have condition inside brackets!");
            }

            CheckIsAndConsume(TokenType.BRACKET_END);

            ifElse.IfBlock = TryToParseBlockInstruction();
            if (ifElse.IfBlock == null)
            {
                // Exeption
                _errors.ReportError(_scanner.PrevToken.Position,
                    "IF instruction has to have block instructionafter this!");
            }

            if(_scanner.CurrentToken.TokenType == TokenType.ELSE)
            {
                _scanner.MoveToNextToken();

                ifElse.ElseBlock = TryToParseBlockInstruction();
                if (ifElse.Condition == null)
                {
                    // Exeption
                    _errors.ReportError(_scanner.PrevToken.Position,
                        "After ELSE instruction must be block instruction after this!");
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

                AssigmentNode assigmentNode = new AssigmentNode(identyfire.Text, identyfire.Position);
                assigmentNode.Expression = TryToParseExpression();
                if(assigmentNode.Expression == null)
                {
                    // Exeption
                    _errors.ReportError(_scanner.PrevToken.Position,
                        "In assigment instruction after '=' sign must be expression!");
                }
                return assigmentNode;
            }
            if(_scanner.CurrentToken.TokenType == TokenType.BRACKET_ENTER)
            {
                _scanner.MoveToNextToken();

                FunctionInvocationNode functionInvocation = new FunctionInvocationNode(identyfire.Text, identyfire.Position);

                Expression expression = TryToParseExpression();
                if(expression == null)
                {
                    CheckIsAndConsume(TokenType.BRACKET_END);
                    return functionInvocation;
                }

                functionInvocation.Arguments.Add(expression);

                while(_scanner.CurrentToken.TokenType == TokenType.COMMA)
                {
                    _scanner.MoveToNextToken();
                    expression = TryToParseExpression();
                    if (expression == null)
                    {
                        // Exeption
                        _errors.ReportError(_scanner.PrevToken.Position,
                            "After ',' must be expression in function invocation!");
                    }
                    else
                    {
                        functionInvocation.Arguments.Add(expression);
                    }
                }

                CheckIsAndConsume(TokenType.BRACKET_END);
                return functionInvocation;
            }

            VariableNode variable = new VariableNode(identyfire.Text, identyfire.Position);
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
                // Exeption
                _errors.ReportError(_scanner.PrevToken.Position,
                    "In THROW instruction must be expression!");
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
                // Exeption
                _errors.ReportError(_scanner.PrevToken.Position,
                    "In Return instruction must be expression!");
            }

            return returnNode;

            
        }
    }
}
