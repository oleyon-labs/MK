using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAn
{
    internal class LexAnOld
    {
		int[,] D = new int[8, 5]
		{ // +- 09 .  e  ---
			{ 1, 2,-5,-5,-1 }, //S0
			{-5, 2,-5,-5,-5 }, //S1
			{-5, 2, 3, 5,-2 }, //S2
			{-5, 4,-5,-5,-5 }, //S3
			{-5, 4,-5, 5,-3 }, //S4
			{ 6, 7,-5,-5,-5 }, //S5
			{-5, 7,-5,-5,-5 }, //S6
			{-5, 7,-5,-5,-4 }, //S7
		};
        int[,] D1 = new int[,]
        {
        //    +-  09   .   e  */   (   )   :   =   ; a-z --- " \n"
            {  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13}, //S0
            {  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13}, //S0
            {  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13}, //S0
            {  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13}, //S0
            {  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13}, //S0
        };
        int[] W = new int[5] { 0, 1, 1, 1, 0 };
        string[] outString = new string[5] {
			"space",				// fin-1
			"(+-)DDD",				// fin-2
			"(+-)DDD.DDD",			// fin-3
			"(+-)DDD.DDDe(+-)DDD",	// fin-4
			"Error", };             // fin-5

        

        int Sclass(char c)
        {
            switch (c)
            {
                case '+': case '-': return (0);
                case '.': return (2);
                case 'e': case 'E': return (3);
                default:
                    {
                        if (Char.IsDigit(c)) return 1;
                        return 4;
                    }
            }
        }
        private Token? CreateToken(string tokenString, int state) => state switch
        {
            0 => new Token(TokenTypes.Id, tokenString), //идентификатор
            1 => new Token(TokenTypes.Num, tokenString),//число
            2 => new Token(TokenTypes.Eq, tokenString), //присваивание
            3 => new Token(TokenTypes.PM, tokenString), //+-
            4 => new Token(TokenTypes.MD, tokenString), //*/
            5 => new Token(TokenTypes.LB, tokenString), //(
            6 => new Token(TokenTypes.RB, tokenString), //)
            7 => new Token(TokenTypes.SC, tokenString), //;
            8 => null,                                  //пробелы
            _ => new Token(TokenTypes.Er, tokenString), //ошибка
        };
        public List<Token> PerformAnalysis(string text)
        {
            List<Token> tokenList = new List<Token>();
            text = text + "\0";
            int currentSymbolPos = 0;
            int scl;
            int currentState = 0;
            int tokenStart = 0;
            //StringBuilder currentToken = new StringBuilder();

            while(currentSymbolPos < text.Length)
            {
                //if(currentState == 0) tokenStart = currentSymbolPos;

                scl = Sclass(text[currentSymbolPos]);

                Console.Write($"{text[currentSymbolPos]}[{currentState}->");
                currentState = D[currentState, scl];
                Console.WriteLine($"{currentState}]");


                //currentToken.Append(text[currentSymbolPos]);

                currentSymbolPos++;

                if(currentState < 0)
                {
                    currentSymbolPos -= W[-currentState - 1];
                    string currentToken = text.Substring(tokenStart, currentSymbolPos-tokenStart);
                    Console.WriteLine($"({W[-currentState - 1]})<- {outString[-currentState - 1]} {currentToken}|");

                    
                    Token? token = CreateToken(currentToken.ToString(), -currentState - 1);
                    if(token != null)
                        tokenList.Add(token);
                    //currentToken.Clear();
                    

                    currentState = 0;
                    tokenStart = currentSymbolPos;
                    
                }
            }
            return tokenList;

        }
    }
}
