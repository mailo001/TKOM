using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TKOM.Interfaces;
using TKOM.Common;

namespace TKOM.Scanners
{
    public class Scanner : IScanner
    {
        ICharReader _charReader;
        IErrors _errors;
        Token _token;
        Token _prevToken;
        Dictionary<string, TokenType> _wordsDictionary;
        Dictionary<string, TokenType> _signsDictionary;

        public Scanner(ICharReader charReader, IErrors errors)
        {
            if (charReader == null || errors == null)
            {
                throw new ArgumentNullException();
            }

            _errors = errors;

            _charReader = charReader;
            Restart();

            InitSignsDictionary();
            InitWordDictionary();
        }

        public void Restart()
        {
            _charReader.Restart();
            _charReader.MoveToNextChar();

            _token = new Token(TokenType.EMPTY, "", (0, 0));
            _prevToken = _token;
        }

        public Token CurrentToken => _token;

        public Token PrevToken => _prevToken;

        public bool MoveToNextToken()
        {
            // PrevToken
            _prevToken = _token;

            //Omit white signs
            while (CharIsWhite(_charReader.CurrentChar))
            {
                _charReader.MoveToNextChar();
            }

            //Position of first char in Token
            (int, int) position = _charReader.CurrentPosition;

            //Check if it is end sign of char reader
            if (_charReader.CurrentChar == (char)0)
            {
                _token = new Token(TokenType.EOF, "", position);
                return false;
            }

            //Keyword or Identyfier
            if (char.IsLetter(_charReader.CurrentChar))
            {
                StringBuilder stringBuilder = new StringBuilder();
                while (char.IsLetterOrDigit(_charReader.CurrentChar))
                {
                    stringBuilder.Append(_charReader.CurrentChar);
                    _charReader.MoveToNextChar();
                }
                string text = stringBuilder.ToString();

                TokenType type;
                if (_wordsDictionary.TryGetValue(text, out type))
                {
                    _token = new Token(type, text, position);
                    return true;
                }

                _token = new Token(TokenType.IDENTIFIRE, text, position);
                return true;
            }

            //Number
            if (char.IsDigit(_charReader.CurrentChar))
            {
                StringBuilder stringBuilder = new StringBuilder();
                while (char.IsDigit(_charReader.CurrentChar))
                {
                    stringBuilder.Append(_charReader.CurrentChar);
                    _charReader.MoveToNextChar();
                }

                //Check if after digit is letter 
                if (char.IsLetter(_charReader.CurrentChar))
                {
                    while (char.IsLetterOrDigit(_charReader.CurrentChar))
                    {
                        stringBuilder.Append(_charReader.CurrentChar);
                        _charReader.MoveToNextChar();
                    }

                    string textWrongToken = stringBuilder.ToString();
                    _token = new Token(TokenType.UNKNOWN, textWrongToken, position);

                    // TODO : Commit wrong identyfier or number error to IError 
                    _errors.ReportError(position, "Incorect identyfire: " + textWrongToken + " It must start with letter!");

                    return true;
                }

                string text = stringBuilder.ToString();
                _token = new Token(TokenType.NUMBER, text, position);
                return true;
            }

            //Brackets singel Token
            if (CharIsBracket(_charReader.CurrentChar))
            {
                string text = _charReader.CurrentChar.ToString();
                TokenType type;
                if (!_signsDictionary.TryGetValue(text, out type))
                {
                    throw new Exception("Invalid Dictionary");
                }

                _token = new Token(type, text, position);

                _charReader.MoveToNextChar();

                return true;
            }

            // Correct Signs
            if (CharIsCorrectSign(_charReader.CurrentChar))
            {
                StringBuilder stringBuilder = new StringBuilder();
                while (CharIsCorrectSign(_charReader.CurrentChar))
                {
                    stringBuilder.Append(_charReader.CurrentChar);
                    _charReader.MoveToNextChar();
                }
                string text = stringBuilder.ToString();

                TokenType type;
                if (_signsDictionary.TryGetValue(text, out type))
                {
                    _token = new Token(type, text, position);
                    return true;
                }

                // Commit wrong operator error to IError 
                _errors.ReportError(position, "Wrong operator in code: " + text);

                _token = new Token(TokenType.UNKNOWN, text, position);

                return true;
            }

            // Commit incorect sign error to IError
            _errors.ReportError(position, "Incorect sign in code: \'" + _charReader.CurrentChar.ToString() + "\'");

            // Incorect Sign
            _token = new Token(TokenType.UNKNOWN, _charReader.CurrentChar.ToString(), position);
            _charReader.MoveToNextChar();

            return true;
        }

        void InitWordDictionary()
        {
            _wordsDictionary = new Dictionary<string, TokenType>();

            _wordsDictionary.Add("if", TokenType.IF);
            _wordsDictionary.Add("else", TokenType.ELSE);
            _wordsDictionary.Add("try", TokenType.TRY);
            _wordsDictionary.Add("catch", TokenType.CATCH);
            _wordsDictionary.Add("while", TokenType.WHILE);

            _wordsDictionary.Add("return", TokenType.RETURN);
            _wordsDictionary.Add("throw", TokenType.THROW);

            _wordsDictionary.Add("true", TokenType.TRUE);
            _wordsDictionary.Add("false", TokenType.FALSE);

            _wordsDictionary.Add("int", TokenType.INT);
        }

        void InitSignsDictionary()
        {
            _signsDictionary = new Dictionary<string, TokenType>();

            _signsDictionary.Add("=", TokenType.ASSIGN);

            _signsDictionary.Add("+", TokenType.PLUS);
            _signsDictionary.Add("-", TokenType.MINUS);
            _signsDictionary.Add("*", TokenType.MULTI);
            _signsDictionary.Add("/", TokenType.DIVIDE);

            _signsDictionary.Add("&&", TokenType.AND);
            _signsDictionary.Add("||", TokenType.OR);
            _signsDictionary.Add("!", TokenType.NOT);

            _signsDictionary.Add("==", TokenType.EQUAL);
            _signsDictionary.Add("!=", TokenType.NO_EQUAL);
            _signsDictionary.Add(">=", TokenType.MORE_EQUAL);
            _signsDictionary.Add("<=", TokenType.LESS_EQUAL);
            _signsDictionary.Add(">", TokenType.MORE);
            _signsDictionary.Add("<", TokenType.LESS);

            _signsDictionary.Add("(", TokenType.BRACKET_ENTER);
            _signsDictionary.Add(")", TokenType.BRACKET_END);
            _signsDictionary.Add("{", TokenType.CURLY_BRACKET_ENTER);
            _signsDictionary.Add("}", TokenType.CURLY_BRACKET_END);

            _signsDictionary.Add(";", TokenType.SEMICOLON);
            _signsDictionary.Add(",", TokenType.COMMA);

        }

        bool CharIsWhite(char c)
        {
            if (char.IsWhiteSpace(c) || char.IsSeparator(c) || c == '\r' || c == '\t' || c == '\n')
            {
                return true;
            }
            return false;
        }

        bool CharIsBracket(char c)
        {
            if(c == '(' || c == ')' || c == '{' || c == '}')
            {
                return true;
            }
            return false;
        }

        bool CharIsCorrectSign(char c)
        {
            if (c == '=' || c == '<' || c == '>' || c == '!'
                || c == '+' || c == '-' || c == '*' || c == '/'
                || c == ';' || c == ',' || c == '|' || c == '&'
                )
            {
                return true;
            }
            return false;
        }
    }
}
