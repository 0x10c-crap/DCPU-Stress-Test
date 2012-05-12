using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Reflection;

namespace GenerateTestAssembly
{
    class Program
    {
        static void Main(string[] args)
        {
            OpcodeTable = new Dictionary<string, byte>();
            ValueTable = new Dictionary<string, byte>();
            NonBasicOpcodeTable = new Dictionary<string, byte>();
            int linesGenerated = 0, curLeft = Console.CursorLeft, curTop = Console.CursorTop;
            int fileNumber = 0;
            StreamWriter writer = new StreamWriter("stressTest_" + fileNumber + ".dasm");
            LoadTable();
            foreach (var kvp in OpcodeTable)
            {
                foreach (var valueA in ValueTable)
                {
                    if (valueA.Key.Contains("PUSH"))
                        continue;
                    foreach (var valueB in ValueTable)
                    {
                        if (valueB.Key.Contains("POP"))
                            continue;
                        string statement = kvp.Key;
                        ushort assembled = kvp.Value;
                        statement = statement.Replace("%a", valueA.Key);
                        statement = statement.Replace("%b", valueB.Key);
                        statement = statement.Replace("_", " ");
                        statement = statement.Replace(".,.", ", ");
                        statement = statement.Replace(".", "");
                        if (statement.Contains("$0"))
                        {
                            string statement1 = statement.Replace("$0", "0x1000");
                            string statement2 = statement.Replace("$0", "0x10");
                            writer.WriteLine(statement1);
                            writer.WriteLine(statement2);
                            linesGenerated++;
                        }
                        else
                            writer.WriteLine(statement);
                        linesGenerated++;
                        if (linesGenerated >= 10000)
                        {
                            writer.Flush();
                            writer.Close();
                            fileNumber++;
                            linesGenerated = 0;
                            writer = new StreamWriter("stressTest_" + fileNumber + ".dasm");
                        }
                        Console.SetCursorPosition(curLeft, curTop);
                        Console.Write(linesGenerated + " lines generated.");
                    }
                }
            }
            foreach (var kvp in NonBasicOpcodeTable)
            {
                foreach (var valueA in ValueTable)
                {
                    if (valueA.Key.Contains("PUSH"))
                        continue;
                    string statement = kvp.Key;
                    ushort assembled = kvp.Value;
                    statement = statement.Replace("%a", valueA.Key);
                    statement = statement.Replace("_", " ");
                    statement = statement.Replace(".,.", ", ");
                    statement = statement.Replace(".", "");
                    if (statement.Contains("$0"))
                    {
                        string statement1 = statement.Replace("$0", "0x1000");
                        string statement2 = statement.Replace("$0", "0x10");
                        writer.WriteLine(statement1);
                        writer.WriteLine(statement2);
                        linesGenerated++;
                    }
                    else
                        writer.WriteLine(statement);
                    linesGenerated++;
                    if (linesGenerated >= 10000)
                    {
                        writer.Flush();
                        writer.Close();
                        fileNumber++;
                        linesGenerated = 0;
                        writer = new StreamWriter("stressTest_" + fileNumber + ".dasm");
                    }
                    Console.SetCursorPosition(curLeft, curTop);
                    Console.Write(linesGenerated + " lines generated.");
                }
            }
            writer.Flush();
            writer.Close();
        }

        static void LoadTable()
        {
            StreamReader sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("GenerateTestAssembly.DCPUtable.txt"));
            string[] lines = sr.ReadToEnd().Replace("\r", "").Split('\n');
            sr.Close();
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;
                string[] parts = line.Split(' ');
                if (parts[0] == "o")
                    OpcodeTable.Add(parts[2], byte.Parse(parts[1], NumberStyles.HexNumber));
                else if (parts[0] == "n")
                    NonBasicOpcodeTable.Add(parts[2], byte.Parse(parts[1], NumberStyles.HexNumber));
                else if (parts[0] == "a,b")
                    ValueTable.Add(parts[2], byte.Parse(parts[1], NumberStyles.HexNumber));
            }
        }

        private static Dictionary<string, byte> OpcodeTable;
        private static Dictionary<string, byte> NonBasicOpcodeTable;
        private static Dictionary<string, byte> ValueTable;
    }
}
