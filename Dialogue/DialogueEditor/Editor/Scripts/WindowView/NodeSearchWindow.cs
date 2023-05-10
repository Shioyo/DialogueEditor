using log4net.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    public DialogueView graphView;
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("CreatNode"),level:0),

            new SearchTreeGroupEntry(new GUIContent("StartEnd"),level:1),
            AddNodeSearch("End",typeof(EndNodeData), 2),

            new SearchTreeGroupEntry(new GUIContent("Dialogue"),level:1),
            AddNodeSearch("Dialogue",typeof(DialogueNodeData), 2),
            AddNodeSearch("Choice",typeof(ChoiceNodeData), 2),
            AddNodeSearch("Switch",typeof(SwitchNodeData), 2),

            new SearchTreeGroupEntry(new GUIContent("Action"),level:1),
            AddNodeSearch("Jump",typeof(JumpNodeData), 2),
            AddNodeSearch("Action",typeof(ActionNodeData),2)
            

        };

        return tree;
    }
    public SearchTreeEntry AddNodeSearch(string name, Type type, int level)
    {
        SearchTreeEntry searchTreeEntry = new SearchTreeEntry(new GUIContent(name));
        searchTreeEntry.userData = type;
        searchTreeEntry.level = level;
        return searchTreeEntry;
    }
    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        Vector2 mousePosition = graphView.editorWindow.rootVisualElement.ChangeCoordinatesTo
        (
            graphView.editorWindow.rootVisualElement.parent, context.screenMousePosition - graphView.editorWindow.position.position
        );
        Vector2 graphMousePosition = graphView.contentViewContainer.WorldToLocal(mousePosition);
        
        var nodeData=graphView.CreatNode((Type)SearchTreeEntry.userData, graphMousePosition);

        graphView.CreatNodeView(nodeData);

        return true;
    }
}
