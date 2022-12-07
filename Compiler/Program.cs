using SyntaxAn;
// 1.Входной язык содержит арифметические выражения, разделённые символом
// ; (точка с запятой). Арифметические выражения состоят из идентификаторов,
// десятичных чисел с плавающей точкой (в обычной и экспоненциальной форме),
// знака присваивания(:=), знаков операций +, –, *, / и круглых скобок. 

namespace Compiler
{
    public class Program
    {
        public static void Main()
        {
            var tokens = LexAn.PerformAnalysis(File.ReadAllText("Examples/t1.txt"));
            var syntaxTree = SyntaxAnalyzer.PerformAnalysis(tokens);
            var operationForest = SyntaxAnalyzer.GetOperationForest(syntaxTree);
            //Console.WriteLine(Translator.Translate(operationForest, true, "F:\\Programms\\assembler\\results\\out.asm"));
            Translator.Translate(operationForest, true, "F:\\Programms\\assembler\\results\\out.asm");
        }
    }
}
