#if TOOLS
using Godot;

[Tool]
public partial class SceneQuickAccessItem : Control {
    [Export] private Label label;
    [Export] private Button openButton;
    [Export] private Button errorButton;
    [Export] private OpenSceneDataEditWindow editButton;
    [Export] private ShowInFileSystem showInFileSystem;

    private int _index;
    public int Index {
        get => _index;
        set {
            if (_index != value) {
                _index = value;
                SetIndex();
            }
        }
    }

    private string _key;
    public string Key {
        get => _key;
        set {
            if (_key != value) {
                _key = value;
                SetKey();
            }
        }
    }
    
    private string _path;
    public string Path {
        get => _path;
        set {
            if (_path != value) {
                _path = value;
                SetPath();    
            }
        }
    }

    private bool _error = false;
    public bool Error {
        get => _error;
        set {
            if (_error != value) {
                _error = value;
                if (errorButton is not null) {
                    SetErrorVisibility(_error);
                }
            }
        }
    }
    
    public override void _EnterTree() {
        base._EnterTree();

        label ??= GetNode<Label>("%Scene Key");
        openButton ??= GetNode<Button>("%Open");
        errorButton ??= GetNode<Button>("%Error");
        editButton ??= GetNode<OpenSceneDataEditWindow>("%Edit");
        showInFileSystem ??= GetNode<ShowInFileSystem>("%ShowInFileSystem");
        SetKey();
        SetPath();
        SetIndex();
        
        openButton.Pressed += OnOpenSceneData;
        SetErrorVisibility(Error);
    }

    // ##### Private Functions #####
    
    private void OnOpenSceneData() {
        EditorInterface.Singleton.OpenSceneFromPath(Path);
    }

    private void SetKey() {
        if (label is not null) {
            label.Text = Key;    
        }
    }

    private void SetIndex() {
        if (editButton is not null) {
            editButton.Index = Index;
        }
    }

    private void SetPath() {
        if (showInFileSystem is { }) {
            showInFileSystem.Path = Path;
        }
    }
    
    private void SetErrorVisibility(bool hasError) {
        if (hasError) {
            errorButton.Show();
        }
        else {
            errorButton.Hide();
        }
    }
}
#endif
