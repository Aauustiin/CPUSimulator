using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipeline
{
    private FetchUnit _fetchUnit;
    private DecodeUnit _decodeUnit;
    private ReservationStation[] _reservationStations;

    public void OnTick()
    {
        // Step 1: Process anything you've got
        _fetchUnit.Fetch();
        _decodeUnit.Decode();
        // Execution Units execute things
        
        // Step 2: Get assigned new things
        // Reservation Stations Issue Things to Execution Units
        
        // Decode Units put things in reservation stations
        // Is there a decode unit with something to pass on?
        // Is there a free reservation station?
        // Put the 
        
        if (!_decodeUnit.IsBusy())
        {
            _decodeUnit.Input = _fetchUnit.Pop();
        }
        
        // Fetch unit fetches an instruction.
        // Decode unit decodes whatever instruction it has
        // Execution 
    }
}
