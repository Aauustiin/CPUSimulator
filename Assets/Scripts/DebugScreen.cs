using System;
using TMPro;
using UnityEngine;

public class DebugScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI branchPredictorText;
    [SerializeField] private TextMeshProUGUI fetchText;
    [SerializeField] private TextMeshProUGUI decodeText;
    [SerializeField] private TextMeshProUGUI ratText;
    [SerializeField] private TextMeshProUGUI registersText;
    [SerializeField] private TextMeshProUGUI memoryText;
    [SerializeField] private TextMeshProUGUI reorderBufferText;
    [SerializeField] private TextMeshProUGUI reservationStationsText;
    [SerializeField] private TextMeshProUGUI integerArithmeticUnitText;
    [SerializeField] private TextMeshProUGUI branchText;
    [SerializeField] private TextMeshProUGUI loadStoreText;

    private void OnEnable()
    {
        EventManager.Tock += OnTock;
    }
    
    private void OnDisable()
    {
        EventManager.Tock -= OnTock;
    }

    private void OnTock(TockInfo info)
    {
        branchPredictorText.text = info.BranchPredictorInfo;
        fetchText.text = info.FetchUnitInfo;
        decodeText.text = info.DecodeUnitInfo;
        ratText.text = info.RegisterAllocationTableInfo;
        registersText.text = info.RegisterInfo;
        memoryText.text = info.MemoryInfo;
        reorderBufferText.text = info.ReorderBufferInfo;
        reservationStationsText.text = info.ReservationStationInfo;
        integerArithmeticUnitText.text = info.IntegerArithmeticUnitInfo;
        branchText.text = info.BranchUnitInfo;
        loadStoreText.text = info.LoadStoreUnitInfo;
    }
}
