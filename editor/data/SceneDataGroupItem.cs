using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

public struct SceneDataGroupItem {
    public int index = 0;
    public string ResourcePath { get; init; }
    public string name = "";
    public string icon = "";
    public List<int> scenes = new();
    public List<string> tags = new();

    public SceneDataGroupItem() {
        ResourcePath = "";
    }

    public SceneDataGroupItem(SceneGroupData group) {
        name = group.name;
        scenes = group.sceneIndexList.ToList();
        tags = group.tags.ToList();
        icon = group.texture;
        ResourcePath = group.ResourcePath;
    }
    
    public SceneGroupData GetResource() {
        return new SceneGroupData(this);
    }

    public bool IsImagePathValid() {
        return ValidateSceneItemPath(icon);
    }
    
    ///// Private Functions /////
    
    private bool ValidateSceneItemPath(string path) {
        return FileAccess.FileExists(path);
    }
}