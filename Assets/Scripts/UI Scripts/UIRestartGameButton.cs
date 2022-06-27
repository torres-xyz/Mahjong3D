using System;
using UnityEngine;

public class UIRestartGameButton : MonoBehaviour
{
    public static EventHandler RestartGameButtonPressed;
    public void RestartGame() => RestartGameButtonPressed?.Invoke(this, EventArgs.Empty);
}
