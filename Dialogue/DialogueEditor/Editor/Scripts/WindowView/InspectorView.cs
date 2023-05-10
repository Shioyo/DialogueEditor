using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
public class InspectorView : VisualElement
{
    Editor editor;
    public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }
    public InspectorView() { 
        
    }

    internal void OnSelectionChanged(BaseNode node)
    {
        Clear();

        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(node.NodeData);
        IMGUIContainer iMGUIContainer = new IMGUIContainer(() =>
        {
            if (editor.target != null)
            {
                editor.OnInspectorGUI();
            }
        }
        );
        EditorGUIUtility.PingObject(node.NodeData);
        Add(iMGUIContainer);
        
    }
}
