using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.TreeNodes;

namespace TKOM.Visitors
{
    public abstract class NodeVisitor
    {
        public abstract void VisitProgram(ProgramNode program);

        public abstract void VisitFunction(FunctionNode functionNode);

        public abstract void VisitVariableDefinition(VariableDefinitionNode variableDefinitionNode);

        public abstract void VisitBlockInstruction(BlockInstructionNode blockInstructionNode);

        public abstract void VisitIfElse(IfElseNode ifElseNode);

        public abstract void VisitWhile(WhileNode whileNode);

        public abstract void VisitTryCatch(TryCatchNode tryCatchNode);

        public abstract void VisitReturn(ReturnNode returnNode);

        public abstract void VisitThrow(ThrowNode throwNode);

        public abstract void VisitOr(OrNode orNode);

        public abstract void VisitAnd(AndNode andNode);

        public abstract void VisitNot(NotNode notNode);

        public abstract void VisitLogicConst(LogicConstNode logicConstNode);

        public abstract void VisitCompare(CompareNode compareNode);

        public abstract void VisitPlusMinus(PlusMinusNode plusMinusNode);

        public abstract void VisitMultiDiv(MultiDivideNode multiDivideNode);

        public abstract void VisitUnary(UnaryNode unaryNode);

        public abstract void VisitConst(ConstNode constNode);

        public abstract void VisitVariable(VariableNode variableNode);

        public abstract void VisitAssigment(AssigmentNode assigmentNode);

        public abstract void VisitFunctionInvocation(FunctionInvocationNode functionInvocationNode);

        public abstract void VisitPrintFunction(PrintFunctionNode printFunctionNode);
    }
}
