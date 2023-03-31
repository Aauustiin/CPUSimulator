using System;

public class ProcessorSimulator {
    private static void Main(string[] args)
    {
        Processor processor = new Processor(1, 16, 128, new Tuple<Opcode, int, int>[] { }, Mode.RELEASE);
    }
}
