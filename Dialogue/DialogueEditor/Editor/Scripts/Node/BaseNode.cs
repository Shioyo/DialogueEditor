using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class BaseNode : Node
{
    public NodeData NodeData;

    public DialogueView DialogueView;
    public Action<BaseNode> OnNodeSelected;
    public Vector2 size=new Vector2(200,250);

    public List<Port> Ports=new List<Port>();

    public BaseNode(NodeData nodeData, DialogueView dialogueView) : base("Assets/Dialogue/DialogueEditor/Editor/UssUxml/NodeView.uxml")
    {
        this.NodeData = nodeData;
        title = GetType().Name;
        SetPosition(new Rect(nodeData.Position, size));
        viewDataKey = nodeData.Guid;
        DialogueView = dialogueView;


        outputContainer.style.flexDirection = FlexDirection.RowReverse;
    }

    public void AddPort(string name,string guid,Direction direction,Port.Capacity capality=Port.Capacity.Single)
    {
        Port port=InstantiatePort(Orientation.Horizontal,direction, capality,typeof(Port));
        port.viewDataKey = guid;
        port.portName = name;
        //port.title= name;
        if (direction==Direction.Input)
        {
            inputContainer.Add(port);
            Ports.Add(port);   
        }else if (direction==Direction.Output)
        {
            outputContainer.Add(port);
            Ports.Add(port);
        }
    }
    public override void OnSelected()
    {
        base.OnSelected();
        if (OnNodeSelected!=null)
        {
            OnNodeSelected.Invoke(this);
        }
    }

    public void Update()
    {
        if (Application.isPlaying)
        {
            if (NodeData.State == NodeState.SLEEP)
            {
                RemoveFromClassList("running");
                RemoveFromClassList("finshed");
                RemoveFromClassList("sleep");
                AddToClassList("sleep");
            }
            if (NodeData.State == NodeState.RUNNING)
            {
                RemoveFromClassList("running");
                RemoveFromClassList("finshed");
                RemoveFromClassList("sleep");
                AddToClassList("running");
            }
            if (NodeData.State == NodeState.FINSHIED)
            {
                RemoveFromClassList("running");
                RemoveFromClassList("finshed");
                RemoveFromClassList("sleep");
                AddToClassList("finshed");
            }
        }

    }
}
