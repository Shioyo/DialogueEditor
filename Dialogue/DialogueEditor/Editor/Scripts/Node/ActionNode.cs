using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ActionNode : BaseNode
{
    public ActionNode(NodeData nodeData, DialogueView dialogueView) : base(nodeData, dialogueView)
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

        SerializedObject serializedObject = new SerializedObject(NodeData as ActionNodeData);

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

        #region Name
        Label name_Label = new Label("ActionName");
        name_Label.AddToClassList("text-label");
        mainContainer.Add(name_Label);
        TextField name_Field = new TextField();
        name_Field.multiline = true;
        name_Field.AddToClassList("text-field");
        name_Field.BindProperty(serializedObject.FindProperty("FunctionName"));
        mainContainer.Add(name_Field);
        #endregion

        this.Q("title").AddToClassList("action-node");
    }
}
