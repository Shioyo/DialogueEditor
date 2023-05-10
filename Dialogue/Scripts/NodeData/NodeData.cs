 using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class NodeData : ScriptableObject
{
    [HideInInspector] public string TypeName;
    public string NodeName;

    [HideInInspector]public string Guid;

    [HideInInspector]public Vector2 Position;
    [HideInInspector]public List<PortData> Ports=new List<PortData>();
    public NodeState State;
}

[System.Serializable]
public class NodeLink
{
    public string BaseGuid;
    public string TargetGuid;
}

[System.Serializable]
public class PortData
{
    public string Name;
    public string Guid;
    public string ConnetToGuid;
    public Direction Direction;
    public Port.Capacity Capacity;
}

public enum NodeState
{
    SLEEP,
    RUNNING,
    FINSHIED,
}