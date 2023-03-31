using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class Parsing
{
    public static Tuple<Opcode, int, int>[] LoadProgram(string filePath)
    {
        var program = new List<Tuple<Opcode, int, int>>();
        var lines = File.ReadAllLines(filePath).ToList();
        foreach (string line in lines)
        {
            string[] tokens = line.Split(' ');
            Enum.TryParse(tokens[0], out Opcode opcode);
            var operandA = Int32.Parse(tokens[1]);
            var operandB = Int32.Parse(tokens[2]);
            var instruction = new Tuple<Opcode, int, int>(opcode, operandA, operandB);
            program.Add(instruction);
        }
        return program.ToArray();
    }
}
