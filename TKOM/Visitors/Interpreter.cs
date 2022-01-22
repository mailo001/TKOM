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

            InitSysFunction();
        }

        void InitSysFunction()
        {
            _functions = new Dictionary<string, FunctionNode>();

            _functions.Add(PrintFunctionNode.IdentyfireStr, new PrintFunctionNode());
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
            FunctionNode mainFunction;
            if(!program.Functions.TryGetValue(_Main, out mainFunction))
            {
                // Exeption
                throw new InterpreterExeption("Program does not implement main function!", (1, 0));
            }
            if(mainFunction.ParametrList.Count != 0)
            {
                // Exeption
                throw new InterpreterExeption("Main function has got parametr list!", program.Functions[_Main].IdentyfirePosition);
            }
            // Add function and check sys function
            foreach(var function in program.Functions)
            {
                if(!_functions.TryAdd(function.Key, function.Value))
                {
                    // Exeption
                    throw new InterpreterExeption("Function" + function.Value.Identyfire + "has same name as system function", function.Value.IdentyfirePosition);
                }
            }

            mainFunction.Accept(this);
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
            if(functionNode.ParametrList == null)
            {
                // Exeption
                throw new Exception("System function visit as function!");
            }

            _functionCallStatments.Push(new FunctionCallStatment());

            AddAndSetParametrList(functionNode.ParametrList);
            if (!_throw.IsNull())
            {
                _functionCallStatments.Pop();
                return;
            }

            functionNode.BlockInstruction.Accept(this);

            _functionCallStatments.Pop();
        }

        public override void VisitFunctionInvocation(FunctionInvocationNode functionInvocationNode)
        {
            FunctionNode function;
            if(!_functions.TryGetValue(functionInvocationNode.Identyfire, out function))
            {
                // Exeption
                throw new InterpreterExeption("Program dose not implement \"" + functionInvocationNode.Identyfire + "\" function!", 
                    functionInvocationNode.Position);
            }
            if(function.ParametrList != null && function.ParametrList.Count != functionInvocationNode.Arguments.Count)
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

            function.Accept(this);
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

        void AddAndSetParametrList(List<VariableDefinitionNode> variableDefinitions)
        {
            if (_lastParametrsList.Count != variableDefinitions.Count)
            {
                throw new Exception("Function has different number of argument!");
            }
            if(variableDefinitions.Count == 0)
            {
                return;
            }

            _accualCall.AddNewFirstScope();
            int i = 0;
            foreach(var parametr in variableDefinitions)
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
