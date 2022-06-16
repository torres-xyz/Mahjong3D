using System;
using UnityEngine;

public class ResumeButton : MonoBehaviour
{
    public static EventHandler ResumeButtonWasPressed;
    public void Pressed() => ResumeButtonWasPressed?.Invoke(this, EventArgs.Empty);
}
