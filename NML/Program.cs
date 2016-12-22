namespace NML {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class Program {
        private static int res;
        private static readonly int[] gx = new int[10];
        private static readonly Dictionary<string, int> jumpMap = new Dictionary<string, int>();
        private static Dictionary<string, int> memory = new Dictionary<string, int>();

        private static void Main(string[] args) {
#if !DEBUG
            string sourceFile = null;

            if (args.Length > 0) {
                sourceFile = args[0];
            } else {
                Console.WriteLine("No source file\n");
                Environment.Exit(1);
            }
            var sourceLines = File.ReadAllLines(sourceFile);
#else
            var sourceLines = new[] { "var lhs 2",
"var rhs 3",
"var acc 0",
"label loop",
"add acc lhs acc",
"sub rhs 1 rhs",
"jnz rhs loop",
"print acc" };
#endif

            for (var i = 0; i < sourceLines.Length; i++) {
                var tokens = sourceLines[i].Split(' ').ToList();

                switch (tokens[0]) {
                    case "add": {
                        var a = ResolveValue(tokens[1]);
                        var b = ResolveValue(tokens[2]);

                        var dst = "RES";
                        if (tokens.Count == 4) {
                            dst = tokens[3];
                        }

                        SetRegister(dst, a + b);
                        break;
                    }
                    case "sub": {
                        var a = ResolveValue(tokens[1]);
                        var b = ResolveValue(tokens[2]);

                        var dst = "RES";
                        if (tokens.Count == 4) {
                            dst = tokens[3];
                        }

                        SetRegister(dst, a - b);
                        break;
                    }
                    case "print": {
                        Console.WriteLine(ResolveValue(tokens[1]));
                        break;
                    }
                    case "mov": {
                        var dst = tokens[1];

                        if (dst == "RES") {
                            throw new Exception("Register RES is not assignable");
                        }

                        var src = tokens[2];
                        var val = ResolveValue(src);
                        SetRegister(dst, val);
                        SetRegister(src, 0);
                        break;
                    }
                    case "var": {
                            memory[tokens[1]] = ResolveValue(tokens[2]);
                        break;
                    }
                    case "label": {
                            jumpMap[tokens[1]] = i;
                            break;
                    }
                    case "jmp": {
                        if (jumpMap.ContainsKey(tokens[1])) {
                            i = jumpMap[tokens[1]];
                        } else {
                            throw new Exception(tokens[1] + " is not a registered as a jump point");
                        }
                        break;
                    }
                    case "jez": {
                        if ((ResolveValue(tokens[1]) == 0) && jumpMap.ContainsKey(tokens[2])) {
                            i = jumpMap[tokens[2]];
                        }
                        break;
                    }
                    case "jnz": {
                        if ((ResolveValue(tokens[1]) != 0) && jumpMap.ContainsKey(tokens[2])) {
                            i = jumpMap[tokens[2]];
                        }
                        break;
                    }
                    case "jeq": {
                        if ((ResolveValue(tokens[1]) == ResolveValue(tokens[2])) && jumpMap.ContainsKey(tokens[3])) {
                            i = jumpMap[tokens[3]];
                        }
                        break;
                    }
                }
            }
        }

        private static int ResolveValue(string token) {
            if (token == "RES") {
                return res;
            }

            int x;
            if ((token.Length == 2) && (token[0] == 'G') && int.TryParse(token[1].ToString(), out x)) {
                return gx[x];
            }

            if (memory.ContainsKey(token)) {
                return memory[token];
            }

            if (int.TryParse(token, out x)) {
                return x;
            }

            throw new Exception(token + " is not a valid register, variabel, or integer");
        }

        private static void SetRegister(string name, int value) {
            if (name == "RES") {
                res = value;
            }

            if (memory.ContainsKey(name)) {
                memory[name] = value;
            }

            if ((name.Length == 2) && (name[0] == 'G')) {
                int x;
                if (int.TryParse(name[1].ToString(), out x)) {
                    gx[x] = value;
                }
            }
        }
    }
}
