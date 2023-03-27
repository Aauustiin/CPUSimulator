using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutionUnit : MonoBehaviour
{
    //void Execute(Tuple<Opcode, int, int> instruction, CPU cpu)
    //{
    //    switch (instruction.Item1)
    //    {
    //        case Opcode.ADD:
    //            cpu.registers[instruction.Item2] += _registers[instruction.Item3];
    //            break;
    //        case Opcode.ADDI:
    //            _registers[instruction.Item2] += instruction.Item3;
    //            break;
    //        case Opcode.SUB:
    //            _registers[instruction.Item2] -= _registers[instruction.Item3];
    //            break;
    //        case Opcode.SUBI:
    //            _registers[instruction.Item2] -= instruction.Item3;
    //            break;
    //        case Opcode.MUL:
    //            _registers[instruction.Item2] *= _registers[instruction.Item3];
    //            break;
    //        case Opcode.DIV:
    //            _registers[instruction.Item2] /= _registers[instruction.Item3];
    //            break;
    //        case Opcode.MOD:
    //            _registers[instruction.Item2] %= _registers[instruction.Item3];
    //            break;
    //        case Opcode.COPY:
    //            _registers[instruction.Item2] = _registers[instruction.Item3];
    //            break;
    //        case Opcode.COPYI:
    //            _registers[instruction.Item2] = instruction.Item3;
    //            break;
    //        case Opcode.LOAD:
    //            _registers[instruction.Item2] = _memory[_registers[instruction.Item3]];
    //            break;
    //        case Opcode.LOADI:
    //            _registers[instruction.Item2] = _memory[instruction.Item3];
    //            break;
    //        case Opcode.STORE:
    //            _memory[_registers[instruction.Item2]] = _registers[instruction.Item3];
    //            break;
    //        case Opcode.CMP:
    //            if (_registers[instruction.Item2] == _registers[instruction.Item3])
    //            {
    //                _registers[instruction.Item2] = 0;
    //            }
    //            else if (_registers[instruction.Item2] > _registers[instruction.Item3])
    //            {
    //                _registers[instruction.Item2] = -1;
    //            }
    //            else _registers[instruction.Item2] = 1;
    //            break;
    //        case Opcode.CMPI:
    //            if (_registers[instruction.Item2] == instruction.Item3)
    //            {
    //                _registers[instruction.Item2] = 0;
    //            }
    //            else if (_registers[instruction.Item2] > instruction.Item3)
    //            {
    //                _registers[instruction.Item2] = -1;
    //            }
    //            else _registers[instruction.Item2] = 1;
    //            break;
    //        case Opcode.BRANCHE:
    //            if (_registers[instruction.Item2] == 0)
    //            {
    //                _programCounter += instruction.Item3 - 1;
    //            }
    //            break;
    //        case Opcode.BRANCHG:
    //            if (_registers[instruction.Item2] == 1)
    //            {
    //                _programCounter += instruction.Item3 - 1;
    //            }
    //            break;
    //        case Opcode.JUMP:
    //            _programCounter += instruction.Item2 - 1;
    //            break;
    //        case Opcode.BREAK:
    //            if (_mode == Mode.DebugC)
    //            {
    //                _mode = Mode.DebugS;
    //            }
    //            break;
    //        default:
    //            break;
    //    }
    //}
}
