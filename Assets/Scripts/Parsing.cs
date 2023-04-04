using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class Parsing
{
    private static readonly Opcode[] LabelledOpcodes =
    {
        Opcode.BRANCHE,
        Opcode.BRANCHG,
        Opcode.BRANCHGE,
        Opcode.JUMP
    };
    
    public static Instruction[] LoadProgram(string filePath)
    {
        var program = new List<Instruction>();
        var lines = File.ReadAllLines(filePath).Select(x => x.Split(' ')).ToList();
        var labels = new Dictionary<string, string>();
        
        // First pass, gets program indexes from labels.
        for (var i = 0; i < lines.Count; i++)
        {
            lines[i] = lines[i].TakeWhile(x => x != "-").ToArray();
            
            if (lines[i][0] == "LABEL")
            {
                labels.Add(lines[i][1], i.ToString());
                lines.RemoveAt(i);
                i--;
            }
        }


        // Second pass, turns strings to Instructions
        foreach (var line in lines)
        {
            Enum.TryParse(line[0], out Opcode opcode);

            if (LabelledOpcodes.Contains(opcode))
            {
                line[^1] = labels[line.Last()];
            }
            
            var operands = new List<int>();
            for (var i = 1; i < line.Length; i++)
            {
                operands.Add(int.Parse(line[i]));
            }
            program.Add(new Instruction(opcode, operands));
        }
        
        program.Add(new Instruction(Opcode.HALT, new List<int>()));
        
        
        return program.ToArray();
    }
}
