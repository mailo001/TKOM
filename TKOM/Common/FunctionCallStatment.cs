using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKOM.Common
{
    public class FunctionCallStatment
    {
        Stack<Dictionary<string, int>> _scopeStack;

        public FunctionCallStatment()
        {
            _scopeStack = new Stack<Dictionary<string, int>>();
        }

        public void AddNewFirstScope()
        {
            _scopeStack.Push(new Dictionary<string, int>());
        }

        public void PopFirstScope()
        {
            _scopeStack.Pop();
        }

        public bool CheckVariableExist(string identyfire)
        {
            foreach (var dictionary in _scopeStack)
            {
                if (dictionary.ContainsKey(identyfire))
                {
                    return true;
                }
            }
            return false;
        }

        public bool TryAddVariable(string identyfire, int value = 0)
        {
            if(CheckVariableExist(identyfire))
            {
                return false;
            }

            _scopeStack.Peek().Add(identyfire, value);
            return true;
        }

        public bool TryGetVariableValue(string identyfire, out int value)
        {
            foreach(var dictionary in _scopeStack)
            {
                if(dictionary.TryGetValue(identyfire, out value))
                {
                    return true;
                }
            }
            value = 0;
            return false;
        }

        public bool TryToSetVariableValue(string identyfire, int value)
        {
            foreach (var dictionary in _scopeStack)
            {
                if (dictionary.ContainsKey(identyfire))
                {
                    dictionary[identyfire] = value;
                    return true;
                }
            }
            return false;
        }
    }
}
