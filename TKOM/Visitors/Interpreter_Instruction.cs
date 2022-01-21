using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.TreeNodes;
using TKOM.Errors;

namespace TKOM.Visitors
{
    public partial class Interpreter : NodeVisitor
    {
        public override void VisitVariableDefinition(VariableDefinitionNode variableDefinitionNode)
        {
            // System variable
            if(variableDefinitionNode.Identyfire == _ExeptionString)
            {
                // Exeption
                throw new InterpreterExeption("Variable \"" + variableDefinitionNode.Identyfire + "\" is system variable!",
                    variableDefinitionNode.IdentyfirePosition);
            }
            if (!_accualCall.TryAddVariable(variableDefinitionNode.Identyfire))
            {
                // Exeption
                throw new InterpreterExeption("Variable \"" + variableDefinitionNode.Identyfire + "\" exist in this scope!",
                    variableDefinitionNode.IdentyfirePosition);
            }
            if(variableDefinitionNode.Value != null)
            {
                variableDefinitionNode.Value.Accept(this);
                if (!_throw.IsNull())
                    return;

                if(_expression.IsNull())
                {
                    // Exeption
                    throw new Exception("Expression is null in variable deffiniton");
                }
                _accualCall.TryToSetVariableValue(variableDefinitionNode.Identyfire, _expression.GetAndClear());
            }
        }

        public override void VisitAssigment(AssigmentNode assigmentNode)
        {
            if(!_accualCall.CheckVariableExist(assigmentNode.Identyfire))
            {
                // Exeption
                throw new InterpreterExeption("Variable \"" + assigmentNode.Identyfire + "\" does not exist in this scope!",
                    assigmentNode.Position);
            }

            assigmentNode.Expression.Accept(this);
            if (!_throw.IsNull())
                return;

            if(_expression.IsNull())
            {
                // Exeption
                throw new Exception("Expression is null in assigment");
            }
            int value = _expression.GetAndClear();
            _accualCall.TryToSetVariableValue(assigmentNode.Identyfire, value);
            _expression.Set(value);
        }

        public override void VisitBlockInstruction(BlockInstructionNode blockInstructionNode)
        {
            _accualCall.AddNewFirstScope();
            foreach(var instruction in blockInstructionNode.Instructions)
            {
                instruction.Accept(this);
                if (!_throw.IsNull() || !_return.IsNull())
                {
                    break;
                }
                _expression.Clear();
            }
            _expression.Clear();
            _accualCall.PopFirstScope();
        }

        public override void VisitIfElse(IfElseNode ifElseNode)
        {
            ifElseNode.Condition.Accept(this);
            if (!_throw.IsNull())
                return;

            if(_expression.IsNull())
            {
                // Exeption
                throw new Exception("Expression is null in if else");
            }

            if(_expression.GetAndClear() != 0)
            {
                ifElseNode.IfBlock.Accept(this);
            }
            else
            {
                if(ifElseNode.ElseBlock != null)
                {
                    ifElseNode.ElseBlock.Accept(this);
                }
            }
        }

        public override void VisitReturn(ReturnNode returnNode)
        {
            returnNode.Value.Accept(this);
            if (!_throw.IsNull())
                return;

            if (_expression.IsNull())
            {
                // Exeption
                throw new Exception("Expression is null in return");
            }

            _return.Set(_expression.GetAndClear());
        }

        public override void VisitThrow(ThrowNode throwNode)
        {
            throwNode.Value.Accept(this);
            if (!_throw.IsNull())
                return;

            if (_expression.IsNull())
            {
                // Exeption
                throw new Exception("Expression is null in throw");
            }

            _throw.Set(_expression.GetAndClear()); ;
        }

        public override void VisitTryCatch(TryCatchNode tryCatchNode)
        {
            tryCatchNode.TryBlock.Accept(this);
            if (_throw.IsNull())
                return;

            int exeption = _throw.GetAndClear();

            // Add to Exeption variable
            _ExeptionVariable = exeption;

            foreach(var catchCond in tryCatchNode.CatchList)
            {
                catchCond.Item1.Accept(this);
                if(!_throw.IsNull())
                {
                    _throw.Set(exeption);
                    _ExeptionVariable = null;
                    return;
                }

                if(_expression.IsNull())
                {
                    // Exeption
                    throw new Exception("Expression is null in try catch");
                }
                if(_expression.GetAndClear() != 0)
                {
                    _ExeptionVariable = null;
                    catchCond.Item2.Accept(this);
                    return;
                }
            }

            _throw.Set(exeption);
            _ExeptionVariable = null;
        }

        public override void VisitWhile(WhileNode whileNode)
        {
            // Condition
            whileNode.Condition.Accept(this);
            if (!_throw.IsNull())
                return;

            if (_expression.IsNull())
            {
                // Exeption
                throw new Exception("Expression is null in while");
            }

            while (_expression.GetAndClear() != 0)
            {
                whileNode.Block.Accept(this);
                if (!_throw.IsNull() || !_return.IsNull())
                    return;

                // Condition
                whileNode.Condition.Accept(this);
                if (!_throw.IsNull())
                    return;

                if (_expression.IsNull())
                {
                    // Exeption
                    throw new Exception("Expression is null in while");
                }
            }
            
        }
    }
}
