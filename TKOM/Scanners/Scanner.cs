﻿using System;
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
        Token _token;
        Dictionary<string, TokenType> _wordsDictionary;
        Dictionary<string, TokenType> _signsDictionary;

        public Scanner(ICharReader charReader)
        {
            if(charReader == null)
            {
                throw new ArgumentNullException();
            }
            _charReader = charReader;
            _charReader.MoveToNextChar();

            _token = null;

            InitSignsDictionary();
            InitWordDictionary();
        }

        public Token CurrentToken => _token;

        public bool MoveToNextToken()
        {
            //Omit white signs
            while(CharIsWhite(_charReader.CurrentChar))
            {
                _charReader.MoveToNextChar();
            }

            //Check if it is end sign of char reader
            if (_charReader.CurrentChar == (char)0)
            {
                _token = null;
                return false;
            }

            //Position of first char in Token
            (int, int) position = _charReader.CurrentPosition;

            //Keyword or Identyfier
            if (char.IsLetter(_charReader.CurrentChar))
            {
                StringBuilder stringBuilder = new StringBuilder();
                while(char.IsLetterOrDigit(_charReader.CurrentChar))
                {
                    stringBuilder.Append(_charReader.CurrentChar);
                    _charReader.MoveToNextChar();
                }
                string text = stringBuilder.ToString();

                TokenType type;
                if(_wordsDictionary.TryGetValue(text, out type))
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
                if(char.IsLetter(_charReader.CurrentChar))
                {
                    while (char.IsLetterOrDigit(_charReader.CurrentChar))
                    {
                        stringBuilder.Append(_charReader.CurrentChar);
                        _charReader.MoveToNextChar();
                    }

                    string textWrongToken = stringBuilder.ToString();
                    _token = new Token(TokenType.UNKNOWN, textWrongToken, position);

                    // TODO : Commit wrong identyfier or number error to IError 

                    return true;
                }

                string text = stringBuilder.ToString();
                _token = new Token(TokenType.NUMBER, text, position);
                return true;
            }

            //Brackets singel Token
            if(CharIsBracket(_charReader.CurrentChar))
            {
                string text = _charReader.CurrentChar.ToString();
                TokenType type;
                if(!_signsDictionary.TryGetValue(text, out type))
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

                // TODO: Commit wrong operator error to IError 

                _token = new Token(TokenType.UNKNOWN, text, position);

                return true;
            }


            // Incorect Sign
            _token = new Token(TokenType.UNKNOWN, _charReader.CurrentChar.ToString(), position);
            _charReader.MoveToNextChar();

            // TODO: Commit incorect sign error to IError

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
            if (char.IsWhiteSpace(c) || char.IsSeparator(c) || c == '\t' || c == '\n')
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
