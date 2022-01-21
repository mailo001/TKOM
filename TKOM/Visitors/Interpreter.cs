using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.TreeNodes;
using TKOM.Common;
using TKOM.Errors;
using TKOM.Interfaces;

namespace TKOM.Visitors
{
    public partial class Interpreter : NodeVisitor, IInterpreter
    {
        Stack<FunctionCallStatment> _functionCallStatments;

        List<int> _lastParametrsList;

        Dictionary<string, FunctionNode> _functions;

        FunctionCallStatment _accualCall { get => _functionCallStatments.Peek();  }
        
        Getter _return;
        Getter _throw;
        Getter _expression;

        const string _ExeptionString = "Exception";
        int? _ExeptionVariable;

        const string _Main = "main";

        public Interpreter()
        {
            _functionCallStatments = new Stack<FunctionCallStatment>();

            _return = new Getter();
            _throw = new Getter();
            _expression = new Getter();

            _lastParametrsList = new List<int>();

            _functions = null;
            _ExeptionVariable = null;
        }

        public (int?, int?) InterpreteProgramTree(ProgramNode program)
        {
            program.Accept(this);
            if(!_throw.IsNull())
            {
                return (null, _throw.GetAndClear());
            }

            return (_return.GetAndClear(), null);
        }

        public override void VisitProgram(ProgramNode program)
        {
            // Check sys function
            if(program.Functions.ContainsKey(_PrintString))
            {
                // Exeption
                throw new InterpreterExeption("Program implements system function!", program.Functions[_PrintString].IdentyfirePosition);
                
            }
            if(!program.Functions.ContainsKey(_Main))
            {
                // Exeption
                throw new InterpreterExeption("Program does not implement main function!", (1, 0));
            }
            if(program.Functions[_Main].ParametrList != null)
            {
                // Exeption
                throw new InterpreterExeption("Main function has got parametr list!", program.Functions[_Main].IdentyfirePosition);
            }

            _functions = program.Functions;

            program.Functions[_Main].Accept(this);
            if (!_throw.IsNull())
                return;

            if (_return.IsNull())
            {
                // Exeption
                throw new InterpreterExeption("Function \"" + _Main + "\" does not return an argument!",
                    program.Functions[_Main].IdentyfirePosition);
            }
        }

        public override void VisitFunction(FunctionNode functionNode)
        {
            _functionCallStatments.Push(new FunctionCallStatment());

            if(functionNode.ParametrList != null)
            { 
                functionNode.ParametrList.Accept(this);
                if (!_throw.IsNull())
                {
                    _functionCallStatments.Pop();
                    return;
                }
            }

            functionNode.BlockInstruction.Accept(this);

            _functionCallStatments.Pop();
        }

        public override void VisitFunctionInvocation(FunctionInvocationNode functionInvocationNode)
        {
            // Add sys function
            if(functionInvocationNode.Identyfire == _PrintString)
            {
                PrintFunction(functionInvocationNode.Arguments);
                _expression.Set(0);
                return;
            }
            
            if(!_functions.ContainsKey(functionInvocationNode.Identyfire))
            {
                // Exeption
                throw new InterpreterExeption("Program dose not implement \"" + functionInvocationNode.Identyfire + "\" function!", 
                    functionInvocationNode.Position);
            }
            if(_functions[functionInvocationNode.Identyfire].ParametrList == null)
            {
                if(functionInvocationNode.Arguments.Count != 0)
                {
                    // Exeption
                    throw new InterpreterExeption("Function \"" + functionInvocationNode.Identyfire + "\" has different number of argument!",
                        functionInvocationNode.Position);
                }
            }
            else if(_functions[functionInvocationNode.Identyfire].ParametrList.Variables.Count != functionInvocationNode.Arguments.Count)
            {
                // Exeption
                throw new InterpreterExeption("Function \"" + functionInvocationNode.Identyfire + "\" has different number of argument!",
                    functionInvocationNode.Position);
            }

            _lastParametrsList.Clear();
            foreach(Expression expression in functionInvocationNode.Arguments)
            {
                expression.Accept(this);
                if (!_throw.IsNull())
                    return;

                if(_expression.IsNull())
                {
                    // Exeption
                    throw new Exception("Function argument does not set expression value!");
                }
                _lastParametrsList.Add(_expression.GetAndClear());
            }

            _functions[functionInvocationNode.Identyfire].Accept(this);
            if (!_throw.IsNull())
                return;

            if(_return.IsNull())
            {
                // Exeption
                throw new InterpreterExeption("Function \"" + functionInvocationNode.Identyfire + "\" does not return an argument!",
                    functionInvocationNode.Position);
            }

            _expression.Set(_return.GetAndClear());

            _lastParametrsList.Clear();
        }

        public override void VisitParametrList(ParametrListNode parametrListNode)
        {
            if (_lastParametrsList.Count != parametrListNode.Variables.Count)
            {
                throw new Exception("Function has different number of argument!");
            }

            _accualCall.AddNewFirstScope();
            int i = 0;
            foreach(var parametr in parametrListNode.Variables)
            {
                parametr.Accept(this);
                if (!_throw.IsNull())
                    return;

                _accualCall.TryToSetVariableValue(parametr.Identyfire, _lastParametrsList[i]);
                i++;
            }
        }
    }
}
