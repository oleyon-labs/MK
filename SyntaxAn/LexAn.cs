using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAn
{
    public static class LexAn
    {
        static int[,] D1 = new int[,]
        {
        //    +-  09   .   e  */   (   )   :   =   ; a-z --- " \n"
            {  10, 1,-10,  7, 11, 13, 14,  8,-10, 12,  7,-10, -9}, //S0//
            { -2,  1,  2,  4, -2, -2, -2, -2, -2, -2,-10,-10, -2}, //S1//
            {-10,  3,-10,-10,-10,-10,-10,-10,-10,-10,-10,-10,-10}, //S2//
            { -2,  3,-10,  4, -2, -2, -2, -2, -2, -2,-10,-10, -2}, //S3//
            {  5,  6,-10,-10,-10,-10,-10,-10,-10,-10,-10,-10,-10}, //S4//
            {-10,  6,-10,-10,-10,-10,-10,-10,-10,-10,-10,-10,-10}, //S5//
            { -2,  6,-10, -2, -2, -2, -2, -2, -2, -2,-10,-10, -2}, //S6//
            { -1,  7,-10,  7, -1, -1, -1, -1, -1, -1,  7,-10, -1}, //S7//
            {-10,-10,-10,-10,-10,-10,-10,-10,  9,-10,-10,-10,-10}, //S8//
            { -3, -3,-10, -3, -3, -3, -3,-10,-10, -3, -3,-10, -3}, //S9//
            { -4, -4,-10, -4, -4, -4, -4,-10,-10, -4, -4,-10, -4}, //S10//
            { -5, -5,-10, -5, -5, -5, -5,-10,-10, -5, -5,-10, -5}, //S11//
            { -8, -8,-10, -8, -8, -8, -8,-10,-10, -8, -8,-10, -8}, //S12//
            { -6, -6,-10, -6, -6, -6, -6,-10,-10, -6, -6,-10, -6}, //S13//
            { -7, -7,-10, -7, -7, -7, -7,-10,-10, -7, -7,-10, -7}, //S14//
        };
        static int[] W = new int[10] { 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 };
        static string[] outString = new string[5] {
			"space",				// fin-1
			"(+-)DDD",				// fin-2
			"(+-)DDD.DDD",			// fin-3
			"(+-)DDD.DDDe(+-)DDD",	// fin-4
			"Error", };             // fin-5

        

        static int Sclass(char c)
        {
            switch (c)
            {
                case '+': case '-': return 0;
                case '.': return 2;
                case 'e': case 'E': return 3;
                case '*': case '/': return 4;
                case '(': return 5;
                case ')': return 6;
                case ':': return 7;
                case '=': return 8;
                case ';': return 9;
                default:
                    {
                        if(Char.IsDigit(c)) return 1;
                        if(Char.IsLetter(c)) return 10;
                        if (c == '\n' || c == '\0' || c == ' ' || c == '\r') return 12;
                        return 11;
                    }
}
        }
        private static Token? CreateToken(string tokenString, int state, int line, int column) => state switch
        {
            0 => new Token(TokenTypes.Id, tokenString, line, column), //идентификатор
            1 => new Token(TokenTypes.Num, tokenString, line, column),//число
            2 => new Token(TokenTypes.Eq, tokenString, line, column), //присваивание
            3 => new Token(TokenTypes.PM, tokenString, line, column), //+-
            4 => new Token(TokenTypes.MD, tokenString, line, column), //*/
            5 => new Token(TokenTypes.LB, tokenString, line, column), //(
            6 => new Token(TokenTypes.RB, tokenString, line, column), //)
            7 => new Token(TokenTypes.SC, tokenString, line, column), //;
            8 => null,                                                //пробелы
            _ => new Token(TokenTypes.Er, tokenString, line, column), //ошибка
        };
        public static List<Token> PerformAnalysis(string text)
        {
            int lineCounter = 1, positionCounter = 0;
            List<Token> tokenList = new List<Token>();
            text = text + "\0";
            int currentSymbolPos = 0;
            int scl;
            int currentState = 0;
            int tokenStart = 0;

            while(currentSymbolPos < text.Length)
            {
                scl = Sclass(text[currentSymbolPos]);
                currentState = D1[currentState, scl];

                if (text[currentSymbolPos] == '\n' || text[currentSymbolPos] == '\r')
                {
                    lineCounter++;
                    positionCounter = currentSymbolPos;
                }

                currentSymbolPos++;

                if(currentState < 0)
                {
                    currentSymbolPos -= W[-currentState - 1];
                    string currentToken = text.Substring(tokenStart, currentSymbolPos-tokenStart);

                    Token? token = CreateToken(currentToken.ToString(), -currentState - 1, lineCounter, tokenStart - positionCounter + 1);
                    if(token != null)
                        tokenList.Add(token);

                    currentState = 0;
                    tokenStart = currentSymbolPos;
                    
                }
            }
            tokenList.Add(new Token(TokenTypes.EL, "$", lineCounter, tokenStart - positionCounter));
            return tokenList;

        }
    }
}
