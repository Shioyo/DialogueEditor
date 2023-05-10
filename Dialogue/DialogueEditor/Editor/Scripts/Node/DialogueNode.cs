using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueNode : BaseNode
{
    public DialogueNode(NodeData nodeData, DialogueView dialogueView) : base(nodeData, dialogueView)
    {
        if (NodeData.Ports.Count==0)
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


        SerializedObject serializedObject = new SerializedObject(NodeData as DialogueNodeData);
        #region Audio_Clip
        Label audio_Label = new Label("Clip");
        audio_Label.AddToClassList("text-label");
        mainContainer.Add(audio_Label);
        
        ObjectField audioClip_Field = new ObjectField()
        {
            objectType = typeof(AudioClip),
            allowSceneObjects = false,
        };
        audioClip_Field.BindProperty(serializedObject.FindProperty("Clip"));
        mainContainer.Add(audioClip_Field);

        #endregion

        #region Name
        Label name_Label = new Label("Name");
        name_Label.AddToClassList("text-label");
        mainContainer.Add(name_Label);
        TextField name_Field = new TextField();
        name_Field.multiline = true;
        name_Field.AddToClassList("text-field");
        name_Field.BindProperty(serializedObject.FindProperty("Name"));
        mainContainer.Add(name_Field);
        #endregion

        #region Dialogue
        Label text_Label = new Label("Dialogue");
        text_Label.AddToClassList("text-label");
        mainContainer.Add(text_Label);
        TextField text_Field = new TextField();
        text_Field.multiline = true;
        text_Field.AddToClassList("text-field");
        text_Field.BindProperty(serializedObject.FindProperty("Dialogue"));
        mainContainer.Add(text_Field);
        #endregion

        this.Q("title").AddToClassList("dialogue-node");
    }
}
