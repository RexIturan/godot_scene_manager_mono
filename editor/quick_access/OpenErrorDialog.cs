#if TOOLS
using Godot;

namespace SceneManagerMono.Editor.Quick_Access;

[Tool]
public partial class OpenErrorDialog : Button {
    [Export] private PackedScene dialogPrefab;

    public override void _Ready() {
        base._Ready();

        Pressed += OpenDialog;
    }

    private void OpenDialog() {
        var dialog = dialogPrefab.Instantiate<AcceptDialog>();
        
        if (dialog is not null) {
            EditorInterface.Singleton.GetBaseControl().AddChild(dialog);
        }
    }
}
#endif
