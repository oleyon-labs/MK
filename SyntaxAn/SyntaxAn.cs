using SyntaxAn.BinaryTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAn
{
    public static class SyntaxAnalyzer
    {
        static string[,] ACTION = new string[,]
        {
        //      i     n     p     m     a     c     (     )     $
            { "r2", "s6", "s7",  " ",  " ",  " ", "s8",  " ", "r2"}, //I0/
            { "s9",  " ",  " ",  " ",  " ",  " ",  " ",  " ",  "a"}, //I1/
            {  " ",  " ","s10",  " ",  " ",  " ",  " ",  " ",  " "}, //I2/
            {  " ",  " ", "r5","s11",  " ", "r5",  " ", "r5",  " "}, //I3/
            {  " ",  " ", "r7", "r7",  " ", "r7",  " ", "r7",  " "}, //I4/
            {  " ",  " ", "r8", "r8",  " ", "r8",  " ", "r8",  " "}, //I5/
            {  " ",  " ", "r9", "r9",  " ", "r9",  " ", "r9",  " "}, //I6/
            { "s5", "s6",  " ",  " ",  " ",  " ", "s8",  " ",  " "}, //I7/
            { "s5", "s6", "s7",  " ",  " ",  " ", "s8",  " ",  " "}, //I8/
            {  " ",  " ",  " ",  " ","s14",  " ",  " ",  " ",  " "}, //I9/
            { "s5", "s6",  " ",  " ",  " ",  " ", "s8",  " ",  " "}, //I10/
            { "s5", "s6",  " ",  " ",  " ",  " ", "s8",  " ",  " "}, //I11/
            {  " ",  " ", "r4","s11",  " ", "r4",  " ", "r4",  " "}, //I12/
            {  " ",  " ","s10",  " ",  " ",  " ",  " ","s17",  " "}, //I13/
            { "s5", "s6", "s7",  " ",  " ",  " ", "s8",  " ",  " "}, //I14/
            {  " ",  " ", "r3","s11",  " ", "r3",  " ", "r3",  " "}, //I15/
            {  " ",  " ", "r6", "r6",  " ", "r6",  " ", "r6",  " "}, //I16/
            {  " ",  " ","r10","r10",  " ","r10",  " ","r10",  " "}, //I17/
            {  " ",  " ","s10",  " ",  " ","s19",  " ",  " ",  " "}, //I18/
            { "r1",  " ",  " ",  " ",  " ",  " ",  " ",  " ", "r1"}, //I19/
        };

        static int[,] GOTO = new int[,]
        {
        //     S   T   M   V
            {  1,  2,  3,  4},//I0
            { -1, -1, -1, -1},//I1
            { -1, -1, -1, -1},//I2
            { -1, -1, -1, -1},//I3
            { -1, -1, -1, -1},//I4
            { -1, -1, -1, -1},//I5
            { -1, -1, -1, -1},//I6
            { -1, -1, 12,  4},//I7
            { -1, 13,  3,  4},//I8
            { -1, -1, -1, -1},//I9
            { -1, -1, 15,  4},//I10
            { -1, -1, -1, 16},//I11
            { -1, -1, -1, -1},//I12
            { -1, -1, -1, -1},//I13
            { -1, 18,  3,  4},//I14
            { -1, -1, -1, -1},//I15
            { -1, -1, -1, -1},//I16
            { -1, -1, -1, -1},//I17
            { -1, -1, -1, -1},//I18
            { -1, -1, -1, -1},//I19
        };

        //правила
        /*I0={  (0) S0->.S
                (1) S->.SiaTc
                (2) S->.
                (3) T->.TpM
                (4) T->.pM
                (5) T->.M
                (6) M->.MmV
                (7) M->.V
                (8) V->.i
                (9) V->.n
                (10) V->.(T) }
        */

        static Dictionary<string, (char, int)> REDUCE = new Dictionary<string, (char, int)>
        {
            {"r1", ('S', 5) },
            {"r2", ('S', 0) },
            {"r3", ('T', 3) },
            {"r4", ('T', 2) },
            {"r5", ('T', 1) },
            {"r6", ('M', 3) },
            {"r7", ('M', 1) },
            {"r8", ('V', 1) },
            {"r9", ('V', 1) },
            {"r10", ('V', 3) },
        };

        public static int Tclass(Token t)
        {
            switch(t.Type)
            {
                case TokenTypes.Id: return 0;
                case TokenTypes.Num: return 1;
                case TokenTypes.Eq: return 4;
                case TokenTypes.PM: return 2;
                case TokenTypes.MD: return 3;
                case TokenTypes.LB: return 6;
                case TokenTypes.RB: return 7;
                case TokenTypes.SC: return 5;
                case TokenTypes.Er: return -1;
                case TokenTypes.EL: return 8;
                default: return -1;
            }
        }
        public static int Tclass(char l)
        {
            switch(l)
            {
                case 'S': return 0;
                case 'T': return 1;
                case 'M': return 2;
                case 'V': return 3;
                default: return -1;
            }
        }

        public static Node<Token> PerformAnalysis(List<Token> tokens)
        {
            int currentState=0;
            int currentTokenI = 0;
            bool accept = false;

            Stack<Token> tokenStack = new Stack<Token>();
            Stack<int> stateStack = new Stack<int>();
            stateStack.Push(currentState);
            Stack<Node<Token>> nodeStack = new Stack<Node<Token>>();
            while (!accept)
            {
                var currentToken = tokens[currentTokenI];
                currentState = stateStack.Peek();
                int currentTokenClass = Tclass(currentToken);
                if (currentTokenClass < 0)
                    throw new Exception($"Ошибка лексического анализа на лексеме {currentToken}");
                string action = ACTION[currentState, currentTokenClass];

                switch(action[0])
                {
                    case 's':
                        stateStack.Push(Int32.Parse(action.Substring(1)));
                        tokenStack.Push(tokens[currentTokenI]);
                        currentTokenI++;
                        break;
                    case 'r':
                        var (l,c) = REDUCE[action];
                        List<Node<Token>> nodes = new();
                        for(int i = 0; i < c; i++)
                        {
                            stateStack.Pop();
                            //формируем дерево
                            var ct = tokenStack.Peek();
                            tokenStack.Pop();

                            if(ct.Type==TokenTypes.NT)
                            {
                                nodes.Add(nodeStack.Peek());
                                nodeStack.Pop();
                            }
                            else
                            {
                                Node<Token> newNode1 = new(ct);
                                nodes.Add(newNode1);
                            }
                        }
                        currentState = stateStack.Peek();
                        stateStack.Push(GOTO[currentState, Tclass(l)]);
                        tokenStack.Push(new Token(TokenTypes.NT, l.ToString()));

                        Node<Token> newNode = new(tokenStack.Peek());
                        nodes.Reverse();
                        newNode.Children = nodes;

                        nodeStack.Push(newNode);

                        break;
                    case 'a':
                        accept = true;
                        break;
                    case ' ':
                        throw new Exception($"Ошибка синтаксического анализа на лексеме {currentToken}");
                }
            }
            //if (tokenStack.Count != 0)
            //{
            //    Console.WriteLine("оставшиеся токены");
            //    foreach(Token token in tokenStack)
            //    { Console.WriteLine($"st: {token}"); }
            //}
            return nodeStack.Peek();
        }

        public static List<Node<Token>> GetOperationForest(Node<Token> tree)
        {
            List<Node<Token>> result = new List<Node<Token>>();
            tree = tree.Copy();
            while (tree.Children.Count > 0 &&tree.Children[0].Data.Value == "S")
            {
                var nextTree = tree.Children[0];
                tree.Children.RemoveAt(0);
                result.Add(GetOperationTree(tree));
                tree=nextTree;
            }
            result.Reverse();
            return result;
        }
        private static Node<Token> GetOperationTree(Node<Token> tree)
        {
            if (tree.Data.IsTerminalOperationTreeToken())
            {
                return tree;
            }
            else if (tree.Data.Type == TokenTypes.NT && tree.Children.Count>0)
            {
                tree.Data = null;
                var node = new Node<Token>();
                for (int i = 0; i < tree.Children.Count; i++)
                {
                    var child = tree.Children[i];
                    if (child.Data.IsOperation())
                        node.Data = child.Data;
                    else
                    {
                        var childTree = GetOperationTree(child);
                        if (childTree != null)
                            node.Children.Add(childTree);
                    }
                }
                if (node.Data is null)
                    node = node.Children[0];
                return node;
            }
            else return null;
        }
    }
}
