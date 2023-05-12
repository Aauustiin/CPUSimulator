using System.Linq;
using UnityEngine;

public class Processor
{
    private FetchUnit[] _fetchUnits;
    private DecodeUnit[] _decodeUnits;
    public ReservationStation[] ReservationStations;

    public readonly int[] Registers;
    public readonly int?[] Scoreboard;
    public int[] RegisterAllocationTable;
    public ReorderBuffer ReorderBuffer;
    public int[] Memory;
    public Instruction[] Instructions;

    public int ProgramCounter;
    private int _cycle;
    public int InstructionsExecuted;
    private bool _finished;

    public ProcessorMode ProcessorMode;

    public Processor() {}

    private void Process()
    {
        while (!_finished)
        {
            // Step 1: Process the current data.

            foreach (var fetchUnit in _fetchUnits)
            {
                fetchUnit.Fetch();
            }

            foreach (var decodeUnit in _decodeUnits)
            {
                decodeUnit.Decode();
            }

            // Step 2: Assign new data.

            // TODO: Commit results from the reorder buffer to the physical register files?

            // TODO: Move results from execution units to the reorder buffer? 

            // TODO: Issue from reservation stations to free execution units

            // Assign decoded instructions to free reservation stations.
            var fullDecodeUnits = _decodeUnits.Where(decodeUnit => decodeUnit.HasOutput()).ToArray();
            var freeReservationStations = ReservationStations.Where(reservationStation =>
                reservationStation.GetState() == ReservationStationState.FREE).ToArray();
            for (var i = 0; (i < freeReservationStations.Count()) & (i < fullDecodeUnits.Count()); i++)
            {
                freeReservationStations[i].SetReservationStationData(fullDecodeUnits[i].Pop().Value);
            }

            // Assign fetched instructions to free decode units.
            var fullFetchUnits = _fetchUnits.Where(fetchUnit => fetchUnit.HasOutput());
            var emptyDecodeUnits = _decodeUnits.Where(decodeUnit => decodeUnit.IsFree());
            fullFetchUnits.Zip(emptyDecodeUnits, (fetchUnit, decodeUnit) => decodeUnit.Input = fetchUnit.Pop());
            
            _cycle++;
        }
    }

    public void Halt()
    {
        _finished = true;

        Debug.Log("Finished!");
        Debug.Log("Registers: " + string.Join(',', Registers));
        Debug.Log("Memory: " + string.Join(',', Memory));
        Debug.Log("Instructions Executed: " + InstructionsExecuted);
        Debug.Log("Cycles Taken: " + _cycle);
        Debug.Log("Instructions Per Cycle (IPC): " + ((float)InstructionsExecuted/_cycle).ToString("N2"));
    }
}

public enum ProcessorMode
{
    RELEASE,
    DEBUGC,
    DEBUGS
}