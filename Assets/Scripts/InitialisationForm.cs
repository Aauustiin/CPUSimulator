using System;
using TMPro;
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

    [SerializeField] private TMP_Dropdown predictorDropdown;
    [SerializeField] private TMP_Dropdown modeDropdown;
    [SerializeField] private TMP_Dropdown programDropdown;

    [SerializeField] private Toggle registerRenamingToggle;

    public void OnSubmit()
    {
        var processorSpecification = new ProcessorSpecification((int)fetchSlider.value, (int)decodeSlider.value,
            (int)integerArithmeticSlider.value, (int)branchSlider.value, (int)loadStoreSlider.value, (int)reservationSlider.value,
            (int)registersSlider.value, 8, (int)reorderSlider.value,
            predictorDropdown.captionText.text == "Dynamic",
            GetMode(modeDropdown.captionText.text),
            registerRenamingToggle.isOn);
        var programSpecification = GetProgramSpecification();
        
        var processor = new Processor(processorSpecification);
        processor.Process(programSpecification);
        
        gameObject.SetActive(false);
    }

    private ProgramSpecification GetProgramSpecification()
    {
        var filePath = programDropdown.captionText.text switch
        {
            "Sum" => "Assets\\Scripts\\Assembly\\Tests\\Sum.txt",
            "Simple Sum" => "Assets\\Scripts\\Assembly\\Tests\\SimpleSum.txt",
            "Super Simple Sum" => "Assets\\Scripts\\Assembly\\Tests\\SuperSimpleSum.txt",
            _ => throw new ArgumentOutOfRangeException()
        };
        var memory = programDropdown.captionText.text switch
        {
            "Sum" => new int[] { 5, 2, 3, 4, 5, 6 },
            "Simple Sum" => new int[] {},
            "Super Simple Sum" => new int[] {},
            _ => throw new ArgumentOutOfRangeException()
        };
        var program = Parsing.LoadProgram(filePath);
        return new ProgramSpecification(program, memory);
    }

    private ProcessorMode GetMode(string choice)
    {
        return choice switch
        {
            "RELEASE" => ProcessorMode.RELEASE,
            "DEBUGC" => ProcessorMode.DEBUGC,
            "DEBUGS" => ProcessorMode.DEBUGS,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}