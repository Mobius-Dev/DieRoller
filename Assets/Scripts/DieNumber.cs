using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class DieNumber : MonoBehaviour
{
    public int Number => _number;

    [SerializeField] private int _number;

    private TextMeshPro _numberText;

    private void OnValidate()
    {
        //If the number associated with the die side this script is attached to, automatically update what text is displayed to match
        //OnValidate so change can be seen immediately in the editor

        if (_numberText == null) _numberText = GetComponent<TextMeshPro>();
        _numberText.text = _number.ToString();
    }
}