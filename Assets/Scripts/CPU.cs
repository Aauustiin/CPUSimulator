using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Opcode {
    ADD,
    ADDI,
    SUB,
    SUBI,
    MUL,
    DIV,
    MOD,
    COPY,
    COPYI,
    LOAD,
    LOADI,
    STORE,
    CMP,
    CMPI,
    BRANCHE,
    BRANCHG,
    JUMP,
    BREAK,
}

enum Mode
{
    Release,
    DebugC,
    DebugS,
}

public class CPU : MonoBehaviour
{
    private int[] _registers;
    private int[] _memory;
    private Tuple<Opcode, int, int>[] _instructions;
    private int _programCounter;
    private Mode _mode;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Tuple<Opcode, int, int> instruction = Fetch();
        Decode();
        Execute(instruction);
    }

    private Tuple<Opcode, int, int> Fetch()
    {
        Tuple<Opcode, int, int> instruction = _instructions[_programCounter];
        _programCounter++;
        return instruction;
    }
    
    private void Decode()
    {
        // Nothing to do here yet.
    }
    
    private void Execute(Tuple<Opcode, int, int> instruction)
    {
        switch (instruction.Item1)
        {
            case Opcode.ADD:
                _registers[instruction.Item2] += _registers[instruction.Item3];
                break;
            case Opcode.ADDI:
                _registers[instruction.Item2] += instruction.Item3;
                break;
            case Opcode.SUB:
                _registers[instruction.Item2] -= _registers[instruction.Item3];
                break;
            case Opcode.SUBI:
                _registers[instruction.Item2] -= instruction.Item3;
                break;
            case Opcode.MUL:
                _registers[instruction.Item2] *= _registers[instruction.Item3];
                break;
            case Opcode.DIV:
                _registers[instruction.Item2] /= _registers[instruction.Item3];
                break;
            case Opcode.MOD:
                _registers[instruction.Item2] %= _registers[instruction.Item3];
                break;
            case Opcode.COPY:
                _registers[instruction.Item2] = _registers[instruction.Item3];
                break;
            case Opcode.COPYI:
                _registers[instruction.Item2] = instruction.Item3;
                break;
            case Opcode.LOAD:
                _registers[instruction.Item2] = _memory[_registers[instruction.Item3]];
                break;
            case Opcode.LOADI:
                _registers[instruction.Item2] = _memory[instruction.Item3];
                break;
            case Opcode.STORE:
                _memory[_registers[instruction.Item2]] = _registers[instruction.Item3];
                break;
            case Opcode.CMP:
                if (_registers[instruction.Item2] == _registers[instruction.Item3])
                {
                    _registers[instruction.Item2] = 0;
                }
                else if (_registers[instruction.Item2] > _registers[instruction.Item3])
                {
                    _registers[instruction.Item2] = -1;
                }
                else _registers[instruction.Item2] = 1;
                break;
            case Opcode.CMPI:
                if (_registers[instruction.Item2] == instruction.Item3)
                {
                    _registers[instruction.Item2] = 0;
                }
                else if (_registers[instruction.Item2] > instruction.Item3)
                {
                    _registers[instruction.Item2] = -1;
                }
                else _registers[instruction.Item2] = 1;
                break;
            case Opcode.BRANCHE:
                if (_registers[instruction.Item2] == 0)
                {
                    _programCounter += instruction.Item3 - 1;
                }
                break;
            case Opcode.BRANCHG:
                if (_registers[instruction.Item2] == 1)
                {
                    _programCounter += instruction.Item3 - 1;
                }
                break;
            case Opcode.JUMP:
                _programCounter += instruction.Item2 - 1;
                break;
            case Opcode.BREAK:
                if (_mode == Mode.DebugC)
                {
                    _mode = Mode.DebugS;
                }
                break;
            default:
                break;
        }
    }
}
