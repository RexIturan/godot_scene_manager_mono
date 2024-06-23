#if TOOLS

using System.Collections.Generic;
using Godot;

//todo rename
[Tool]
public partial class SceneList : Control {
    [Export] private AddNewScene addNewScene;
    [Export] private SceneListDisplay sceneListDisplay;

    [Export] private Button deleteToggle;
    [Export] private Button refreshButton;

    [Signal]
    public delegate void RefreshedEventHandler();
    
    public override void _EnterTree() {
        sceneListDisplay ??= GetNode<SceneListDisplay>("%Scene List Display");
        addNewScene ??= GetNode<AddNewScene>("%Add New Scene");

        deleteToggle ??= GetNode<Button>("%delete toggle");
        refreshButton ??= GetNode<Button>("%refresh");
    }

    public override void _Ready() {
        deleteToggle.ButtonPressed = false;
        deleteToggle.Toggled += OnDeleteToggle;
        refreshButton.Pressed += OnRefresh;
        sceneListDisplay.Clear();
    }

    // ##### Public Functions #####
    
    public void AddAll(List<SceneDataItem> sceneList) {
        sceneListDisplay.AddAll(sceneList);
    }
    
    public void UpdateSceneItems(List<SceneDataItem> sceneDataItems) {
        sceneListDisplay.UpdateSceneItems(sceneDataItems);
    }
    
    public void Clear() {
        sceneListDisplay.Clear();
    }
    
    // ##### Callbacks #####
    
    private void OnDeleteToggle(bool deleteVisible) {
        sceneListDisplay.ToggleDelete(deleteVisible);
    }

    private void OnRefresh() {
        EmitSignal(SignalName.Refreshed);
    }
}

#endif