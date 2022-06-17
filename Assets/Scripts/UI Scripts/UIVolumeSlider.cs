using System;
using UnityEngine;

public class UIVolumeSlider : MonoBehaviour
{
    public static EventHandler<float> VolumeChanged;
    public void OnValueChanged(float val) => VolumeChanged?.Invoke(this, Mathf.Log10(val) * 20);
}