#if TOOLS
using Godot;

[Tool]
public partial class ResourceUidDisplay : Container {
    private LineEdit FileInputLong => GetNode<LineEdit>("%UID long");
    private LineEdit FileInputString => GetNode<LineEdit>("%UID string");

    public override void _Ready() {
        base._Ready();

        SetUids();
        ProjectSettings.SettingsChanged += OnProjectSettingsChanged;
    }

    // public override void _ExitTree() {
    //     base._ExitTree();
    //     
    //     ProjectSettings.SettingsChanged -= OnProjectSettingsChanged;
    // }

    private void OnProjectSettingsChanged() {
        SetUids();
    }

    private void SetUids() {
        var path = SceneManagerSettings.GetSceneListPath();
        
        if (ResourceLoader.Exists(path)) {
            var uid = ResourceLoader.GetResourceUid(SceneManagerSettings.GetSceneListPath());
            FileInputLong.Text = uid.ToString();
            FileInputString.Text = ResourceUid.IdToText(uid);
        }
        else {
            FileInputLong.Text = "";
            FileInputString.Text = "";
        }
    }
}
#endif