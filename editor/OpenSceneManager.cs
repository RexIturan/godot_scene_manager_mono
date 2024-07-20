#if TOOLS
using Godot;
using SceneManagerMono.Editor.Util;

namespace SceneManagerMono.Editor;

[Tool]
public partial class OpenSceneManager : Button {
    public override void _Ready() {
        base._Ready();

        Pressed += OpenWindow;
    }

    private void OpenWindow() {
        var window = EditorInterface.Singleton.GetBaseControl().GetFirstOrDefaultNode<SceneManagerWindow>();

        if (window is not null) {
            if (window.Visible) {
                window.GrabFocus();
            }
            else {
                window.Show();    
            }
        }
    }
}
#endif
