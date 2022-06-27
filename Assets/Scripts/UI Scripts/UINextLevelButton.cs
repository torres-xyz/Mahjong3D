using System;
using UnityEngine;

public class UINextLevelButton : MonoBehaviour
{
    public static EventHandler NextLevelButtonPressed;
    public void NextLevel() => NextLevelButtonPressed?.Invoke(this, EventArgs.Empty);
}
