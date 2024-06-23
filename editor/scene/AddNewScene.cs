using Godot;
using System.Diagnostics;

[Tool]
public partial class AddNewScene : Container {
    [Export] private FileInput fileInput;
    [Export] private LineEdit keyLineEdit;
    
    [Export] private Button clearButton;
    [Export] private Button addButton;

    [Export] private StyleBox invalidLineEditTheme;
    [Export] private StyleBox invalidLineEditReadOnlyTheme;
    
    public override void _EnterTree() {
        keyLineEdit ??= GetNode<LineEdit>("%New Key Input");
        fileInput ??= GetNode<FileInput>("%File Input");
        clearButton ??= GetNode<Button>("%Clear");
        addButton ??= GetNode<Button>("%Add");

        invalidLineEditTheme ??= ResourceLoader.Load<StyleBox>("res://addons/scene_manager_mono/theme/invalid_stylebox.tres");
        invalidLineEditReadOnlyTheme ??= ResourceLoader.Load<StyleBox>("res://addons/scene_manager_mono/theme/invalid_stylebox_readonly.tres");
        
        Debug.Assert(fileInput is not null);
        Debug.Assert(keyLineEdit is not null);
        Debug.Assert(clearButton is not null);
        Debug.Assert(addButton is not null);
        Debug.Assert(invalidLineEditTheme is not null);
        Debug.Assert(invalidLineEditReadOnlyTheme is not null);
    }

    public override void _Ready() {
        fileInput.Changed += OnFileSelected;
        keyLineEdit.FocusEntered += OnKeyFocused;
        addButton.Pressed += OnAdd;
        clearButton.Pressed += OnClear;
    }

    ///// Private Functions /////
    /// 
    private void OnKeyFocused() {
        ClearLineEditInvalidTheme(keyLineEdit);
        fileInput.SetValid(true);
    }
    
    private void OnClear() {
        ClearPath();
        fileInput.SetValid(true);
        keyLineEdit.Clear();
        ClearLineEditInvalidTheme(keyLineEdit);
    }
    
    private void OnAdd() {
        if (keyLineEdit.Text == "") {
            SetLineEditInvalidTheme(keyLineEdit);
        }

        if (fileInput.Text == "") {
            fileInput.SetValid(false);
        }
        
        if (keyLineEdit.Text != "" && fileInput.Text != "") {
            SceneManagerEditor.Instance.AddSceneData(keyLineEdit.Text, fileInput.Text);
            
            keyLineEdit.Clear();
            ClearPath();
        }
    }
    
    ///// File Dialog
    
    private void OnFileSelected() {
        fileInput.SetValid(true);
    }
    
    ///// Path Input 
    
    // line edit with clear button
    private void ClearPath() {
        fileInput.Text = "";
    }

    ///// Line Edit Util
    
    private void SetLineEditInvalidTheme(LineEdit lineEdit) {
        lineEdit.AddThemeStyleboxOverride("normal", invalidLineEditTheme);
        lineEdit.AddThemeStyleboxOverride("read_only", invalidLineEditReadOnlyTheme);
    }
    
    private void ClearLineEditInvalidTheme(LineEdit lineEdit) {
        lineEdit.RemoveThemeStyleboxOverride("normal");
        lineEdit.RemoveThemeStyleboxOverride("read_only");
    }
}
