using System;
using UnityEngine;

public class RestartGameButton : MonoBehaviour
{
    public static EventHandler RestartGameButtonPressed;
    public void RestartGame() => RestartGameButtonPressed?.Invoke(this, EventArgs.Empty);
}
