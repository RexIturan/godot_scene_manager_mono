#if TOOLS
using Godot;

namespace SceneManagerMono.Editor.Scene_Group;

[Tool]
public partial class SceneGroupList : Container {
    [Export] private PackedScene sceneGroupPrefab;

    public override void _Ready() {
        base._Ready();

        if (SceneManagerEditor.Instance is {}) {
            SceneManagerEditor.Instance.Changed += CreateGroupItemList;
            
            CreateGroupItemList();
        }
    }

    public override void _ExitTree() {
        base._ExitTree();

        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.Changed -= CreateGroupItemList;
        }
    }

    private void Clear() {
        foreach (var sceneItem in GetChildren()) {
            RemoveChild(sceneItem);
            sceneItem.QueueFree();
        }
    }
    
    private void CreateGroupItemList() {
        Clear();
        var groups = SceneManagerEditor.Instance.SceneDataGroupList;
        
        foreach (var group in groups) {
            var groupItem = CreateSceneGroupItem();
            groupItem.Data = group;
            AddChild(groupItem);
            groupItem.Text = group.name;
        }
    }

    private SceneGroupItem CreateSceneGroupItem() {
        return sceneGroupPrefab.Instantiate<SceneGroupItem>();
    }
}
#endif
