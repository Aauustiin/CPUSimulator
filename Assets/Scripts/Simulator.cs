using System;
using TMPro;
using UnityEngine;

public class Simulator : MonoBehaviour
{
    [SerializeField] private TMP_InputField input;
    
    private const string DefaultFilePath = "Assets\\Scripts\\Assembly\\Test\\Test.txt";
    private const ProcessorMode DefaultMode = ProcessorMode.DEBUGC;

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

        ProcessorMode processorMode;
        var b = Array.IndexOf(args, "-mode");
        if (b != -1) Enum.TryParse(args[b + 1], out processorMode);
        else processorMode = DefaultMode;

        _processor = new Processor(1, 16);
        var program = Parsing.LoadProgram(filePath);
        _processor.Process(new int[3] {2, 1, 4}, program, processorMode);
    }
}