#if TOOLS
using Godot;

[Tool]
public partial class RefreshLoadedSceneList : Button {
    public override void _Ready() {
        base._Ready();

        if (SceneManagerEditor.Exists()) {
            Pressed += OnResfresh;    
        }
    }

    private void OnResfresh() {
        SceneManagerEditor.Instance?.Load();
        SceneManagerEditor.Instance?.InitData();
    }
}
#endif
