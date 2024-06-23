#if TOOLS
using Godot;

[Tool]
public partial class RefreshLoadedSceneList : Button {
    ///// Godot Functions /////
    
    public override void _EnterTree() {
        base._EnterTree();
        
        if (SceneManagerEditor.Exists()) {
            Pressed += OnResfresh;    
        }
    }

    public override void _ExitTree() {
        base._ExitTree();
    }

    public override void _Ready() {
        base._Ready();

    }

    ///// Callback Functions /////
    
    private void OnResfresh() {
        SceneManagerEditor.Instance?.Load();
        SceneManagerEditor.Instance?.InitData();
    }
}
#endif
