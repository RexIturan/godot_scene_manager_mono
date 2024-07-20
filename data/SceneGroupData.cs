using Godot;
using Godot.Collections;
#if TOOLS
using SceneManagerMono.Editor.Data;
#endif

namespace SceneManagerMono.Data;

[Tool]
[GlobalClass]
public partial class SceneGroupData : Resource {
    [Export] public string name;
    [Export] public string texture;
    [Export] public Array<int> sceneIndexList;
    [Export] public Array<string> tags;
    
    public SceneGroupData() {}

#if TOOLS
    public SceneGroupData(SceneDataGroupItem groupData) {
        UpdateData(groupData);
    }
    
    public void UpdateData(SceneDataGroupItem groupData) {
        name = groupData.name;
        tags = new Array<string>(groupData.tags);
        texture = groupData.icon;
        sceneIndexList = new Array<int>(groupData.scenes);
    }
#endif
}
