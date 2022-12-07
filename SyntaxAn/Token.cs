using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAn
{
    public class Token
    {
        public TokenTypes Type { get; set; }
        public string Value { get; set; }
        public int linePos { get; set; }
        public int columnPos { get; set; }
        public Token(TokenTypes type, string value)
        {
            Type = type;
            Value = value;
        }
        public Token(TokenTypes type, string value, int linePos, int columnPos) : this(type, value)
        {
            this.linePos = linePos;
            this.columnPos = columnPos;
        }
        public override string ToString()
        {
            return $"Type: {Type}, Value: {Value}, Line: {linePos}, Symbol: {columnPos}";
        }
        public bool IsOperation()
        {
            switch(Type)
            {
                case TokenTypes.Eq:
                case TokenTypes.MD:
                case TokenTypes.PM:
                    return true;
                default: return false;
            }
        }
        public bool IsOperationTreeToken()
        {
            return(IsOperation() || IsTerminalOperationTreeToken());
        }
        public bool IsTerminalOperationTreeToken()
        {
            return (Type == TokenTypes.Id || Type == TokenTypes.Num);
        }
    }
}
