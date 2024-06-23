#if TOOLS
using Godot;

[Tool]
public partial class ToggleShowDeleteButton : CheckButton {
    public override void _Ready() {
        base._Ready();

        if (SceneManagerEditor.Exists()) {
            ButtonPressed = SceneManagerEditor.Instance.ShowSceneDelete;
            Pressed += ToggleShowDelete;
            SceneManagerEditor.Instance.ShowSceneDeleteChanged += UpdateValue;
        }
    }

    ///// Private Functions /////
    
    private void UpdateValue(bool newvalue) {
        if (SceneManagerEditor.Exists()) {
            ButtonPressed = SceneManagerEditor.Instance.ShowSceneDelete;    
        }
    }

    private void ToggleShowDelete() {
        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.ShowSceneDelete = !SceneManagerEditor.Instance.ShowSceneDelete;    
        }
    }
}
#endif
