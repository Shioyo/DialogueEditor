using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Config")]
public class DialogueConfig : ScriptableObject
{
    [Range(1, 20)]
    public float PrintSpeed=6;
    [Range(0, 1)]
    public float BgVolume=0.8f;
    [Range(0, 1)]
    public float EffectVolume=0.8f;

    [Range(0.1f, 2f)]
    public float DialogueInterval=0.5f;

}