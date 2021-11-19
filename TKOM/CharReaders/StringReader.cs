using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.Interfaces;

namespace TKOM.CharReaders
{
    public class StringReader : ICharReader
    {
        char _currentChar;

        int _position;
        int _positionInLine;
        int _currentLine;


        string _source;

        public StringReader(string source)
        {
            if(source == null)
            {
                throw new ArgumentNullException();
            }
            if(source.Length == 0)
            {
                throw new ArgumentException("Empty string");
            }
            _source = source;

            Restart();
        }

        public char CurrentChar => _currentChar;

        public (int, int) CurrentPosition => (_currentLine, _positionInLine);

        public string GetStringFromPosition((int, int) position, int lenght)
        {
            int positionInString = -1;
            int line = 1;

            while(line != position.Item1)
            {
                positionInString += 1;

                if (positionInString >= _source.Length)
                {
                    throw new ArgumentOutOfRangeException();
                }
                if(_source[positionInString] == '\n')
                {
                    line += 1;
                }
            }
            positionInString += position.Item2;

            return _source.Substring(positionInString, lenght);
        }

        public void Restart()
        {
            _position = -1;
            _positionInLine = 0;
            _currentLine = 1;

            _currentChar = (char)0;
        }

        public bool MoveToNextChar()
        {
            _position += 1;
            if(_position >= _source.Length)
            {
                _position = _source.Length;
                _currentChar = (char)0;
                return false;
            }

            _currentChar = _source[_position];

            if(_currentChar == '\n')
            {
                _positionInLine = 0;
                _currentLine += 1;
            }
            else
            {
                _positionInLine += 1;
            }

            return true;
        }
    }
}
