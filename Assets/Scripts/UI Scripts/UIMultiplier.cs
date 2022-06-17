using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class UIMultiplier : MonoBehaviour
{
    private TMP_Text multiplierText;
    private readonly string charX = "x";
    private StringBuilder sbMultiplier;
    
    //Animation Related
    IEnumerator growShrinkRoutine;
    private RectTransform rectTransform;
    private Vector3 rectTransformStartScale;
    [SerializeField] private AnimationCurve animationCurve;
    private readonly int magicNumberMaxMultiplier = 20;
    private int currentMultiplier = 1;

    private void OnEnable()
    {
        if (multiplierText == null)
            multiplierText = GetComponent<TMP_Text>();
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        growShrinkRoutine = GrowShrink();
        rectTransformStartScale = rectTransform.localScale;
        GameManager.MultiplierChanged += OnMultiplierChanged;
        sbMultiplier = new StringBuilder();
    }
    private void OnDisable()
    {
        GameManager.ScoreChanged -= OnMultiplierChanged;
    }

    private void OnMultiplierChanged(object sender, int mult)
    {
        currentMultiplier = mult;
        sbMultiplier.Clear();
        sbMultiplier.Append(charX).Append(currentMultiplier);
        multiplierText.text = sbMultiplier.ToString();

        StopCoroutine(growShrinkRoutine);
        growShrinkRoutine = GrowShrink();
        StartCoroutine(growShrinkRoutine);
    }

    IEnumerator GrowShrink()
    {
        float t = 0.0f;
        Vector3 startSize = rectTransformStartScale;
        Vector3 endSize = startSize + Vector3.Lerp(Vector3.zero, Vector3.one, currentMultiplier/(float)magicNumberMaxMultiplier);

        float duration = 0.25f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = animationCurve.Evaluate(t / duration);

            transform.localScale = Vector3.LerpUnclamped(startSize, endSize, progress);

            yield return null;
        }
    }

}
