using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.Common;

namespace TKOM.Interfaces
{
    public interface IScanner
    {
        Token CurrentToken { get; }

        Token PrevToken { get; }

        bool MoveToNextToken();

        void Restart();
    }
}
