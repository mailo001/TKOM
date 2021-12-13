using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.Common;

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
    }

    /// <summary>
    /// DefinicjaZmiennej     =   NazwaTypu identyfikator["=" Wyrazenie];
    /// </summary>
    public class VariableDefinitionNode : Instruction
    {
        Token _token;

        public VariableDefinitionNode(Token token) : base(NodeType.VariableDefinition)
        {
            _token = token;
        }

        public Token IdentyfireToken { get => _token; }
        public string Identyfire { get => _token.Text; }
        public Expression Value { get; set; }
    }

    public class ReturnNode : Instruction
    {
        public ReturnNode() : base(NodeType.Return) { }

        public Expression Value { get; set; }
    }

    public class ThrowNode : Instruction
    {
        public ThrowNode() : base(NodeType.Throw) { }

        public Expression Value { get; set; }
    }

    public class IfElseNode : Instruction
    {
        public IfElseNode() : base(NodeType.IfElse) { }

        public Expression Condition { get; set; }
        public BlockInstructionNode IfBlock { get; set; }
        public BlockInstructionNode ElseBlock { get; set; }
    }

    public class WhileNode : Instruction
    {
        public WhileNode() : base(NodeType.While) { }

        public Expression Condition { get; set; }
        public BlockInstructionNode Block { get; set; }
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
    }
}
