using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceNodeData : NodeData
{
    public List<ChoiceData> Choice = new List<ChoiceData>();
    public string Name;
    public string Dialogue;
}
[System.Serializable]
public class ChoiceData
{
    public string ChoiceName;
    public string PortGuid;
}
