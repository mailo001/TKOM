using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKOM.Interfaces
{
    public interface IErrors
    {
        void AddSource(ICharReader reader);
        void ReportError((int, int) position, string message);

    }

}
