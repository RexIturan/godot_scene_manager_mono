#if TOOLS
using Godot;
using SceneManagerMono.Editor.Data;
using SceneManagerMono.Editor.Scene_Group;
using SceneManagerMono.Editor.Util;

namespace SceneManagerMono.Editor.Scene_List;

[Tool]
public partial class GroupedSceneContainer : Container {
    private Button mainButton;
    
    public SceneDataGroupItem Group { get; set; }

    public SceneGroupPage sceneGroupPage;

    ///// Godot Functions /////

    public override void _EnterTree() {
        base._EnterTree();

        mainButton ??= GetNode<Button>("%Content Container");
        sceneGroupPage ??= GetNode<SceneGroupPage>("%Scene Group Page");
        sceneGroupPage.Group = Group;
    }

    public override void _Ready() {
        base._Ready();
        
        Name = Group.name;
        mainButton.Text = Group.name;
    }

    ///// Public Functions /////

    public void UpdateSceneItems() {
        sceneGroupPage.UpdateSceneItems();
    }

    ///// Public Static Functions /////
    
    public static GroupedSceneContainer CreateInstance(SceneDataGroupItem group) {
        var groupContainer = GodotHelper.LoadAndInstaniate<GroupedSceneContainer>(
            "res://addons/scene_manager_mono/editor/scene_list/grouped_scene_container.tscn"
        );
        groupContainer.Group = group;
        return groupContainer;
    }
}
#endif
