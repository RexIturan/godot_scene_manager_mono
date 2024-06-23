#if TOOLS
using Godot;
using System.Diagnostics;


//todo export fileextension filter
[Tool]
public partial class SceneFileDialog : Button {
    [Export] public string FilterName { get; set; } = "Scene Files";
    [Export] private string FileExtensionFilter { get; set; } = "*.tscn, *.scn, *.res";
    [Export] private FileDialog fileDialog;

    [Export] public FileType FileType;
    
    [Signal] public delegate void FileSelectedEventHandler(string path);
    
    public override void _EnterTree() {
        fileDialog ??= GetNode<FileDialog>("%FileDialog");
        Debug.Assert(fileDialog is not null);
    }

    public override void _Ready() {
        Pressed += OpenFileDialog;
        fileDialog.FileSelected += OnFileSelected;
        fileDialog.DirSelected += OnFileSelected;
        SetFileDialogMasks();
    }

    // ##### Callbacks #####
    
    private void SetFileDialogMasks() {
        fileDialog.FileMode = FileType.GetFileMode();
        fileDialog.ClearFilters();
        fileDialog.AddFilter($"{FileType.GetExtensions()}; {FileType.GetFilterName()}");
    }
    
    private void OpenFileDialog() {
        fileDialog.PopupCentered(new Vector2I(600, 600));
    }
    
    private void OnFileSelected(string path) {
        EmitSignal(SignalName.FileSelected, path);
    }
}
#endif
