#if TOOLS
using Godot;
using System.Diagnostics;
using SceneManagerMono.Editor.Data;
using SceneManagerMono.Editor.Scene;
using SceneManagerMono.Editor.Util;

namespace SceneManagerMono.Editor.Scene_List;

[Tool]
public partial class SceneItem : Control {
    [Export] private LineEdit keyInput;
    // [Export] private LineEdit pathInput;
    [Export] private FileInput fileInput;
    [Export] private Button delete;
    [Export] private OpenSceneDataEditWindow edit;
    
    [Export] private StyleBox invalidLineEditTheme;
    [Export] private StyleBox invalidLineEditReadOnlyTheme;

    [Signal] public delegate void DeleteRequestedEventHandler();
    [Signal] public delegate void KeyChangedEventHandler(string key, string oldKey);
    [Signal] public delegate void PathChangedEventHandler(string path);

    public SceneDataItem SceneDataItem { get; set; }
    public string Key => keyInput.Text;
    public string Path => fileInput.Text;
    private string lastKey = "";
    
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
    
    public override void _EnterTree() {
        // pathInput ??= GetNode<LineEdit>("%path");
        keyInput ??= GetNode<LineEdit>("%key");
        fileInput ??= GetNode<FileInput>("%File Input");
        delete ??= GetNode<Button>("%delete");
        edit ??= GetNode<OpenSceneDataEditWindow>("%edit");
        Debug.Assert(keyInput is not null);
        Debug.Assert(fileInput is not null);
        Debug.Assert(delete is not null);
        
        invalidLineEditTheme ??= ResourceLoader.Load<StyleBox>("res://addons/scene_manager_mono/theme/invalid_stylebox.tres");
        invalidLineEditReadOnlyTheme ??= ResourceLoader.Load<StyleBox>("res://addons/scene_manager_mono/theme/invalid_stylebox_readonly.tres");
         
        Debug.Assert(invalidLineEditTheme is not null);
        Debug.Assert(invalidLineEditReadOnlyTheme is not null);
    }

    public override void _Ready() {
        delete.Pressed += OnDelete;
        keyInput.TextChanged += OnKeyChanged;
        fileInput.Changed += OnFileSelected;
        
        if (SceneManagerEditor.Exists()) {
            SetDeleteVisible(SceneManagerEditor.Instance.ShowSceneDelete);
            SceneManagerEditor.Instance.ShowSceneDeleteChanged += SetDeleteVisible;
        }
    }

    public override void _ExitTree() {
        base._ExitTree();

        if (SceneManagerEditor.Exists()) {
            SceneManagerEditor.Instance.ShowSceneDeleteChanged -= SetDeleteVisible;
        }
    }

    public void Init(SceneDataItem sceneData) {
        var key = sceneData.key;
        var path = sceneData.path;
        Index = sceneData.index;
        
        lastKey = key;
        if (keyInput is {} && keyInput.Text != key) {
            keyInput.Text = key;
        }

        if (fileInput is {} && fileInput.Text != path) {
            fileInput.Text = path;
        }
        
        SetIndex();
    }

    public void SetKeyValidity(bool isValid) {
        if (isValid) {
            ClearLineEditInvalidTheme(keyInput);
        }
        else {
            SetLineEditInvalidTheme(keyInput);
        }
    }
    
    public void SetPathValidity(bool isValid) {
        fileInput.SetValid(isValid);
        fileInput.ShowFileDialog = !isValid;
    }
    
    public void SetDeleteVisible(bool deleteVisible) {
        delete.Visible = deleteVisible;
    }
    
    ///// Callbacks
    
    private void OnDelete() {
        EmitSignal(SignalName.DeleteRequested);
    }
    
    private void OnKeyChanged(string newKey) {
        EmitSignal(SignalName.KeyChanged, newKey, lastKey);
        lastKey = newKey;
    }
    
    private void OnFileSelected() {
        EmitSignal(SignalName.PathChanged, fileInput.Text);
    }
    
    ///// Line Edit Util Functions
    
    private void SetIndex() {
        if (edit is not null) {
            edit.Index = Index;
        }
    }
    
    private void SetLineEditInvalidTheme(LineEdit lineEdit) {
        if (lineEdit is { }) {
            lineEdit.AddThemeStyleboxOverride("normal", invalidLineEditTheme);
            lineEdit.AddThemeStyleboxOverride("read_only", invalidLineEditReadOnlyTheme);    
        }
    }
    
    private void ClearLineEditInvalidTheme(LineEdit lineEdit) {
        if (lineEdit is {}) {
            lineEdit.RemoveThemeStyleboxOverride("normal");
            lineEdit.RemoveThemeStyleboxOverride("read_only");    
        }
    }
}
#endif
