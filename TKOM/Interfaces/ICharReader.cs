using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKOM.Interfaces
{
    public interface ICharReader
    {
        public char CurrentChar { get; }

        public (int, int) CurrentPosition { get; }

        public bool MoveToNextChar();

        public void Restart();

        public string GetStringFromPosition((int, int) position, int lenght);
    }
}
