using Godot;

[Tool]
public partial class SceneManager_EditorWindow : Control {
    [Export] private SceneList sceneList;
    [Export] private Button saveButton;
    [Export] private Button loadButton;

    public override void _EnterTree() {
        sceneList ??= GetNode<SceneList>("%Scene List");
        saveButton ??= GetNode<Button>("%save");
        loadButton ??= GetNode<Button>("%load");
    }

    public override void _Ready() {
        if (SceneManagerEditor.Instance is {}) {
            loadButton.Pressed += CreateSceneDataList;
            saveButton.Pressed += Save;

            sceneList.Refreshed += OnRefresh;

            SceneManagerEditor.Instance.Changed += UpdateSceneDataList;
        
            CreateSceneDataList();            
        }
    }

    public override void _ExitTree() {
        base._ExitTree();
        
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.Changed -= UpdateSceneDataList;    
        }
    }

    // ##### Private Functions #####
    
    private void Save() {
        SceneManagerEditor.Instance.Save();
    }
    
    private void CreateSceneDataList() {
        if (SceneManagerEditor.Instance.HasValidData) {
            sceneList.Clear();
            sceneList.AddAll(SceneManagerEditor.Instance.SceneDataItems);    
        }
    }
    
    private void UpdateSceneDataList() {
        if (SceneManagerEditor.Instance.HasValidData) {
            sceneList.UpdateSceneItems(SceneManagerEditor.Instance.SceneDataItems);    
        }
    }
    
    // ##### Callbacks #####
    
    private void OnRefresh() {
        UpdateSceneDataList();
    }
}
