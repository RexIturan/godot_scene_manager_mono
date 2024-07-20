#if TOOLS
using Godot;
using SceneManagerMono.Editor.Data;
using SceneManagerMono.Editor.Util;

namespace SceneManagerMono.Editor.Scene_List;

[Tool]
public partial class GroupedSceneList : Container {
    public override void _Ready() {
        if (SceneManagerEditor.Instance is {}) {
            SceneManagerEditor.Instance.Changed += UpdateSceneDataList;

            UpdateSceneDataList();
        }
    }

    public override void _ExitTree() {
        base._ExitTree();
        
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.Changed -= UpdateSceneDataList;    
        }
    }
    
    ///// Private Functions /////

    private void Clear() {
        foreach (var child in GetChildren()) {
            RemoveChild(child);
            child.QueueFree();
        }
    }
    
    private void UpdateSceneDataList() {
        if (SceneManagerEditor.Exists()) {
            var groupContainerList = this.GetNodes<GroupedSceneContainer>();
            var groups = SceneManagerEditor.Instance.SceneDataGroupList;
            if (groupContainerList.Count -1 != groups.Count) {
                // recreate
                Clear();
                
                var allGroup = GroupedSceneContainer.CreateInstance(new SceneDataGroupItem() {
                    index = -1,
                    name = "Ungrouped"
                });
                AddChild(allGroup);
                
                foreach (var group in groups) {
                    var groupContainer = GroupedSceneContainer.CreateInstance(group);
                    AddChild(groupContainer);
                }
            }
            else {
                //update
                foreach (var groupContainer in groupContainerList) {
                    groupContainer.UpdateSceneItems();
                }
            }
        }
    }
}
#endif
