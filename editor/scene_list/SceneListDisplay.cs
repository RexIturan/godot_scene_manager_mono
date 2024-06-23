#if TOOLS
using Godot;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class SceneListDisplay : Container {
    [Export] private PackedScene sceneItemPrefab;
    
    private List<SceneItem> sceneItems = new();
    // private Dictionary<string, List<SceneItem>> keys = new();
    
    // ##### Godot Functions #####
    
    public override void _EnterTree() {
        sceneItemPrefab ??=
            ResourceLoader.Load<PackedScene>("res://addons/scene_manager_mono/editor/scene_list/scene_list_item.tscn");
    }

    public override void _ExitTree() {
        base._ExitTree();
        Clear();
    }

    public override void _Ready() {
        Clear();
    }

    // ##### Public Functions #####
    
    public void Clear() {
        foreach (var sceneItem in GetChildren()) {
            RemoveChild(sceneItem);
            sceneItem.QueueFree();
        }
        
        sceneItems.Clear();
    }

    public void UpdateSceneItems(List<SceneDataItem> sceneDataItems) {
        if (sceneDataItems.Count != sceneItems.Count) {
            Clear();
            AddAll(sceneDataItems);
        }
        else {
            foreach (var sceneItem in sceneItems) {
                var found = sceneDataItems.First(item => item.index == sceneItem.SceneDataItem.index);

                if (found is { }) {
                    sceneItem.SceneDataItem = found;
                    sceneItem.Init(found);
                    
                    sceneItem.SetKeyValidity(!sceneItem.SceneDataItem.duplicateKey);
                    sceneItem.SetPathValidity(ValidateSceneItemPath(sceneItem.Path));
                }
            }
        }
    }
    
    public void AddAll(List<SceneDataItem> sceneDataList) {
        foreach (var sceneData in sceneDataList) {
            AddItem(sceneData);
        }
    }

    public void ToggleDelete(bool deleteVisible) {
        sceneItems.ForEach(sceneItem => sceneItem.SetDeleteVisible(deleteVisible));
    }
    
    public void AddItem(SceneDataItem sceneDataItem) {
        var sceneItem = CreateSceneItem();
        sceneItems.Add(sceneItem);
        sceneItem.SceneDataItem = sceneDataItem;

        AddChild(sceneItem);
        sceneItem.Init(sceneDataItem);
        
        sceneItem.SetKeyValidity(!sceneItem.SceneDataItem.duplicateKey);
        sceneItem.SetPathValidity(ValidateSceneItemPath(sceneItem.Path));

        sceneItem.PathChanged += (path) => OnPathChanged(sceneItem, path);
        sceneItem.KeyChanged += (key, _) => OnKeyChanged(sceneItem, key);
        sceneItem.DeleteRequested += () => OnDeleteSceneItem(sceneItem);
    }
    
    // ##### Callbacks #####
    
    private void OnDeleteSceneItem(SceneItem sceneItem) {
        SceneManagerEditor.Instance.RequestDeleteScene(sceneItem.SceneDataItem, true);
    }

    private void OnPathChanged(SceneItem sceneItem, string path) {
        sceneItem.SetPathValidity(ValidateSceneItemPath(path));
    }
    
    private void OnKeyChanged(SceneItem sceneItem, string key) {
        SceneManagerEditor.Instance.SetSceneKey(sceneItem.SceneDataItem.index, key);
    }
    
    // ##### Util Functions #####

    private bool ValidateSceneItemPath(string path) {
        return FileAccess.FileExists(path);
    }

    private SceneItem CreateSceneItem() {
        return sceneItemPrefab.Instantiate<SceneItem>();
    }
}
#endif