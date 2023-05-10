using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 
using System.Linq;
using System;
using UnityEditor.Experimental.GraphView;

[CreateAssetMenu(fileName = "NewDialogueTree",menuName ="Dialogue/DialogueAsset")]
public class DialogueAsset : ScriptableObject
{
    public List<NodeData> Nodes=new List<NodeData>();
    public List<NodeLink> Edges =new List<NodeLink>();
    
    public SharedProperty SharedProperty;
    public NodeData CreatNodeData(System.Type type,Vector2 position)
    {
        NodeData node= ScriptableObject.CreateInstance(type) as NodeData;
        node.name = type.Name;
        node.TypeName = type.Name;
        node.NodeName = "UnNamed";
        node.Position=position;
        node.Guid = GUID.Generate().ToString();
        Nodes.Add(node);

        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(node, this);
        }
        AssetDatabase.SaveAssets();
        return node;
    }

    public void CreatEdge(Port from,Port to)
    {
        GetPortByGuid(from.viewDataKey).ConnetToGuid=to.viewDataKey;
        
        Edges.Add(new NodeLink() { BaseGuid=from.viewDataKey,TargetGuid=to.viewDataKey }) ;
    }

    public void DeleteNode(NodeData node)
    {
        Nodes.Remove(node);

        if (!Application.isPlaying)
        {
            AssetDatabase.RemoveObjectFromAsset(node);
        }
        AssetDatabase.SaveAssets();
    }
    public void DeleteEdge(Port from,Port to)
    {
        GetPortByGuid(from.viewDataKey).ConnetToGuid = default(string);
        Edges.Remove(Edges.Find(edge => edge.BaseGuid == from.viewDataKey && edge.TargetGuid == to.viewDataKey));
    }
    public void DeleteEdge(NodeLink edge)
    {
        GetPortByGuid(edge.BaseGuid).ConnetToGuid = default(string);
        Edges.Remove(edge);
    }

    public NodeData GetNodeByGuid(string guid)
    {
        return Nodes.FirstOrDefault(node=>node.Guid==guid);    
    }
    public NodeData GetNodeByPortGuid(string portGuid)
    {
        return Nodes.FirstOrDefault(node=>node.Ports.Exists(port=>port.Guid==portGuid));
    }
    public PortData GetPortByGuid(string guid)
    {
        PortData portData = null;
        Nodes.ForEach(node =>
        {
            node.Ports.ForEach((port) =>
            {
                if (port.Guid == guid)
                {
                    portData = port;
                }
            });
        });
        return portData;
    }

    public DialogueAsset Clone()
    {
        DialogueAsset clone = Instantiate(this);
        clone.Nodes = new List<NodeData>();
        clone.Edges = new List<NodeLink>();
        foreach (var node in Nodes)
        {
            clone.Nodes.Add(Instantiate(node));
        }
        foreach(var edge in Edges)
        {
            clone.Edges.Add(new NodeLink() { BaseGuid = edge.BaseGuid, TargetGuid = edge.TargetGuid });
        }
        //clone.SharedProperty = this.SharedProperty;
        return clone;
    }
}
