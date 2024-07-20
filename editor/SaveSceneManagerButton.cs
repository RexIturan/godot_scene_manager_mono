#if TOOLS
using Godot;

namespace SceneManagerMono.Editor;

[Tool]
public partial class SaveSceneManagerButton : Button {
    public override void _Ready() {
        base._Ready();

        if (SceneManagerEditor.Exists()) {
            Pressed += OnSave;    
        }
    }

    private void OnSave() {
        SceneManagerEditor.Instance?.Save();
    }
}
#endif
