#if TOOLS
using System.Collections.Generic;
using Godot;
using SceneManagerMono.Editor.Data;
using SceneManagerMono.Editor.Scene_List;

namespace SceneManagerMono.Editor.Scene_Group;

[Tool]
public partial class SceneGroupPage : Control {
    [Export] private SceneListDisplay sceneListDisplay;
    
    public SceneDataGroupItem Group { get; set; }

    ///// Godot Functions /////
    
    public override void _EnterTree() {
        base._EnterTree();

        sceneListDisplay ??= GetNode<SceneListDisplay>("%SceneList");
    }

    public override void _Ready() {
        base._Ready();
        
        Name = Group.name;
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.Changed += UpdateSceneItems;
            
            UpdateSceneItems();
        }
    }

    public override void _ExitTree() {
        base._ExitTree();
        
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.Changed -= UpdateSceneItems;    
        }
    }

    ///// Public Functions /////
    
    public void UpdateSceneItems() {
        if (Group.index >= 0 && Group.index < SceneManagerEditor.Instance.SceneDataGroupList.Count) {
            Group = SceneManagerEditor.Instance.SceneDataGroupList[Group.index];
            var items = SceneManagerEditor.Instance.SceneDataItems;

            List<SceneDataItem> groupItems = new();
            foreach (var sceneIndex in Group.scenes) {
                groupItems.Add(items[sceneIndex]);
            }
        
            sceneListDisplay.UpdateSceneItems(groupItems);    
        } else if (Group.index == -1) {
            var items = SceneManagerEditor.Instance.UnGroupedItems;
            sceneListDisplay.UpdateSceneItems(items);    
        }
    }
}
#endif
