using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class EndNode : BaseNode
{
    public EndNode(NodeData nodeData, DialogueView dialogueView) : base(nodeData, dialogueView)
    {
        if (NodeData.Ports.Count == 0)
        {
            string inputGuid = Guid.NewGuid().ToString();
            NodeData.Ports.Add(new PortData()
            {
                Name = "In",
                Direction = Direction.Input,
                Capacity = Port.Capacity.Multi,
                Guid = inputGuid
            });
        }


        for (int i = 0; i < NodeData.Ports.Count; i++)
        {
            AddPort(NodeData.Ports[i].Name, NodeData.Ports[i].Guid,
                NodeData.Ports[i].Direction,
                NodeData.Ports[i].Capacity);
        }

        this.Q("title").AddToClassList("end-node");
    }
}
