#if TOOLS
using Godot;

[Tool]
public partial class ShowInFileSystem : Button {
    public string Path { get; set; }

    public override void _Ready() {
        base._Ready();

        Pressed += OnShowInFileSystem;
    }

    private void OnShowInFileSystem() {
        EditorInterface.Singleton.GetFileSystemDock().NavigateToPath(Path);
    }
}
#endif
