using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueEventAsset",menuName = "Dialogue/Event/EventAsset")]
public class DialogueEventSo : ScriptableObject
{
    public int Swicth_01(DialogueAsset dialogueAsset)
    {
        return 0;
    }
    public void AddSampleInt(DialogueAsset dialogueAsset)
    {
        dialogueAsset.SharedProperty.SampleInt++;
    }
}
