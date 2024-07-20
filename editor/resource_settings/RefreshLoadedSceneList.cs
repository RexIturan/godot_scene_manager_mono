#if TOOLS
using Godot;

namespace SceneManagerMono.Editor.Resource_Settings;

[Tool]
public partial class RefreshLoadedSceneList : Button {
    ///// Godot Functions /////
    
    public override void _EnterTree() {
        base._EnterTree();
        
        if (SceneManagerEditor.Exists()) {
            Pressed += OnResfresh;    
        }
    }

    ///// Callback Functions /////
    
    private void OnResfresh() {
        SceneManagerEditor.Instance?.Load();
        SceneManagerEditor.Instance?.InitData();
    }
}
#endif
