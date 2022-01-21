using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.Interfaces;

namespace TKOM.Errors
{
    public class InterpreterExeption : Exception
    {
        Error _error;
        public InterpreterExeption(string message, (int, int) position) : base(message)
        {
            _error = new Error(position, message);
        }

        public string Show(ICharReader charReader)
        {
            return _error.Show(charReader);
        }
    }
}
