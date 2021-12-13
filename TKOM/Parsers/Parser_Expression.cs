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
            if (_scanner.CurrentToken.TokenType != TokenType.OR)
            {
                return expression;
            }
            if (expression == null)
            {
                // TODO : Exeption
            }

            while (_scanner.CurrentToken.TokenType == TokenType.OR)
            {
                OrNode or = new OrNode(_scanner.CurrentToken);
                _scanner.MoveToNextToken();

                or.Left = expression;
                or.Right = TryToParseAnd();
                if(or.Right == null)
                {
                    // TODO : Exeption
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
            if (_scanner.CurrentToken.TokenType != TokenType.AND)
            {
                return expression;
            }
            if (expression == null)
            {
                // TODO : Exeption
            }

            while (_scanner.CurrentToken.TokenType == TokenType.AND)
            {
                AndNode and = new AndNode(_scanner.CurrentToken);
                _scanner.MoveToNextToken();

                and.Left = expression;
                and.Right = TryToParseNot();
                if (and.Right == null)
                {
                    // TODO : Exeption
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
                NotNode not = new NotNode(_scanner.CurrentToken);
                _scanner.MoveToNextToken();
                not.Expression = TryToParseLogicArgument();
                if (not.Expression == null)
                {
                    // TODO : Exeption
                }
                return not;
            }
            return TryToParseLogicArgument();
        }

        /// <summary>
        /// ArgumentLogiczny      =   Porownanie
        ///                         | StalaLogiczna 
        ///                         | "(" Wyrazenie ")";
        /// </summary>
        /// <returns></returns>
        Expression TryToParseLogicArgument()
        {
            if(_scanner.CurrentToken.TokenType == TokenType.TRUE || _scanner.CurrentToken.TokenType == TokenType.FALSE)
            {
                return TryToParseLogicConst();
            }
            if(_scanner.CurrentToken.TokenType == TokenType.BRACKET_ENTER)
            {
                return TryToParseBracket();
            }
            return TryToParseComapre();
        }

        Expression TryToParseLogicConst()
        {
            if (_scanner.CurrentToken.TokenType == TokenType.TRUE || _scanner.CurrentToken.TokenType == TokenType.FALSE)
            {
                Expression expression = new LogicConstNode(_scanner.CurrentToken);
                _scanner.MoveToNextToken();
                return expression;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Porownanie            =   WyrazenieArytmetyczne [OperatorPorownania WyrazenieArytmetyczne] ;
        /// </summary>
        /// <returns></returns>
        private Expression TryToParseComapre()
        {
            Expression expression = TryToParsePlusMinus();
            if (!IsCompare(_scanner.CurrentToken.TokenType))
            {
                return expression;
            }
            if (expression == null)
            {
                // TODO : Exeption
            }

            CompareNode compare = new CompareNode(_scanner.CurrentToken);
            _scanner.MoveToNextToken();

            compare.Left = expression;
            compare.Right = TryToParsePlusMinus();
            if (compare.Right == null)
            {
                // TODO : Exeption
            }

            return compare;
        }

        bool IsCompare(TokenType tokenType)
        {
            return tokenType == TokenType.EQUAL
                || tokenType == TokenType.NO_EQUAL
                || tokenType == TokenType.MORE
                || tokenType == TokenType.MORE_EQUAL
                || tokenType == TokenType.LESS
                || tokenType == TokenType.LESS_EQUAL;
        }

        /// <summary>
        /// WyrazenieArytmetyczne =   WyrazeniaRazyDziel {OperatorPlusMinus WyrazeniaRazyDziel};
        /// </summary>
        /// <returns></returns>
        Expression TryToParsePlusMinus()
        {
            Expression expression = TryToParseMultiDivide();
            if (_scanner.CurrentToken.TokenType != TokenType.PLUS && _scanner.CurrentToken.TokenType != TokenType.MINUS)
            {
                return expression;
            }
            if (expression == null)
            {
                // TODO : Exeption
            }

            while (_scanner.CurrentToken.TokenType == TokenType.PLUS || _scanner.CurrentToken.TokenType == TokenType.MINUS)
            {
                PlusMinusNode plusMinus = new PlusMinusNode(_scanner.CurrentToken);
                _scanner.MoveToNextToken();

                plusMinus.Left = expression;
                plusMinus.Right = TryToParseMultiDivide();
                if (plusMinus.Right == null)
                {
                    // TODO : Exeption
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
            if (_scanner.CurrentToken.TokenType != TokenType.MULTI && _scanner.CurrentToken.TokenType != TokenType.DIVIDE)
            {
                return expression;
            }
            if (expression == null)
            {
                // TODO : Exeption
            }

            while (_scanner.CurrentToken.TokenType == TokenType.MULTI || _scanner.CurrentToken.TokenType == TokenType.DIVIDE)
            {
                MultiDivideNode multiDivide = new MultiDivideNode(_scanner.CurrentToken);
                _scanner.MoveToNextToken();

                multiDivide.Left = expression;
                multiDivide.Right = TryToParseUnary();
                if (multiDivide.Right == null)
                {
                    // TODO : Exeption
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
                UnaryNode unary = new UnaryNode(_scanner.CurrentToken);
                _scanner.MoveToNextToken();
                unary.Expression = TryToParseArgument();
                if (unary.Expression == null)
                {
                    // TODO : Exeption
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
            if(_scanner.CurrentToken.TokenType == TokenType.NUMBER)
            {
                return TryToParseConst();
            }
            if(_scanner.CurrentToken.TokenType == TokenType.BRACKET_ENTER)
            {
                return TryToParseBracket();
            }
            return TryToParseIdentyfikator();
        }

        Expression TryToParseConst()
        {
            if (_scanner.CurrentToken.TokenType == TokenType.NUMBER)
            {
                ConstNode con = new ConstNode(_scanner.CurrentToken);
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

            BracketNode bracket = new BracketNode(_scanner.CurrentToken);
            _scanner.MoveToNextToken();

            bracket.Expression = TryToParseExpression();
            if(bracket.Expression == null)
            {
                // TODO : Exeption
            }

            CheckIsAndConsume(TokenType.BRACKET_END);

            return bracket;
        }
    }
}
