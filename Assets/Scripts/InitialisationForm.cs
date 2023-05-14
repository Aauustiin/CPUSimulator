using System;
using UnityEngine;
using UnityEngine.UI;

public class InitialisationForm : MonoBehaviour
{
    [SerializeField] private Slider fetchSlider;
    [SerializeField] private Slider decodeSlider;
    [SerializeField] private Slider registersSlider;
    [SerializeField] private Slider integerArithmeticSlider;
    [SerializeField] private Slider branchSlider;
    [SerializeField] private Slider loadStoreSlider;
    [SerializeField] private Slider reservationSlider;
    [SerializeField] private Slider reorderSlider;

    [SerializeField] private Dropdown predictorDropdown;
    [SerializeField] private Dropdown modeDropdown;
    [SerializeField] private Dropdown programDropdown;

    public void OnSubmit()
    {
        var processorSpecification = new ProcessorSpecification((int)fetchSlider.value, (int)decodeSlider.value,
            (int)integerArithmeticSlider.value, (int)branchSlider.value, (int)loadStoreSlider.value, (int)reservationSlider.value,
            (int)registersSlider.value, 8, (int)reorderSlider.value,
            predictorDropdown.captionText.text == "Dynamic" ? true : false,
            GetMode(modeDropdown.captionText.text));
        var programSpecification = GetProgramSpecification();
        
        var processor = new Processor(processorSpecification);
        processor.Process(programSpecification);
        
        // Will have to set monitoring UI stuff to be active.
        gameObject.SetActive(false);
    }

    private ProgramSpecification GetProgramSpecification()
    {
        var filePath = programDropdown.captionText.text switch
        {
            "Sum" => "Assets\\Scripts\\Assembly\\Test\\Sum.txt",
            _ => throw new ArgumentOutOfRangeException()
        };
        // This will have to be different things depending on the demonstration.
        var memory = new int[] { 1, 2, 3, 4, 5, 6 };
        var program = Parsing.LoadProgram(filePath);
        return new ProgramSpecification(program, memory);
    }

    private ProcessorMode GetMode(string choice)
    {
        return choice switch
        {
            "Release" => ProcessorMode.RELEASE,
            "Debug Continuous" => ProcessorMode.DEBUGC,
            "Debug Step-Through" => ProcessorMode.DEBUGS,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}