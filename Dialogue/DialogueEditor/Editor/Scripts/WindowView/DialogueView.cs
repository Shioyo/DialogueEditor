using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine.UI;

public class DialogueView : GraphView
{
    public DialogueWindow editorWindow;
    public DialogueAsset dialogueAssetSo;

    public List<BaseNode> NodeViews = new List<BaseNode>();
    private NodeSearchWindow searchWindow;
    public Action<BaseNode> OnNodeSelected;
    public new class UxmlFactory : UxmlFactory<DialogueView, GraphView.UxmlTraits> { }

    VisualElement TipView;
    Label textTip;
    public DialogueView()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());


        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Dialogue/DialogueEditor/Editor/UssUxml/DialogueWindow.uss");
        styleSheets.Add(styleSheet);


        searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        searchWindow.graphView = this;
        nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);

    }
    

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.Where(endport => endport.direction != startPort.direction && endport != startPort).ToList();
    }

    
    public void Populate(DialogueAsset dialogueTreeSo)
    {
        this.dialogueAssetSo = dialogueTreeSo;
        NodeViews = new List<BaseNode>();

        //删除所有元素
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        //添加StartNode
        if (!dialogueTreeSo.Nodes.Exists(node => node is StartNodeData))
        {
            NodeData nodeData = CreatNode(typeof(StartNodeData), Vector2.zero);

            EditorUtility.SetDirty(dialogueTreeSo);
            AssetDatabase.SaveAssets();
        }
        //创建所有NodeView
        if (dialogueTreeSo.Nodes != null)
        {
            dialogueTreeSo.Nodes.ForEach(node => CreatNodeView(node));
        }
        //创建所有Edge
        dialogueTreeSo.Edges.ForEach(link =>
        {
            Port from = GetPortByGuid(link.BaseGuid);
            Port to = GetPortByGuid(link.TargetGuid);


            Edge edge = from.ConnectTo(to);

            AddElement(edge);
        });
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach((elem) =>
            {
                //如果删除的是节点
                if (elem is BaseNode nodeView)
                {
                    for (int i = dialogueAssetSo.Edges.Count-1; i > 0; i--)
                    {
                        if (nodeView.Ports.Exists(port=>port.viewDataKey== dialogueAssetSo.Edges[i].BaseGuid ||
                        port.viewDataKey == dialogueAssetSo.Edges[i].TargetGuid) )
                        {
                            DeleteEdge(dialogueAssetSo.Edges[i]);
                        }
                    }
                    NodeViews.Remove(nodeView);
                    DeleteNode(nodeView.NodeData);
                }

                //如果删除的是连线
                if (elem is Edge edge)
                {
                    DeleteEdge(edge.output, edge.input);
                    
                }
            });
        }
        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                CreatEdge(edge.output, edge.input);
            });
        }
        if (graphViewChange.movedElements !=null)
        {
            graphViewChange.movedElements.ForEach(elem =>
            {
                if (elem is BaseNode nodeView)
                {
                    nodeView.NodeData.Position += graphViewChange.moveDelta;
                }
            });
        }
        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        base.BuildContextualMenu(evt);
    }
    //创建节点数据
    public NodeData CreatNode(System.Type type, Vector2 position)
    {
        NodeData node = dialogueAssetSo.CreatNodeData(type,position);

        return node;
    }

    public void CreatEdge(Port from,Port to)
    {
        dialogueAssetSo.CreatEdge(from, to);
    }
    public void DeleteNode(NodeData node)
    {
        dialogueAssetSo.DeleteNode(node);
    }
    public void DeleteEdge(NodeLink edge)
    {
        dialogueAssetSo.DeleteEdge(edge);
    }
    public void DeleteEdge(Port from,Port to)
    {
        dialogueAssetSo.DeleteEdge(from,to);
    }
    //创建对应的View
    public void CreatNodeView(NodeData nodeData)
    {
        string TypeName = nodeData.TypeName;
        var viewType=Type.GetType(TypeName.Substring(0, TypeName.Length - 4));

        var nodeView = Activator.CreateInstance(viewType,nodeData,this) as BaseNode;
        nodeView.OnNodeSelected = OnNodeSelected;
        NodeViews.Add(nodeView);

        this.AddElement(nodeView as BaseNode);
    }
}