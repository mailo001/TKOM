using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.Common;
using TKOM.Visitors;

namespace TKOM.TreeNodes
{
    public class PrintFunctionNode : FunctionNode
    {
        public static string IdentyfireStr = "print";
        public PrintFunctionNode() : base(IdentyfireStr, (1, 0)) 
        {
            _parametrs = null;
        }

        public override void Accept(NodeVisitor nodeVisitor)
        {
            nodeVisitor.VisitPrintFunction(this);
        }
    }

}
