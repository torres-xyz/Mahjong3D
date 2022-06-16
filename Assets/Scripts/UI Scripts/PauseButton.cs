using System;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    public static EventHandler PauseButtonWasPressed;
    public void Pressed() => PauseButtonWasPressed?.Invoke(this, EventArgs.Empty);
}
