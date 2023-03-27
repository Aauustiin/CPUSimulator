using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    public static event System.Action Tick;

    public static void TriggerTick()
    {
        Tick?.Invoke();
    }

    public static event System.Action<Tuple<Opcode, int, int>> InstructionDecoded;

    public static void TriggerInstructionDecoded(Tuple<Opcode, int, int> instruction)
    {
        InstructionDecoded?.Invoke(instruction);
    }
}
