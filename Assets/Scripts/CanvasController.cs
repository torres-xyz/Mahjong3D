using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasController : MonoBehaviour
{
    private Canvas canvas;

    private void OnEnable()
    {
        canvas = GetComponent<Canvas>();
        GameManager.SwitchCanvas += OnSwitchCanvas;
    }
    private void OnDisable()
    {
        GameManager.SwitchCanvas -= OnSwitchCanvas;
    }

    private void OnSwitchCanvas(object sender, string canvasName) => canvas.enabled = canvasName == gameObject.name;
}
