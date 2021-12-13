using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.Common;

namespace TKOM.TreeNodes
{
    public abstract class Expression : Instruction
    {
        Token _token;
        public Expression(Token token, NodeType nodeType) : base(nodeType)
        {
            _token = token;
        }

        public Token Token { get => _token; }

    }

    public class LogicConstNode : Expression
    {
        public LogicConstNode(Token token) : base(token, NodeType.ConstLogic) { }
    }

    public class ConstNode : Expression
    {
        public ConstNode(Token token) : base(token, NodeType.Const) { }
    }

    /// <summary>
    /// Two Argument operator
    /// </summary>
    public abstract class Operator : Expression
    {
        public Operator(Token token, NodeType nodeType) : base(token ,nodeType) { }

        public Expression Left { get; set; }
        public Expression Right { get; set; }
    }

    public class OrNode : Operator
    {
        public OrNode(Token token) : base(token, NodeType.Or) { }
    }

    public class AndNode : Operator
    {
        public AndNode(Token token) : base(token, NodeType.And) { }
    }

    public class CompareNode : Operator
    {
        public CompareNode(Token token) : base(token, NodeType.Compare) { }
    }

    public class PlusMinusNode : Operator
    {
        public PlusMinusNode(Token token) : base(token, NodeType.PlusMinus) { }
    }

    public class MultiDivideNode : Operator
    {
        public MultiDivideNode(Token token) : base(token, NodeType.MultiDivide) { }
    }

    /// <summary>
    /// One argument operatro
    /// </summary>
    public abstract class OneOperator : Expression
    {
        public OneOperator(Token token, NodeType nodeType) : base(token, nodeType) { }

        public Expression Expression { get; set; }
    }

    public class NotNode : OneOperator
    {
        public NotNode(Token token) : base(token, NodeType.Not) { }
    }

    public class UnaryNode : OneOperator
    {
        public UnaryNode(Token token) : base(token, NodeType.Not) { }
    }

    public class BracketNode : OneOperator
    {
        public BracketNode(Token token) : base(token, NodeType.Not) { }
    }

}
