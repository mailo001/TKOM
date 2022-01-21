using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.Common;
using TKOM.Visitors;

namespace TKOM.TreeNodes
{
    public abstract class IdentyfireNode : Expression
    {
        string _identyfire;
        public IdentyfireNode(string identyfire, (int, int) position, NodeType nodeType) : base(position, nodeType)
        {
            _identyfire = identyfire;
        }

        public string Identyfire { get => _identyfire; }
    }

    public class VariableNode : IdentyfireNode
    {
        public VariableNode(string identyfire, (int, int) position) : base(identyfire, position, NodeType.Variable) { }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitVariable(this);
        }
    }

    public class AssigmentNode : IdentyfireNode
    {
        public AssigmentNode(string identyfire, (int, int) position) : base(identyfire, position, NodeType.Assigment) { }

        public Expression Expression { get; set; }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitAssigment(this);
        }
    }

    public class FunctionInvocationNode : IdentyfireNode
    {
        List<Expression> _argument;
        public FunctionInvocationNode(string identyfire, (int, int) position) : base(identyfire, position, NodeType.FunctionInvocation) 
        {
            _argument = new List<Expression>();
        }

        public List<Expression> Arguments { get => _argument; }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitFunctionInvocation(this);
        }
    }
}
