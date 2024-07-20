#if TOOLS
using System.Collections.Generic;
using System.Linq;
using Godot;
using SceneManagerMono.Editor.Data;
using SceneManagerMono.Editor.Scene_List;

namespace SceneManagerMono.Editor.Scene_Key_Confilct;

[Tool]
public partial class SceneKeyConflictsDialog : AcceptDialog {
    private SceneListDisplay sceneListDisplay;
    private HashSet<int> duplicateIdices = new HashSet<int>();

    public override void _EnterTree() {
        base._EnterTree();

        sceneListDisplay ??= GetNode<SceneListDisplay>("%SceneList");
    }

    public override void _Ready() {
        base._Ready();

        SceneManagerEditor.Instance.Changed += UpdateSceneItems;
        
        SceneManagerEditor.Instance.ValidateSceneDataKeys();
        CreateDuplicateSceneKeyList();
    }

    public override void _ExitTree() {
        base._ExitTree();
        
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.Changed -= UpdateSceneItems;    
        }
    }

    ///// Private Functions /////
    
    private void UpdateSceneItems() {
        List<SceneDataItem> duplicateSceneKeyData = SceneManagerEditor.Instance.GetSceneDataWithDuplicateKeys();
        foreach (var item in duplicateSceneKeyData) {
            duplicateIdices.Add(item.index);
        }
        
        var duplicatedItems = 
            SceneManagerEditor.Instance.SceneDataItems.Where(item => duplicateIdices.Contains(item.index)).ToList();
        sceneListDisplay.UpdateSceneItems(duplicatedItems);
    }
    
    private void CreateDuplicateSceneKeyList() {
        //get all konflicting keys
        List<SceneDataItem> duplicateSceneKeyData = SceneManagerEditor.Instance.GetSceneDataWithDuplicateKeys();
        duplicateIdices = duplicateSceneKeyData.Select(item => item.index).ToHashSet();
        
        sceneListDisplay.Clear();
        
        if (duplicateSceneKeyData.Count == 0) {
            //todo
            // show no duplicate keys?
        }
        else {
            sceneListDisplay.AddAll(duplicateSceneKeyData);
        }
    }
}
#endif
