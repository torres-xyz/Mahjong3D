using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSO", menuName = "ScriptableObjects/LevelSO")]
public class LevelSO : ScriptableObject
{
    [Header("3D Board Level Layout")]
    public int[,,] level;
}
