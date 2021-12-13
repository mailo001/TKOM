using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.Common;

namespace TKOM.TreeNodes
{
    public enum NodeType
    {
        Program,

        Function,
        ParametrList,

        BlockInstruction,

        VariableDefinition,
        Return,
        Throw,
        TryCatch,
        IfElse,
        While,

        Variable,
        Assigment,
        FunctionInvocation,

        Or,
        And,
        Not,
        ConstLogic,
        Compare,

        PlusMinus,
        MultiDivide,
        Unary,
        Const,

        Brackets
    }

    public abstract class Node
    {
        NodeType _nodeType;

        public Node(NodeType nodeType)
        {
            _nodeType = nodeType;
        }

        public NodeType NodeType { get => _nodeType; }
    }


    /// <summary>
    /// Program   =   {Funkcja} ;
    /// </summary>
    public class ProgramNode : Node
    {
        Dictionary<string, FunctionNode> _dictionary;
        public ProgramNode() : base(NodeType.Program)
        {
            _dictionary = new Dictionary<string, FunctionNode>();
        }

        public Dictionary<string, FunctionNode> Functions { get => _dictionary; }
    }


    /// <summary>
    /// Funkcja               =   NazwaTypu identyfikator "(" [ListaParametrow] ")" InstrukcjaBlokowa;
    /// </summary>
    public class FunctionNode : Node
    {
        Token _identyfire;
        public FunctionNode(Token identyfire) : base(NodeType.Function)
        {
            _identyfire = identyfire;
        }

        public string Identyfire { get => _identyfire.Text; }

        public Token IdentyfireToken { get => _identyfire; }

        public ParametrListNode ParametrList { get; set; }

        public BlockInstructionNode BlockInstruction { get; set; }
    }


    /// <summary>
    /// ListaParametrow       =   DefinicjaZmiennej {"," DefinicjaZmiennej} ;
    /// </summary>
    public class ParametrListNode : Node
    {
        List<VariableDefinitionNode> _variables;

        public ParametrListNode() : base(NodeType.ParametrList)
        {
            _variables = new List<VariableDefinitionNode>();
        }

        public List<VariableDefinitionNode> Variables { get => _variables; }
    }

}
