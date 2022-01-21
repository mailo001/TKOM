using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using TKOM.Interfaces;

namespace TKOM.CharReaders
{
    public class FileReader : ICharReader
    {
        char _currentChar;

        int _positionInLine;
        int _currentLineNumber;
        string _currentLine;

        string _fileName;

        StreamReader _streamReader;

        public FileReader(string fileName)
        {
            if(!File.Exists(fileName))
            {
                throw new ArgumentException("File don't exist");
            }
            _fileName = fileName;

            Restart();

            if(_currentLine == null)
            {
                throw new ArgumentException("File is empty");
            }
        }

        public char CurrentChar => _currentChar;

        public (int, int) CurrentPosition => (_currentLineNumber, _positionInLine);

        public string GetLine((int, int) position)
        {
            StreamReader streamReader = new StreamReader(_fileName);
            for (int i = 1; i < position.Item1; i++)
            {
                streamReader.ReadLine();
            }
            string line = streamReader.ReadLine();
            streamReader.Close();
            return line;
        }

        public string GetStringFromPosition((int, int) position, int lenght)
        {
            StreamReader streamReader = new StreamReader(_fileName);
            for(int i=1; i<position.Item1; i++)
            {
                streamReader.ReadLine();
            }
            string line = streamReader.ReadLine();
            streamReader.Close();
            if (line == null)
            {
                throw new ArgumentOutOfRangeException();
            }

            return line.Substring(position.Item2 - 1, lenght);
        }

        public bool MoveToNextChar()
        {
            if(_currentLine == null)
            {
                _streamReader.Close();
                _currentChar = '\0';
                return false;
            }

            if(_positionInLine >= _currentLine.Length)
            {
                _positionInLine = 0;
                _currentLineNumber += 1;
                _currentLine = _streamReader.ReadLine();
                _currentChar = '\n';
                return true;
            }

            _currentChar = _currentLine[_positionInLine];
            _positionInLine += 1;
            return true;
        }

        public void Restart()
        {
            if(_streamReader != null)
            {
                _streamReader.Close();
            }
            _streamReader = new StreamReader(_fileName);

            _currentChar = '\0';

            _positionInLine = 0;
            _currentLineNumber = 1;
            _currentLine = _streamReader.ReadLine();
        }
    }
}
