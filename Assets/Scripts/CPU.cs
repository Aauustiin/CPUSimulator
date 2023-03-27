using System;
using System.Collections.Generic;
using UnityEngine;

public class CPU : MonoBehaviour
{
    private FetchUnit _fetchUnit;
    private List<Tuple<Opcode, int, int>> _fetchDecodeBuffer;
    private DecodeUnit _decodeUnit;
    private List<Tuple<Opcode, int, int>> _decodeExecuteBuffer;
    private List<ExecutionUnit> _executionUnits;

    private int[] _registers;
    private int[] _memory;
    private Tuple<Opcode, int, int>[] _instructions;

    private int _programCounter;
    private int _cycle;

    private Mode _mode;

    private void OnTick()
    {
        // Fetch unit fetch some instructions and put them in buffer.
        // Decode unit decode some instructions and put them in buffer.
        // Execution units execute some instructions and ?? Write back? Write to architectural register file or something else?
    }
}
