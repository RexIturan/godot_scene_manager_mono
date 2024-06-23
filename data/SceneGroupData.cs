using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class SceneGroupData : Resource {
    [Export] public string name;
    [Export] public string texture;
    [Export] public Array<int> sceneIndexList;
    [Export] public Array<string> tags;
    
    public SceneGroupData() {}

    public SceneGroupData(SceneDataGroupItem groupData) {
        UpdateData(groupData);
    }
    
    public void UpdateData(SceneDataGroupItem groupData) {
        name = groupData.name;
        tags = new Array<string>(groupData.tags);
        texture = groupData.icon;
        sceneIndexList = new Array<int>(groupData.scenes);
    }
}
