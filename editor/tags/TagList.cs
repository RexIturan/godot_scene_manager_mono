#if TOOLS
using Godot;

[Tool]
public partial class TagList : Container {
    [Export] private PackedScene tagItemPrefab;
    
    public override void _Ready() {
        base._Ready();

        var scenemanagerEditor = SceneManagerEditor.Instance;
        if (scenemanagerEditor is { }) {
            scenemanagerEditor.Changed += CreateTagItemList;

            CreateTagItemList();
        }
    }

    private void Clear() {
        foreach (var items in GetChildren()) {
            RemoveChild(items);
            items.QueueFree();
        }
    }
    
    private void CreateTagItemList() {
        Clear();
        var tags = SceneManagerEditor.Instance.TagList;
        
        foreach (var tag in tags) {
            var groupItem = CreateSceneGroupItem();
            groupItem.TagData = tag;
            AddChild(groupItem);
        }
    }

    private TagItem CreateSceneGroupItem() {
        return tagItemPrefab.Instantiate<TagItem>();
    }
}
#endif
