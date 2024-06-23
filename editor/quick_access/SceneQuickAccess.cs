#if TOOLS
using Godot;

[Tool]
public partial class SceneQuickAccess : Control {
    [Export] private PackedScene sceneItemPrefab;
    [Export] private Container sceneItemContainer;

    // ##### Godot Functions #####
    
    public override void _EnterTree() {
        base._EnterTree();
        
        //todo get sceneItemContainer
        sceneItemContainer ??= GetNode<Container>("%Scene Item Container");
    }

    public override void _Ready() {
        base._Ready();

        SceneManagerEditor.Instance.Changed += OnSceneDataChanged;
        
        Clear();
        CreateSceneList();
    }

    public override void _ExitTree() {
        base._ExitTree();
        
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.Changed -= OnSceneDataChanged;    
        }
    }
    
    // ##### Callbacks #####

    private void OnSceneDataChanged() {
        Clear();
        CreateSceneList();
    }
    
    // ##### Private Functions #####
    
    private void CreateSceneList() {
        if (SceneManagerEditor.Instance.HasValidData) {
            var sceneDataList = SceneManagerEditor.Instance.SceneDataItems;
            
            foreach (var sceneData in sceneDataList) {
                var item = CreateSceneItem();
                item.Error = sceneData.duplicateKey;
                
                sceneItemContainer.AddChild(item);
                
                item.Key = sceneData.key;
                item.Path = sceneData.path;
                item.Index = sceneData.index;
            }
        }
    }
    
    private void Clear() {
        foreach (var child in sceneItemContainer.GetChildren()) {
            sceneItemContainer.RemoveChild(child);
            child.QueueFree();
        }
    }
    
    private SceneQuickAccessItem CreateSceneItem() {
        return sceneItemPrefab.Instantiate<SceneQuickAccessItem>();
    }  
}
#endif
