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

        public override void VisitAnd(AndNode andNode)
        {
            andNode.Left.Accept(this);
            if (!_throw.IsNull())
                return;

            int left = _expression.GetAndClear();

            if(left == 0)
            {
                _expression.Set(0);
                return;
            }

            andNode.Right.Accept(this);
            if (!_throw.IsNull())
                return;

            int right = _expression.GetAndClear();

            _expression.Set(left * right == 0 ? 0 : 1);
        }

        public override void VisitCompare(CompareNode compareNode)
        {
            compareNode.Left.Accept(this);
            if (!_throw.IsNull())
                return;

            int left = _expression.GetAndClear();

            compareNode.Right.Accept(this);
            if (!_throw.IsNull())
                return;

            int right = _expression.GetAndClear();

            bool result;
            switch(compareNode.NodeType)
            {
                case NodeType.Equal:
                    result = left == right;
                    break;
                case NodeType.NoEqual:
                    result = left != right;
                    break;
                case NodeType.More:
                    result = left > right;
                    break;
                case NodeType.MoreEqual:
                    result = left >= right;
                    break;
                case NodeType.Less:
                    result = left < right;
                    break;
                case NodeType.LessEqual:
                    result = left <= right;
                    break;
                default:
                    throw new Exception("Invalid Tree: CompareNode");
            }
            _expression.Set(result? 1 : 0);
        }

        public override void VisitConst(ConstNode constNode)
        {
            _expression.Set(constNode.Value);
        }

        public override void VisitLogicConst(LogicConstNode logicConstNode)
        {
            _expression.Set(logicConstNode.Value ? 1 : 0);
        }

        public override void VisitMultiDiv(MultiDivideNode multiDivideNode)
        {
            multiDivideNode.Left.Accept(this);
            if (!_throw.IsNull())
                return;

            int left = _expression.GetAndClear();

            multiDivideNode.Right.Accept(this);
            if (!_throw.IsNull())
                return;

            int right = _expression.GetAndClear();

            if (multiDivideNode.NodeType == NodeType.Multi)
                _expression.Set(left * right);
            else if (multiDivideNode.NodeType == NodeType.Divide)
                _expression.Set(left / right);
            else
                throw new Exception("Invalid Tree: MultiDivideNode");
        }

        public override void VisitNot(NotNode notNode)
        {
            notNode.Expression.Accept(this);
            if (!_throw.IsNull())
                return;

            _expression.Set(_expression.GetAndClear() == 0 ? 1 : 0);
        }

        public override void VisitOr(OrNode orNode)
        {
            orNode.Left.Accept(this);
            if (!_throw.IsNull())
                return;

            int left = _expression.GetAndClear();

            if (left != 0)
            {
                _expression.Set(1);
                return;
            }

            orNode.Right.Accept(this);
            if (!_throw.IsNull())
                return;

            int right = _expression.GetAndClear();

            _expression.Set(left == 0 && right == 0 ? 0 : 1);
        }

        public override void VisitPlusMinus(PlusMinusNode plusMinusNode)
        {
            plusMinusNode.Left.Accept(this);
            if (!_throw.IsNull())
                return;

            int left = _expression.GetAndClear();

            plusMinusNode.Right.Accept(this);
            if (!_throw.IsNull())
                return;

            int right = _expression.GetAndClear();

            if (plusMinusNode.NodeType == NodeType.Plus)
                _expression.Set(left + right);
            else if (plusMinusNode.NodeType == NodeType.Minus)
                _expression.Set(left - right);
            else
                throw new Exception("Invalid Tree: PlusMinusNode");
        }

        public override void VisitUnary(UnaryNode unaryNode)
        {
            unaryNode.Expression.Accept(this);
            if (!_throw.IsNull())
                return;

            _expression.Set(-_expression.GetAndClear());
        }

        public override void VisitVariable(VariableNode variableNode)
        {
            if(_ExeptionVariable != null && variableNode.Identyfire == _ExeptionString)
            {
                _expression.Set(_ExeptionVariable.Value);
                return;
            }
            int value;
            if(!_accualCall.TryGetVariableValue(variableNode.Identyfire, out value))
            {
                // Exeption
                throw new InterpreterExeption("Variable \"" + variableNode.Identyfire + "\" does not exist in this scope!",
                    variableNode.Position);
            }
            _expression.Set(value);
        }

    }
}
