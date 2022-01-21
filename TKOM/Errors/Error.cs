using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.Interfaces;

namespace TKOM.Errors
{
    public class Error
    {
        public (int, int) position;
        public string message;

        public Error((int, int) pos, string mess)
        {
            position = pos;
            message = mess;
        }

        public string Show(ICharReader charReader)
        {
            if (charReader == null)
                throw new ArgumentNullException();

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append('\n');
            stringBuilder.Append("Error: ");
            stringBuilder.Append(message);
            stringBuilder.Append('\n');
            stringBuilder.Append("Position: (line: ");
            stringBuilder.Append(position.Item1);
            stringBuilder.Append(", char: ");
            stringBuilder.Append(position.Item2);
            stringBuilder.Append(")");
            stringBuilder.Append('\n');
            stringBuilder.Append('\n');
            string line = charReader.GetLine(position);
            stringBuilder.Append(line);
            stringBuilder.Append('\n');
            for (int i = 1; i < position.Item2; i++)
            {
                if(line[i-1] == '\t')
                    stringBuilder.Append('\t');
                else
                    stringBuilder.Append(' ');
            }
            stringBuilder.Append('^');
            stringBuilder.Append('\n');

            return stringBuilder.ToString();
        }
    }
}
