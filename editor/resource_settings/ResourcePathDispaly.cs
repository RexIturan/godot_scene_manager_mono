#if TOOLS
using Godot;

[Tool]
public partial class ResourcePathDispaly : Container {
    private FileInput fileInput;

    public override void _EnterTree() {
        base._EnterTree();

        fileInput ??= GetNode<FileInput>("File Input");
    }

    public override void _Ready() {
        base._Ready();

        UpdateResourcePath();
        ProjectSettings.SettingsChanged += OnProjectSettingsChanged;
    }

    // public override void _ExitTree() {
    //     base._ExitTree();
    //     
    //     // ProjectSettings.SettingsChanged -= OnProjectSettingsChanged;
    // }

    ///// Private Functions /////
    
    private void OnProjectSettingsChanged() {
        UpdateResourcePath();
    }

    private void UpdateResourcePath() {
        fileInput.Text = SceneManagerSettings.GetSceneListPath();
    }
}
#endif
