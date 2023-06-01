using TMPro;
using UnityEngine;

public class UpdateText : MonoBehaviour
{
    private TextMeshProUGUI _text;
    
    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    public void OnValueChanged(float value)
    {
        _text.text = value.ToString();
    }
}
