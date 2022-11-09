using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private TextMeshProUGUI _totalText;
    [SerializeField] private string _resultTextDuringRoll;
    [SerializeField] private string _resultTextPrefix;
    [SerializeField] private string _totalTextPrefix;

    private int _totalCount;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;

        _resultText.text = _resultTextPrefix;
        _totalText.text = _totalTextPrefix;
    }

    public void ReportResult(int result)
    {
        //Sets the result text to latest result, updates the total text with a sum of all results

        _resultText.text = _resultTextPrefix + result.ToString();

        _totalCount += result;
        _totalText.text = _totalTextPrefix + _totalCount.ToString();
    }

    public void ReportNewRoll()
    {
        //Simply changes the text during rolls

        _resultText.text = _resultTextPrefix + _resultTextDuringRoll;
    }
}