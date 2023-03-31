using System;

public class ProcessorSimulator {
    private static void Main(string[] args)
    {
        Processor processor = new Processor(1, 16);
        var program = Parsing.LoadProgram("put path here");
        processor.Initialise(new int[128], program, Mode.RELEASE);
        processor.Process();
    }
}
