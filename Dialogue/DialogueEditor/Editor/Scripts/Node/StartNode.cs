using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class StartNode : BaseNode
{
    public StartNode(NodeData nodeData, DialogueView dialogueView) : base(nodeData,dialogueView)
    {
        if (NodeData.Ports.Count == 0)
        {
            string outputGuid = Guid.NewGuid().ToString();
            NodeData.Ports.Add(new PortData()
            {
                Name = "Out",
                Direction = Direction.Output,
                Capacity = Port.Capacity.Single,
                Guid = outputGuid
            });
        }

        for (int i = 0; i < NodeData.Ports.Count; i++)
        {
            AddPort(NodeData.Ports[i].Name, NodeData.Ports[i].Guid,
                NodeData.Ports[i].Direction,
                NodeData.Ports[i].Capacity);
        }

        this.Q("title").AddToClassList("start-node");

        RefreshExpandedState();
        RefreshPorts();
    }
}
