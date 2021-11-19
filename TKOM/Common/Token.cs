using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKOM.Common
{
    public enum TokenType
    {
        // SingleToken
        PLUS,
        MINUS,
        MULTI,
        DIVIDE,

        ASSIGN,

        OR,
        AND,

        EQUAL,
        NO_EQUAL,
        MORE,
        MORE_EQUAL,
        LESS,
        LESS_EQUAL,

        SEMICOLON,
        COMMA,

        BRACKET_ENTER,
        BRACKET_END,

        CURLY_BRACKET_ENTER,
        CURLY_BRACKET_END,

        //Keywords

        TRY,
        CATCH,
        IF,
        ELSE,
        WHILE,
        RETURN,
        THROW,

        INT,
        TRUE,
        FALSE,

        IDENTIFIRE,
        NUMBER,

        UNKNOWN
    }

    public class Token
    {
        string _text;
        TokenType _tokenType;
        (int, int) _position;

        public Token(TokenType tokenType, string text, (int,int) position)
        {
            _tokenType = tokenType;
            _text = text;
            _position = position;
        }

        public TokenType TokenType { get => _tokenType; }

        public string Text { get => _text; }

        public (int, int) Position { get => _position; }

        public int? GetIntValue()
        {
            if(_tokenType != TokenType.NUMBER)
            {
                return null;
            }
            return int.Parse(_text);
        }

    }
}
