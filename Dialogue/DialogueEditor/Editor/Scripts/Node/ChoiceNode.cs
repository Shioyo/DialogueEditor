using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ChoiceNode : BaseNode
{

    public ChoiceNode(NodeData nodeData, DialogueView dialogueView) : base(nodeData, dialogueView)
    {
        NodeData = nodeData;
        Button button = new Button(() =>
        {
            CreatChoicePort(AddNewChoice());
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

            AddNewChoice();
            AddNewChoice();
        }

        for (int i = 0; i < NodeData.Ports.Count; i++)
        {
            if (NodeData.Ports[i].Direction==Direction.Input)
            {
                AddPort(NodeData.Ports[i].Name, NodeData.Ports[i].Guid,
                    NodeData.Ports[i].Direction,
                    NodeData.Ports[i].Capacity);
            }
            if (NodeData.Ports[i].Direction == Direction.Output)
            {
                CreatChoicePort(NodeData.Ports[i].Guid);
            }
        }


        SerializedObject serializedObject = new SerializedObject(NodeData as ChoiceNodeData);

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

        #region Text_Field
        Label text_Label = new Label("Dialogue");
        text_Label.AddToClassList("text-label");
        mainContainer.Add(text_Label);
        TextField text_Field = new TextField();
        text_Field.AddToClassList("text-field");
        text_Field.BindProperty(serializedObject.FindProperty("Dialogue"));
        mainContainer.Add(text_Field);
        #endregion

        outputContainer.style.flexDirection = FlexDirection.Column;
        this.Q("title").AddToClassList("choice-node");
    }

    public string AddNewChoice()
    {
        string guid=Guid.NewGuid().ToString();
        NodeData.Ports.Add(new PortData()
        {
            Name = "Out",
            Direction = Direction.Output,
            Capacity = Port.Capacity.Single,
            Guid = guid
        });
        (NodeData as ChoiceNodeData).Choice.Add(new ChoiceData() {PortGuid=guid
            ,ChoiceName= "Choice " + (NodeData as ChoiceNodeData).Choice.Count }  );
        return guid;
    }
    public void DeleteChoice(Port port)
    {
        var portData = NodeData.Ports.Find(node => node.Guid == port.viewDataKey);
        port.connections.ToList().ForEach(connection => { port.Disconnect(connection); DialogueView.RemoveElement(connection); });
        DialogueView.dialogueAssetSo.DeleteEdge(port, DialogueView.GetPortByGuid(portData.ConnetToGuid));

        (NodeData as ChoiceNodeData).Choice.Remove(
            (NodeData as ChoiceNodeData).Choice.Find(choice => choice.PortGuid == port.viewDataKey));
        NodeData.Ports.Remove(NodeData.Ports.Find(node => node.Guid == port.viewDataKey));
        outputContainer.Remove(port);
    }

    public void CreatChoicePort(string guid)
    {
        Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Port));
        port.portName = "Out";
        port.viewDataKey = guid;
        Ports.Add(port);

        var choiceData = (NodeData as ChoiceNodeData).Choice.Find(node => node.PortGuid == guid);
        TextField textField = new TextField();
        textField.value = choiceData.ChoiceName;
        textField.RegisterValueChangedCallback((newvalue) =>
        {
            choiceData.ChoiceName = newvalue.newValue;
            textField.SetValueWithoutNotify(choiceData.ChoiceName);
        });

        Button button = new Button(() => {
            DeleteChoice(port);
        });
        button.text = " X ";
        

        port.contentContainer.Add(textField);
        port.contentContainer.Add(button);

        outputContainer.Add(port);

        RefreshPorts();
        RefreshExpandedState();
    }
}
