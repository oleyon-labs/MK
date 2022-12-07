using SyntaxAn;
using SyntaxAn.BinaryTree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{

    public static class Translator
    {
        public static void Translate(List<Node<Token>> forest, bool debug, string filename)
        {
            File.WriteAllText(filename, Translate(forest, debug));
        }
        public static string Translate(List<Node<Token>> forest, bool debug)
        {
            List<string> ids = new List<string>();
            string result = "global  _main\n";
            string sectionData = "section .data\n";
            string sectionText = "section .text\n_main:\n";
            if (debug)
            {
                result += "extern printf\n";
                sectionData += "Message db \"%f\", 10, 0\n";
            }

            foreach (var tree in forest)
            {
                var id = tree.Children[0].Data;
                var idIndex = -1;
                if (!ids.Contains(id.Value))
                {
                    ids.Add(id.Value);
                    sectionData += $"{id.Value}: dd 0.0\n";
                }
                sectionText += TranslateEquation(tree.Children[1], ids);
                sectionText += $"pop dword [{id.Value}]\n";
                if (debug)
                {
                    sectionText += $"sub esp, 8\nmov ebx, {id.Value}\nfld dword [ebx]\nfstp qword [esp]\npush dword Message\ncall printf\nadd esp, 12\n";
                }
            }
            sectionText += "ret\n";
            result += sectionData + sectionText;
            return result;
        }

        private static string TranslateEquation(Node<Token> equation, List<string> ids)
        {
            var token = equation.Data;

            switch (equation.Children.Count)
            {
                case 0:
                    if (token.Type == TokenTypes.Id)
                    {
                        if (!ids.Contains(token.Value))
                            throw new Exception($"Id {equation.Data} is being used before assigning value to it");
                        else
                        {
                            return $"push dword [{token.Value}]\n";
                        }
                    }
                    else
                    {
                        var value = token.Value;
                        if (!token.Value.Contains('.') && !token.Value.Contains('e'))
                            value += ".0";
                        return $"push __float32__({value})\n";
                    }
                case 1:
                    var str = TranslateEquation(equation.Children[0], ids);
                    if (token.Value == "-")
                    {
                        str += $"fld dword [esp]\nfchs\nfstp dword [esp]\n";
                    }
                    return str;
                case 2:
                    var str2 = TranslateEquation(equation.Children[1], ids);
                    str2 += TranslateEquation(equation.Children[0], ids);
                    str2 += "fld dword [esp]\npop eax\n";
                    var op = "";
                    switch (token.Value)
                    {
                        case "-":
                            op = "fsub";
                            break;
                        case "+":
                            op = "fadd";
                            break;
                        case "*":
                            op = "fmul";
                            break;
                        case "/":
                            op = "fdiv";
                            break;
                    }
                    str2 += $"{op} dword [esp]\nfstp dword[esp]\n";
                    return str2;
                default: throw new Exception($"undefined operation {token}");
            }
        }
    }
}
