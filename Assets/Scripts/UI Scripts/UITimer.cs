using System;
using System.Text;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class UITimer : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;
    private StringBuilder sbTime;

    private void OnEnable()
    {
        GameManager.TimeLeftChanged += OnTimeLeftChanged;
        if (timeText == null)
            Debug.LogError("'timeText' was not set in the inspector");
        sbTime = new StringBuilder();
    }
    private void OnDisable()
    {
        GameManager.ScoreChanged -= OnTimeLeftChanged;
    }

    private void OnTimeLeftChanged(object sender, int timeLeft)
    {
        sbTime.Clear();
        //This won't display negative numbers, but we'll never have to display those anyway.
        sbTime.Append(TimeSpan.FromSeconds(timeLeft).ToString(@"m\:ss"));
        timeText.text = sbTime.ToString();
    }
}
