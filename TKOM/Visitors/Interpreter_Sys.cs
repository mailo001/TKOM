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
        const string _PrintString = "print";
        void PrintFunction(List<Expression> expressions)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (Expression expression in expressions)
            {
                expression.Accept(this);
                if (!_throw.IsNull())
                    return;

                if (_expression.IsNull())
                {
                    // TODO : Exeption
                }
                stringBuilder.Append(_expression.GetAndClear());
                stringBuilder.Append(' ');
            }

            Console.WriteLine(stringBuilder.ToString());
        }
    }
}
