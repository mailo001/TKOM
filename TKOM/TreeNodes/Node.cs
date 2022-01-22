using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.Common;
using TKOM.Visitors;

namespace TKOM.TreeNodes
{
    public enum NodeType
    {
        Non,

        SysFunction,

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
        Equal,
        NoEqual,
        More,
        MoreEqual,
        Less,
        LessEqual,

        Plus,
        Minus,
        Multi,
        Divide,
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

        public abstract void Accept(NodeVisitor nodeVisitor);
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

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitProgram(this);
        }
    }


    /// <summary>
    /// Funkcja               =   NazwaTypu identyfikator "(" [ListaParametrow] ")" InstrukcjaBlokowa;
    /// ListaParametrow       =   DefinicjaZmiennej {"," DefinicjaZmiennej} ;
    /// </summary>
    public class FunctionNode : Node
    {
        protected string _identyfire;
        protected (int, int) _position;
        protected List<VariableDefinitionNode> _parametrs;

        public FunctionNode(string identyfire, (int, int) position) : base(NodeType.Function)
        {
            _identyfire = identyfire;
            _position = position;
            _parametrs = new List<VariableDefinitionNode>();
        }

        public string Identyfire { get => _identyfire; }

        public (int, int) IdentyfirePosition { get => _position; }

        public List<VariableDefinitionNode> ParametrList { get => _parametrs; }

        public BlockInstructionNode BlockInstruction { get; set; }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitFunction(this);
        }
    }

    

}
