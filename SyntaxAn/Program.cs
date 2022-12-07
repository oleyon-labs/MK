﻿
// 1.Входной язык содержит арифметические выражения, разделённые символом
// ; (точка с запятой). Арифметические выражения состоят из идентификаторов,
// десятичных чисел с плавающей точкой (в обычной и экспоненциальной форме),
// знака присваивания(:=), знаков операций +, –, *, / и круглых скобок. 

namespace SyntaxAn
{
    public class Program
    {
        public static void Main()
        {
            var tokens = LexAn.PerformAnalysis(File.ReadAllText("Examples/t1.txt"));
            foreach (var token in tokens)
                Console.WriteLine(token);
            var syntaxTree = SyntaxAnalyzer.PerformAnalysis(tokens);
            syntaxTree.PrintPretty("", true);
            var operationForest = SyntaxAnalyzer.GetOperationForest(syntaxTree);
            Console.WriteLine("-------------------------");
            Console.WriteLine("Деревья операций:");
            foreach(var operationTree in operationForest)
            {
                operationTree.PrintPretty("", true, (token) => { Console.WriteLine(token.Value); });
                Console.WriteLine("-------------------------");
            }
            Console.ReadLine();
        }
    }
}
