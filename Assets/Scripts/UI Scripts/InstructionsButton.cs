using System;
using UnityEngine;

public class InstructionsButton : MonoBehaviour
{
    public static EventHandler InstructionsButtonWasPressed;
    public void Pressed() => InstructionsButtonWasPressed?.Invoke(this, EventArgs.Empty);
}
