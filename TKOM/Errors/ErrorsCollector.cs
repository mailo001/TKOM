using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.Interfaces;

namespace TKOM.Errors
{
    public class ErrorsCollector : IErrors
    {
        ICharReader _charReader;
        List<Error> _errors;

        public ErrorsCollector()
        {
            _errors = new List<Error>();
        }

        public void AddSource(ICharReader reader)
        {
            _charReader = reader;
        }

        public void ReportError((int, int) position, string message)
        {
            _errors.Add(new Error(position, message));
        }

        public List<Error> ErrorsList { get => _errors; }

        public void PrintErrors()
        {
            foreach(Error error in _errors)
            {
                Console.Write(error.Show(_charReader));
            }
        }
    }
}
