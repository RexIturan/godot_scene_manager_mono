#if TOOLS
using System.Collections.Generic;
using Godot;

public struct SceneDataItem {
    public int index = 0;
    public bool duplicateKey = false;
    public string ResourcePath { get; init; }
    public string key = "";
    public string name = "";
    public string path = "";
    public string description = "";
    public string imagePath = "";
    public List<string> tags = new();

    public SceneDataItem() {
        ResourcePath = null;
    }

    public SceneDataItem(SceneData item) {
        key = item.Key;
        name = item.name;
        path = item.Path;
        description = item.description;
        imagePath = item.texture is {} ? item.texture.ResourcePath : "";
        tags = new List<string>(item.tags);
        ResourcePath = item.ResourcePath;
    }

    ///// Public Functions /////
    
    public SceneData GetResource() {
        return new SceneData(this);
    }

    public bool IsPathValid() {
        return ValidateSceneItemPath(path);
    }

    public bool IsImagePathValid() {
        return ValidateSceneItemPath(imagePath);
    }
    
    ///// Private Functions /////
    
    private bool ValidateSceneItemPath(string path) {
        return FileAccess.FileExists(path);
    }
}
#endif
