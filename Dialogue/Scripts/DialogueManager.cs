using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public DialogueUIManager DialogueUIManager;
    public DialogueAsset DialogueAsset;
    public DialogueConfig DialogueConfig;
    private void Awake()
    {
        if (DialogueAsset!=null)
        {
            DialogueAsset = DialogueAsset.Clone();
        }
        if (DialogueUIManager==null)
        {
            
            Debug.LogError("UIManager is null or empty");
        }
        else
        {
            DialogueUIManager.DisableUI();
        }
    }
    public void StartDialogue()
    {
        StartNodeData startNodeData=DialogueAsset.Nodes.FirstOrDefault(node => node is StartNodeData) as StartNodeData;
        SwitchRunning(startNodeData);
    }
    public void ParseDialogue()
    {

    }
    public void ResumeDialogue()
    {

    }
    public void EndDialogue()
    {

    }
    private void SwitchRunning(NodeData nodeData)
    {
        switch (nodeData)
        {
            case StartNodeData startNodeData:
                RunningNode(startNodeData);
                break;
            case DialogueNodeData dialogueNodeData:
                RunningNode(dialogueNodeData);
                break;
            case ChoiceNodeData choiceNodeData:
                RunningNode(choiceNodeData); 
                break;
            case SwitchNodeData switchNodeData:
                RunningNode(switchNodeData); 
                break;
            case JumpNodeData jumpNodeData: 
                RunningNode(jumpNodeData);
                break;
            case EndNodeData endNodeData:
                RunningNode(endNodeData);
                break;
            case ActionNodeData actionNodeData:
                RunningNode(actionNodeData); 
                break;
            default: 
                break;
        }
    }

    #region RunningNode
    public void RunningNode(StartNodeData node)
    {
        DialogueUIManager.EnableUI();
        node.State = NodeState.FINSHIED;
        if (node.Ports.Count!=0)
        {
            SwitchRunning(DialogueAsset.GetNodeByPortGuid(node.Ports[0].ConnetToGuid));
        }
    }
    public void RunningNode(EndNodeData node)
    {
        DialogueUIManager.DisableUI();
    }
    public void RunningNode(DialogueNodeData node)
    {
        IEnumerator tmp()
        {
            yield return null;

            node.State = NodeState.RUNNING;
            int index = 0;
            float timer = 0;
            float printSpeed = DialogueConfig.PrintSpeed;
            bool printFinshed = false;
            DialogueUIManager.ContentText.text = default(string);

            while (!Input.GetMouseButtonDown(0))
            {
                yield return null;
                timer += Time.deltaTime;
                if (index == node.Dialogue.Length)
                {
                    printFinshed = true;
                    break;
                }
                else if (timer > 1f / 3 / printSpeed)
                {
                    DialogueUIManager.ContentText.text = DialogueUIManager.ContentText.text + node.Dialogue[index];
                    index++;
                    timer = 0;
                }
            }

            if (!printFinshed)//如果输出未结束
            {
                DialogueUIManager.ContentText.text = node.Dialogue;
            }

            yield return new WaitForSeconds(DialogueConfig.DialogueInterval);
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (node.Ports.Count != 0)
                    {
                        node.State = NodeState.FINSHIED;
                        SwitchRunning(DialogueAsset.GetNodeByPortGuid(node.Ports.Find(port => port.Direction == Direction.Output).ConnetToGuid));
                        break;
                    }
                }
                yield return null;

            }
            yield return null;
        }

        StartCoroutine(tmp());
    }

    public void RunningNode(ChoiceNodeData node)
    {
        IEnumerator tmp()
        {
            yield return null;

            node.State = NodeState.RUNNING;
            int index = 0;
            float timer = 0;
            float printSpeed = DialogueConfig.PrintSpeed;
            bool printFinshed = false;
            DialogueUIManager.ContentText.text = default(string);

            while (!Input.GetMouseButtonDown(0))
            {
                yield return null;
                timer += Time.deltaTime;
                if (index == node.Dialogue.Length)
                {
                    printFinshed = true;
                    break;
                }
                else if (timer > 1f / 3 / printSpeed)
                {
                    DialogueUIManager.ContentText.text = DialogueUIManager.ContentText.text + node.Dialogue[index];
                    index++;
                    timer = 0;
                }
            }

            if (!printFinshed)//如果输出未结束
            {
                DialogueUIManager.ContentText.text = node.Dialogue;
            }
            for (int i = 0; i < node.Choice.Count; i++)
            {
                string guid = node.Choice[i].PortGuid;
                MakeButton(node.Choice[i].ChoiceName, () =>
                {
                    node.State = NodeState.FINSHIED;
                    ClearAllButton();
                    SwitchRunning(DialogueAsset.GetNodeByPortGuid(node.Ports.Find(port => port.Guid == guid).ConnetToGuid));
                });
            }
            yield return null;
        }
        StartCoroutine(tmp());
    }
    public void RunningNode(SwitchNodeData node)
    {
        if (node.DialogueEventAsset==null)
        {
            throw new NullReferenceException("DialogueEventAsset is null or empty");
        }
        Type type=node.DialogueEventAsset.GetType();
        var methodInfo=type.GetMethod(node.FunctionName);
        int switchIndex= (int) methodInfo.Invoke(node.DialogueEventAsset,new object[] { DialogueAsset });
        if (node.Ports.Count != 0 && switchIndex < node.Ports.Count) 
        {
            node.State=NodeState.FINSHIED;
            //Debug.Log(DialogueAsset.GetNodeByPortGuid(node.Ports[switchIndex].ConnetToGuid));
            SwitchRunning(DialogueAsset.GetNodeByPortGuid(node.Ports[switchIndex+1].ConnetToGuid));
        }
        else
        {
            throw new ArgumentOutOfRangeException("SwitchIndex is out of range!");
        }
    }
    public void RunningNode(JumpNodeData node)
    {
        var jumpToNode=DialogueAsset.Nodes.FirstOrDefault(n => n.NodeName == node.JumpToNodeName);
        if (jumpToNode!=null)
        {
            SwitchRunning(jumpToNode);
        }
        else
        {
            Debug.LogError($"Jump To Node {node.JumpToNodeName} Failed!");
        }
    }
    public void RunningNode(ActionNodeData node)
    {
        if (node.DialogueEventAsset == null)
        {
            throw new NullReferenceException("DialogueEventAsset is null or empty");
        }
        if (node.FunctionName==null || node.FunctionName == default(string))
        {
            Debug.LogError("FunctionName is empty");
            return;
        }
        Type type = node.DialogueEventAsset.GetType();
        var methodInfo = type.GetMethod(node.FunctionName);
        methodInfo.Invoke(node.DialogueEventAsset, new object[] { DialogueAsset });
        if (node.Ports.Count != 0)
        {
            node.State = NodeState.FINSHIED;
            SwitchRunning(DialogueAsset.GetNodeByPortGuid(node.Ports.Find(port => port.Direction == Direction.Output).ConnetToGuid));
        }
    }
    public void MakeButton(string buttonText,UnityAction action)
    {
        DialogueUIManager.MakeButton(buttonText, action);
    }
    public void ClearAllButton()
    {
        DialogueUIManager.ClearAllButton();
    }
    #endregion

}