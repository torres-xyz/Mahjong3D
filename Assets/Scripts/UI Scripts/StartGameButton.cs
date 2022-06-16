using System;
using UnityEngine;

public class StartGameButton : MonoBehaviour
{
    public static EventHandler LoadMainScene;
    public void StartGame() => LoadMainScene?.Invoke(this, EventArgs.Empty);
}
