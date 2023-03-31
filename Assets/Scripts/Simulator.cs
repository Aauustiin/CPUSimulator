using System;
using TMPro;
using UnityEngine;

public class Simulator : MonoBehaviour
{
    [SerializeField] private TMP_InputField input;
    
    private const string DefaultFilePath = "Assets\\Scripts\\Assembly\\Test\\Test.txt";
    private const Mode DefaultMode = Mode.DEBUGC;

    private Processor _processor;

    public void OnRun()
    {
        Simulate(input.text.Split(' '));
    }

    public void OnStep()
    {
        _processor.TriggerTick();
    }
    
    private void Simulate(string[] args)
    {
        string filePath;
        var a = Array.IndexOf(args, "-program");
        if (a != -1) filePath = args[a + 1];
        else filePath = DefaultFilePath;

        Mode mode;
        var b = Array.IndexOf(args, "-mode");
        if (b != -1) Enum.TryParse(args[b + 1], out mode);
        else mode = DefaultMode;

        _processor = new Processor(1, 16);
        var program = Parsing.LoadProgram(filePath);
        _processor.Process(new int[128], program, mode);
    }
}