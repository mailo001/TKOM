using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.Common;

namespace TKOM.TreeNodes
{
    public abstract class IdentyfireNode : Expression
    {
        public IdentyfireNode(Token token, NodeType nodeType) : base(token, nodeType) { }

        public string Identyfire { get => Token.Text; }
    }

    public class VariableNode : IdentyfireNode
    {
        public VariableNode(Token token) : base(token, NodeType.Variable) { }
    }

    public class AssigmentNode : IdentyfireNode
    {
        public AssigmentNode(Token token) : base(token, NodeType.Assigment) { }

        public Expression Expression { get; set; }
    }

    public class FunctionInvocationNode : IdentyfireNode
    {
        List<Expression> _argument;
        public FunctionInvocationNode(Token token) : base(token, NodeType.FunctionInvocation) 
        {
            _argument = new List<Expression>();
        }

        public List<Expression> Arguments { get => _argument; }
    }
}
