using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.TreeNodes;
using TKOM.Common;

namespace TKOM.Visitors
{
    public partial class Interpreter
    {
        public override void VisitPrintFunction(PrintFunctionNode printFunctionNode)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (int expression in _lastParametrsList)
            {
                stringBuilder.Append(expression);
                stringBuilder.Append(' ');
            }

            Console.WriteLine(stringBuilder.ToString());

            _return.Set(0);
        }
    }
}
