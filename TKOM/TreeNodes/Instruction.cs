using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.Common;
using TKOM.Visitors;

namespace TKOM.TreeNodes
{

    public abstract class Instruction : Node
    {
        public Instruction(NodeType nodeType) : base(nodeType) { }
    }

    /// <summary>
    /// InstrukcjaBlokowa     =   "{" {Instrukcja} "}" ;
    /// </summary>
    public class BlockInstructionNode : Instruction
    {
        List<Instruction> _instructions;

        public BlockInstructionNode() : base(NodeType.BlockInstruction)
        {
            _instructions = new List<Instruction>();
        }

        public List<Instruction> Instructions { get => _instructions; }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitBlockInstruction(this);
        }
    }

    /// <summary>
    /// DefinicjaZmiennej     =   NazwaTypu identyfikator["=" Wyrazenie];
    /// </summary>
    public class VariableDefinitionNode : Instruction
    {
        string _identyfire;
        (int, int) _position;
        public VariableDefinitionNode(string identyfire, (int, int) position) : base(NodeType.VariableDefinition)
        {
            _identyfire = identyfire;
            _position = position;
        }

        public string Identyfire { get => _identyfire; }

        public (int, int) IdentyfirePosition { get => _position; }

        public Expression Value { get; set; }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitVariableDefinition(this);
        }
    }

    public class ReturnNode : Instruction
    {
        public ReturnNode() : base(NodeType.Return) { }

        public Expression Value { get; set; }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitReturn(this);
        }
    }

    public class ThrowNode : Instruction
    {
        public ThrowNode() : base(NodeType.Throw) { }

        public Expression Value { get; set; }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitThrow(this);
        }
    }

    public class IfElseNode : Instruction
    {
        public IfElseNode() : base(NodeType.IfElse) { }

        public Expression Condition { get; set; }
        public BlockInstructionNode IfBlock { get; set; }
        public BlockInstructionNode ElseBlock { get; set; }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitIfElse(this);
        }
    }

    public class WhileNode : Instruction
    {
        public WhileNode() : base(NodeType.While) { }

        public Expression Condition { get; set; }
        public BlockInstructionNode Block { get; set; }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitWhile(this);
        }
    }

    public class TryCatchNode : Instruction
    {
        List<(Expression, BlockInstructionNode)> _catchList;

        public TryCatchNode() : base(NodeType.TryCatch) 
        {
            _catchList = new List<(Expression, BlockInstructionNode)>();
        }

        public BlockInstructionNode TryBlock { get; set; }

        public List<(Expression, BlockInstructionNode)> CatchList { get => _catchList; }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitTryCatch(this);
        }
    }
}
