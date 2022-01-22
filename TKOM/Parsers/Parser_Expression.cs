using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.Common;
using TKOM.TreeNodes;

namespace TKOM.Parsers
{
    public partial class Parser
    {
        Expression TryToParseExpression()
        {
            return TryToParseOr();
        }

        /// <summary>
        /// Wyrazenie             =   WyrazenieI {"||" WyrazenieI} ;
        /// </summary>
        /// <returns></returns>
        Expression TryToParseOr()
        {
            Expression expression = TryToParseAnd();
            
            if (expression == null)
            {
                return null;
            }

            while (_scanner.CurrentToken.TokenType == TokenType.OR)
            {
                OrNode or = new OrNode(_scanner.CurrentToken.Position);
                _scanner.MoveToNextToken();

                or.Left = expression;
                or.Right = TryToParseAnd();
                if(or.Right == null)
                {
                    // Exeption
                    _errors.ReportError(or.Position,
                        "After '||' operator must be expression");
                }

                expression = or;
            }

            return expression;
        }

        /// <summary>
        /// WyrazenieI            =   ZaprzeczenieLogiczny {"&&" ZaprzeczenieLogiczny} ;
        /// </summary>
        /// <returns></returns>
        Expression TryToParseAnd()
        {
            Expression expression = TryToParseNot();
            
            if (expression == null)
            {
                return null;
            }

            while (_scanner.CurrentToken.TokenType == TokenType.AND)
            {
                AndNode and = new AndNode(_scanner.CurrentToken.Position);
                _scanner.MoveToNextToken();

                and.Left = expression;
                and.Right = TryToParseNot();
                if (and.Right == null)
                {
                    // Exeption
                    _errors.ReportError(and.Position,
                        "After '&&' operator must be expression");
                }

                expression = and;
            }

            return expression;
        }

        /// <summary>
        /// ZaprzeczenieLogiczny  =   ["!"] ArgumentLogiczny ;
        /// </summary>
        /// <returns></returns>
        Expression TryToParseNot()
        {
            if(_scanner.CurrentToken.TokenType == TokenType.NOT)
            {
                NotNode not = new NotNode(_scanner.CurrentToken.Position);
                _scanner.MoveToNextToken();
                not.Expression = TryToParseLogicArgument();
                if (not.Expression == null)
                {
                    // Exeption
                    _errors.ReportError(not.Position,
                        "After '!' operator must be expression");
                }
                return not;
            }
            return TryToParseLogicArgument();
        }

        /// <summary>
        /// ArgumentLogiczny      =   Porownanie
        ///                         | StalaLogiczna 
        /// </summary>
        /// <returns></returns>
        Expression TryToParseLogicArgument()
        {
            Expression expression;
            if((expression = TryToParseLogicConst()) != null
                || (expression = TryToParseCompare()) != null)
            {
                return expression;
            }
            return null;
        }

        Expression TryToParseLogicConst()
        {
            bool value;
            if (_scanner.CurrentToken.TokenType == TokenType.TRUE)
                value = true;
            else if (_scanner.CurrentToken.TokenType == TokenType.FALSE)
                value = false;
            else
                return null;

            Expression expression = new LogicConstNode(value, _scanner.CurrentToken.Position);
            _scanner.MoveToNextToken();
            return expression;
        }

        /// <summary>
        /// Porownanie            =   WyrazenieArytmetyczne [OperatorPorownania WyrazenieArytmetyczne] ;
        /// </summary>
        /// <returns></returns>
        private Expression TryToParseCompare()
        {
            Expression expression = TryToParsePlusMinus();
            if (expression == null)
            {
                return null;
            }

            NodeType nodeType;
            if (!IsCompare(_scanner.CurrentToken.TokenType, out nodeType))
            {
                return expression;
            }

            CompareNode compare = new CompareNode(nodeType, _scanner.CurrentToken.Position);
            _scanner.MoveToNextToken();

            compare.Left = expression;
            compare.Right = TryToParsePlusMinus();
            if (compare.Right == null)
            {
                // Exeption
                _errors.ReportError(compare.Position,
                        "After compare operator must be expression");
            }

            return compare;
        }

        bool IsCompare(TokenType tokenType, out NodeType nodeType)
        {
            nodeType = NodeType.Non;

            if (tokenType == TokenType.EQUAL)
                nodeType = NodeType.Equal;
            else if (tokenType == TokenType.NO_EQUAL)
                nodeType = NodeType.NoEqual;
            else if (tokenType == TokenType.MORE)
                nodeType = NodeType.More;
            else if (tokenType == TokenType.MORE_EQUAL)
                nodeType = NodeType.MoreEqual;
            else if (tokenType == TokenType.LESS)
                nodeType = NodeType.Less;
            else if (tokenType == TokenType.LESS_EQUAL)
                nodeType = NodeType.LessEqual;

            if (nodeType == NodeType.Non)
                return false;
            return true;
        }

        /// <summary>
        /// WyrazenieArytmetyczne =   WyrazeniaRazyDziel {OperatorPlusMinus WyrazeniaRazyDziel};
        /// </summary>
        /// <returns></returns>
        Expression TryToParsePlusMinus()
        {
            Expression expression = TryToParseMultiDivide();
            
            if (expression == null)
            {
                return null;
            }

            while (_scanner.CurrentToken.TokenType == TokenType.PLUS || _scanner.CurrentToken.TokenType == TokenType.MINUS)
            {
                NodeType nodeType;
                if (_scanner.CurrentToken.TokenType == TokenType.PLUS)
                    nodeType = NodeType.Plus;
                else
                    nodeType = NodeType.Minus;

                PlusMinusNode plusMinus = new PlusMinusNode(nodeType, _scanner.CurrentToken.Position);
                _scanner.MoveToNextToken();

                plusMinus.Left = expression;
                plusMinus.Right = TryToParseMultiDivide();
                if (plusMinus.Right == null)
                {
                    // Exeption
                    _errors.ReportError(plusMinus.Position,
                        "After plus-minus operator must be expression");
                }

                expression = plusMinus;
            }

            return expression;
        }


        /// <summary>
        /// WyrazeniaRazyDziel    =   OperatorUnarny {OperatorRazyDziel OperatorUnarny} ;
        /// </summary>
        /// <returns></returns>
        private Expression TryToParseMultiDivide()
        {
            Expression expression = TryToParseUnary();
            
            if (expression == null)
            {
                return null;
            }

            while (_scanner.CurrentToken.TokenType == TokenType.MULTI || _scanner.CurrentToken.TokenType == TokenType.DIVIDE)
            {
                NodeType nodeType;
                if (_scanner.CurrentToken.TokenType == TokenType.MULTI)
                    nodeType = NodeType.Multi;
                else
                    nodeType = NodeType.Divide;

                MultiDivideNode multiDivide = new MultiDivideNode(nodeType, _scanner.CurrentToken.Position);
                _scanner.MoveToNextToken();

                multiDivide.Left = expression;
                multiDivide.Right = TryToParseUnary();
                if (multiDivide.Right == null)
                {
                    // Exeption
                    _errors.ReportError(multiDivide.Position,
                        "After multi-divide operator must be expression");
                }

                expression = multiDivide;
            }

            return expression;
        }

        /// <summary>
        /// OperatorUnarny        =   ["-"] Argument ;
        /// </summary>
        /// <returns></returns>
        Expression TryToParseUnary()
        {
            if (_scanner.CurrentToken.TokenType == TokenType.MINUS)
            {
                UnaryNode unary = new UnaryNode(_scanner.CurrentToken.Position);
                _scanner.MoveToNextToken();
                unary.Expression = TryToParseArgument();
                if (unary.Expression == null)
                {
                    // Exeption
                    _errors.ReportError(unary.Position,
                        "After '-' operator must be expression");
                }
                return unary;
            }
            return TryToParseArgument();
        }

        /// <summary>
        /// Argument              =   IdentyfikatorZFunkcja
        ///                         | stala
        ///                         | "(" Wyrazenie ")" ;
        /// </summary>
        /// <returns></returns>
        Expression TryToParseArgument()
        {
            Expression expression;
            if((expression = TryToParseConst()) != null
                || (expression = TryToParseIdentyfikator()) != null
                || (expression = TryToParseBracket()) != null)
            {
                return expression;
            }
            return null;
            
        }

        Expression TryToParseConst()
        {
            if (_scanner.CurrentToken.TokenType == TokenType.NUMBER)
            {
                ConstNode con = new ConstNode((int)_scanner.CurrentToken.GetIntValue(),_scanner.CurrentToken.Position);
                _scanner.MoveToNextToken();
                return con;
            }
            return null;
        }

        Expression TryToParseBracket()
        {
            if(_scanner.CurrentToken.TokenType != TokenType.BRACKET_ENTER)
            {
                return null;
            }

            _scanner.MoveToNextToken();

            Expression expression = TryToParseExpression();
            if(expression == null)
            {
                // Exeption
                _errors.ReportError(_scanner.PrevToken.Position,
                        "Inside brackets must be expression!");
            }

            CheckIsAndConsume(TokenType.BRACKET_END);

            return expression;
        }
    }
}
