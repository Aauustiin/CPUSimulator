using System;
using TMPro;
using UnityEngine;

public class Simulator : MonoBehaviour
{
    [SerializeField] private TMP_InputField input;
    
    private const string DefaultFilePath = "Assets\\Scripts\\Assembly\\Test\\Test.txt";
    private const Mode DefaultMode = Mode.RELEASE;

    public void OnRun()
    {
        Simulate(input.text.Split(' '));
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

        Processor processor = new Processor(1, 16);
        var program = Parsing.LoadProgram(filePath);
        processor.Initialise(new int[128], program, mode);
        processor.Process();
    }
}