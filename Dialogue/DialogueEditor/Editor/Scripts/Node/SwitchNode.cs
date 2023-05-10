using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class SwitchNode : BaseNode
{
    TextField text_Field;
    public SwitchNode(NodeData nodeData, DialogueView dialogueView) : base(nodeData, dialogueView)
    {
        NodeData = nodeData;
        Button button = new Button(() =>
        {
            CreatBrachPort(CreatBranch());
        });
        button.name = "title-button";
        button.text = " + Add New Choice";
        titleButtonContainer.Add(button);


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

            CreatBranch();
            CreatBranch();
        }

        for (int i = 0; i < NodeData.Ports.Count; i++)
        {
            if (NodeData.Ports[i].Direction == Direction.Input)
            {
                AddPort(NodeData.Ports[i].Name, NodeData.Ports[i].Guid,
                    NodeData.Ports[i].Direction,
                    NodeData.Ports[i].Capacity);
            }
            if (NodeData.Ports[i].Direction == Direction.Output)
            {
                CreatBrachPort(NodeData.Ports[i].Guid);
            }
        }

        SerializedObject serializedObject = new SerializedObject(NodeData as SwitchNodeData);

        #region EventAsset_Field
        Label eventAsset_Label = new Label("EventAsset");
        eventAsset_Label.AddToClassList("text-label");
        mainContainer.Add(eventAsset_Label);

        ObjectField eventAsset_Field = new ObjectField()
        {
            objectType = typeof(DialogueEventSo),
            allowSceneObjects = false,
        };
        eventAsset_Field.BindProperty(serializedObject.FindProperty("DialogueEventAsset"));
        mainContainer.Add(eventAsset_Field);

        #endregion

        #region Text_Field
        Label text_Label = new Label("FuctionName");
        text_Label.AddToClassList("text-label");
        mainContainer.Add(text_Label);
        text_Field = new TextField();
        text_Field.AddToClassList("text-field");
        text_Field.BindProperty(serializedObject.FindProperty("FunctionName"));
        mainContainer.Add(text_Field);
        #endregion

        outputContainer.style.flexDirection = FlexDirection.Column;
        this.Q("title").AddToClassList("switch-node");
    }



    public string CreatBranch()
    {
        string guid=Guid.NewGuid().ToString();
        NodeData.Ports.Add(new PortData()
        {
            Name = "Out",
            Direction = Direction.Output,
            Capacity = Port.Capacity.Single,
            Guid = guid
        });
        return guid;
    }

    public void DeleteBranch(Port port)
    {
        var portData = NodeData.Ports.Find(node => node.Guid == port.viewDataKey);
        port.connections.ToList().ForEach(connection => { port.Disconnect(connection); DialogueView.RemoveElement(connection); });
        DialogueView.dialogueAssetSo.DeleteEdge(port, DialogueView.GetPortByGuid(portData.ConnetToGuid));
        
        NodeData.Ports.Remove(portData);
        outputContainer.Remove(port);
    }

    public void CreatBrachPort(string guid)
    {
        Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Port));
        port.portName = "Out";
        port.viewDataKey = guid;
        Ports.Add(port);

        Button button = new Button(() => {
            DeleteBranch(port);
        });
        button.text = " X ";

        port.contentContainer.Add(button);
        outputContainer.Add(port);

        RefreshPorts();
        RefreshExpandedState();
    }
}
