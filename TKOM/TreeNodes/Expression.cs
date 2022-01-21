using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.Common;
using TKOM.Visitors;

namespace TKOM.TreeNodes
{
    public abstract class Expression : Instruction
    {
        (int, int) _position;
        public Expression((int, int) position, NodeType nodeType) : base(nodeType)
        {
            _position = position;
        }

        public (int, int) Position { get => _position; }

    }

    public class LogicConstNode : Expression
    {
        bool _value;
        public LogicConstNode(bool value, (int, int) position) : base(position, NodeType.ConstLogic)
        {
            _value = value;
        }

        public bool Value { get => _value; }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitLogicConst(this);
        }
    }

    public class ConstNode : Expression
    {
        int _value;
        public ConstNode(int value, (int, int) position) : base(position, NodeType.Const) 
        {
            _value = value;
        }

        public int Value { get => _value; }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitConst(this);
        }
    }

    /// <summary>
    /// Two Argument operator
    /// </summary>
    public abstract class Operator : Expression
    {
        public Operator((int, int) position, NodeType nodeType) : base(position ,nodeType) { }

        public Expression Left { get; set; }
        public Expression Right { get; set; }
    }

    public class OrNode : Operator
    {
        public OrNode((int, int) position) : base(position, NodeType.Or) { }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitOr(this);
        }
    }

    public class AndNode : Operator
    {
        public AndNode((int, int) position) : base(position, NodeType.And) { }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitAnd(this);
        }
    }

    public class CompareNode : Operator
    {
        public CompareNode(NodeType nodeType, (int, int) position) : base(position, nodeType)
        {
            if(nodeType != NodeType.Equal
                && nodeType != NodeType.NoEqual
                && nodeType != NodeType.More
                && nodeType != NodeType.MoreEqual
                && nodeType != NodeType.Less
                && nodeType != NodeType.LessEqual)
            {
                throw new ArgumentException("Incorect node type");
            }
        }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitCompare(this);
        }
    }

    public class PlusMinusNode : Operator
    {
        public PlusMinusNode(NodeType nodeType, (int, int) position) : base(position, nodeType)
        {
            if (nodeType != NodeType.Plus
                && nodeType != NodeType.Minus)
            {
                throw new ArgumentException("Incorect node type");
            }
        }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitPlusMinus(this);
        }
    }

    public class MultiDivideNode : Operator
    {
        public MultiDivideNode(NodeType nodeType, (int, int) position) : base(position, nodeType)
        {
            if (nodeType != NodeType.Multi
                && nodeType != NodeType.Divide)
            {
                throw new ArgumentException("Incorect node type");
            }
        }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitMultiDiv(this);
        }
    }

    /// <summary>
    /// One argument operatro
    /// </summary>
    public abstract class OneOperator : Expression
    {
        public OneOperator((int, int) position, NodeType nodeType) : base(position, nodeType) { }

        public Expression Expression { get; set; }
    }

    public class NotNode : OneOperator
    {
        public NotNode((int, int) position) : base(position, NodeType.Not) { }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitNot(this);
        }
    }

    public class UnaryNode : OneOperator
    {
        public UnaryNode((int, int) position) : base(position, NodeType.Unary) { }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitUnary(this);
        }
    }

    public class BracketNode : OneOperator
    {
        public BracketNode((int, int) position) : base(position, NodeType.Brackets) { }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitBracket(this);
        }
    }

}
