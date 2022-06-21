//https://johnleonardfrench.com/the-right-way-to-make-a-volume-slider-in-unity-using-logarithmic-conversion/
using System;
using UnityEngine;

public class UIVolumeSlider : MonoBehaviour
{
    public static EventHandler<float> VolumeChanged;
    public void OnValueChanged(float val) => VolumeChanged?.Invoke(this, Mathf.Log10(val) * 20);
}