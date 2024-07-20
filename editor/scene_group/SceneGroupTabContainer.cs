#if TOOLS
using Godot;

namespace SceneManagerMono.Editor.Scene_Group;

[Tool]
public partial class SceneGroupTabContainer : TabContainer {
    [Export] private PackedScene sceneGroupPagePrefab;

    public override void _EnterTree() {
        base._EnterTree();

        sceneGroupPagePrefab ??= ResourceLoader.Load<PackedScene>(
            "res://addons/scene_manager_mono/editor/scene_group/scene_group_page.tscn");
    }

    public override void _Ready() {
        base._Ready();

        if (SceneManagerEditor.Instance is {}) {
            SceneManagerEditor.Instance.Changed += UpdateTabs;

            CreateTabs();
        }
    }

    public override void _ExitTree() {
        base._ExitTree();
        
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.Changed -= UpdateTabs;    
        }
    }

    ///// Private Functions /////
    
    private void UpdateTabs() {
        if (SceneManagerEditor.Instance.SceneDataGroupList.Count != GetChildren().Count -1) {
            CreateTabs();    
        }
        // else {
        //     var groupDataList = SceneManagerEditor.Instance.SceneDataGroupList;
        //     foreach (var child in GetChildren()) {
        //         if (child is SceneGroupPage groupPage) {
        //             var groupIndex = groupPage.Group.index;
        //             var groupData = groupDataList[groupIndex];
        //
        //             groupPage.Group = groupData;
        //         }
        //     }
        // }
    }

    private void CreateTabs() {
        Clear();
        foreach (var group in SceneManagerEditor.Instance.SceneDataGroupList) {
            var groupPage = CreateSceneGroupPage();
            groupPage.Group = group;
            AddChild(groupPage);
        }
    }

    private void Clear() {
        foreach (var child in GetChildren()) {
            if (child.Name != "All") {
                RemoveChild(child);
                child.QueueFree();
            }
        }
    }

    private SceneGroupPage CreateSceneGroupPage() {
        return sceneGroupPagePrefab.Instantiate<SceneGroupPage>();
    }
}
#endif 
