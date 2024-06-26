using Godot;

namespace SceneManagerMono.Runtime.Util;

public partial class SceneActionButton : Button {
    [Export] private SceneAction action;

    public override void _Ready() {
        Pressed += OnSceneAction;
    }

    private void OnSceneAction() {
        var options = new SceneManager.Options() { addToHistory = false };
        SceneManager.Instance.ChangeScene(action, options);
    }
}
