using Godot;
using Godot.Collections;
#if TOOLS
using SceneManagerMono.Editor.Data;
#endif

namespace SceneManagerMono.Data;

[Tool]
[GlobalClass]
public partial class SceneData : Resource {
    [Export] public string Key { get; set; }
    [Export] public string Path { get; set; }
    [Export] public string name;
    [Export] public string description;
    [Export] public Texture2D texture;
    [Export] public Array<string> tags;
    //todo add group ref?

    public SceneData() {}
    
#if TOOLS
    public SceneData(SceneDataItem dataItem) {
        UpdateData(dataItem);
    }
    
    public void UpdateData(SceneDataItem dataItem) {
        Key = dataItem.key;
        name = dataItem.name;
        Path = dataItem.path;
        description = dataItem.description;
        tags = new Array<string>(dataItem.tags);
        texture = null;
        if (dataItem.IsImagePathValid()) {
            texture = ResourceLoader.Load<Texture2D>(dataItem.imagePath);
        }
    }
#endif
}
