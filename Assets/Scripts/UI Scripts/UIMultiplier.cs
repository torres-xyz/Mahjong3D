using System.Text;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class UIMultiplier : MonoBehaviour
{
    [SerializeField] private TMP_Text multiplierText;
    private StringBuilder sbMultiplier;
    private readonly string charX = "x";

    private void OnEnable()
    {
        GameManager.MultiplierChanged += OnMultiplierChanged;
        if (multiplierText == null)
            Debug.LogError("'multiplierText' was not set in the inspector");
        sbMultiplier = new StringBuilder();
    }
    private void OnDisable()
    {
        GameManager.ScoreChanged -= OnMultiplierChanged;
    }

    private void OnMultiplierChanged(object sender, int mult)
    {
        sbMultiplier.Clear();
        sbMultiplier.Append(charX).Append(mult);
        multiplierText.text = sbMultiplier.ToString();
    }
}
