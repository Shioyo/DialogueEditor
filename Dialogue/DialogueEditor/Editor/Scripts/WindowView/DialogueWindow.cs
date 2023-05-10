using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using UnityEditor.Callbacks;

public class DialogueWindow : EditorWindow
{
    DialogueView dialogueTreeView;
    InspectorView inspectorView;
    IMGUIContainer SharedPropertyIMGUI;

    [MenuItem("DialogueTreeEditor/Editor ...")]
    public static void OpenWindow()
    {
        DialogueWindow wnd = GetWindow<DialogueWindow>();
        wnd.titleContent = new GUIContent("DialogueTreeEditor");
    }
    [OnOpenAsset]
    public static bool OnOpenAssset(int instanceid, int line)
    {
        if (Selection.activeObject is DialogueAsset)
        {
            OpenWindow();
            return true;
        }
        else
        {
            return false;
        }
    }
    VisualElement root;
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Dialogue/DialogueEditor/Editor/UssUxml/DialogueWindow.uxml");
        visualTree.CloneTree(root);

        

        dialogueTreeView = root.Q<DialogueView>();
        inspectorView = root.Q<InspectorView>();
        SharedPropertyIMGUI = root.Q<IMGUIContainer>("property");

        dialogueTreeView.OnNodeSelected = OnNodeSelectionChanged;

        OnSelectionChange();    
    }
    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChange;
        EditorApplication.playModeStateChanged += OnPlayModeChange;
    }
    private void OnPlayModeChange(PlayModeStateChange mode)
    {
        switch (mode)
        {

            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:

                break;
            case PlayModeStateChange.EnteredPlayMode:

                break;
            case PlayModeStateChange.ExitingPlayMode:
                OnSelectionChange();
                break;
            default:
                break;
        }
    }

    private void OnNodeSelectionChanged(BaseNode node)
    {
        inspectorView.OnSelectionChanged(node);
    }

    DialogueAsset dialogueAsset;
    Editor editor;
    private void OnSelectionChange()
    {
        if (Selection.activeObject is DialogueAsset asset)
        {
            dialogueAsset = asset;

            if (AssetDatabase.CanOpenAssetInEditor(dialogueAsset.GetInstanceID()))
            {
                if(dialogueTreeView!=null)  dialogueTreeView.editorWindow = this;
                if (dialogueTreeView != null) dialogueTreeView.Populate(dialogueAsset);
                if (dialogueAsset.SharedProperty != null)
                {
                    UnityEngine.Object.DestroyImmediate(editor);
                    editor = Editor.CreateEditor(dialogueAsset.SharedProperty);

                    if (SharedPropertyIMGUI != null&&dialogueAsset.SharedProperty != null)
                    {
                        UnityEngine.Object.DestroyImmediate(editor);
                        editor = Editor.CreateEditor(dialogueAsset.SharedProperty);
                        SharedPropertyIMGUI.onGUIHandler = () =>
                        {
                            if (editor.target != null)
                            {
                                editor.OnInspectorGUI();
                            }
                        };

                    }
                }
            }
        }else
        {
            if (Selection.activeGameObject)
            {
                var manager = Selection.activeGameObject.GetComponent<DialogueManager>();
                if (manager!=null && manager.DialogueAsset!=null)
                {
                    dialogueAsset = manager.DialogueAsset;
                    if(dialogueTreeView!=null)  dialogueTreeView.editorWindow = this;
                    if(dialogueTreeView!=null)  dialogueTreeView.Populate(dialogueAsset);
                    if (SharedPropertyIMGUI!=null&& dialogueAsset.SharedProperty != null)
                    {
                        UnityEngine.Object.DestroyImmediate(editor);
                        editor = Editor.CreateEditor(dialogueAsset.SharedProperty);
                        SharedPropertyIMGUI.onGUIHandler = () =>
                        {
                            if (editor.target!=null)
                            {
                                editor.OnInspectorGUI();
                            } 
                        };

                    }
                }
            }
        }

    }
    private void OnInspectorUpdate()
    {
        for (int i = 0; i < dialogueTreeView.NodeViews.Count; i++)
        {
            dialogueTreeView.NodeViews[i].Update();
        }
    }
}