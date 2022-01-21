using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKOM.TreeNodes;

namespace TKOM.Interfaces
{
    public interface IParser
    {
        ProgramNode GenerateProgramTree();
    }
}
