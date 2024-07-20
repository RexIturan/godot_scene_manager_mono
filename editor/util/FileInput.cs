#if TOOLS
using Godot;

namespace SceneManagerMono.Editor.Util;

[Tool]
public partial class FileInput : Container {
    [Export] private FileType fileType;
    [Export] private bool editable;
    
    private bool _showFileDialog;
    [Export] public bool ShowFileDialog {
        get => _showFileDialog;
        set {
            _showFileDialog = value;
            if (fileDialogButton is {}) {
                fileDialogButton.Visible = _showFileDialog;
            }
        }
    }
    
    [Export] private StyleBox invalidLineEditTheme;
    [Export] private StyleBox invalidLineEditReadOnlyTheme;

    private DragFilePathInput filePathInput;
    private SceneFileDialog fileDialogButton;

    public string Text {
        get => filePathInput?.Text ?? "";
        set {
            if (filePathInput is {}) {
                filePathInput.Text = value;    
            }
        }
    }

    [Signal]
    public delegate void ChangedEventHandler();
    
    // ##### Godot Functions #####
    
    public override void _EnterTree() {
        base._EnterTree();
        var fileExtensionFilter = fileType.GetExtensions();

        filePathInput = GetNode<DragFilePathInput>("%Drag File Input");
        fileDialogButton = GetNode<SceneFileDialog>("%Open FileDialog");

        filePathInput.FileExtensionFilter = fileExtensionFilter;
        filePathInput.Active = editable;
        filePathInput.Changed += OnFileSelected;

        fileDialogButton.Visible = ShowFileDialog;
        fileDialogButton.FileType = fileType;
        //todo hook into data hooks

        fileDialogButton.FileSelected += OnFileSelected;
    }

    // ##### Private Functions #####
    
    private void OnFileSelected(string path) {
        if (filePathInput.Text != path) {
            filePathInput.Text = path;    
        }
        EmitSignal(SignalName.Changed);
    }
    
    private void SetLineEditInvalidTheme(LineEdit lineEdit) {
        if (lineEdit is { }) {
            lineEdit.AddThemeStyleboxOverride("normal", invalidLineEditTheme);
            lineEdit.AddThemeStyleboxOverride("read_only", invalidLineEditReadOnlyTheme);    
        }
    }
    
    private void ClearLineEditInvalidTheme(LineEdit lineEdit) {
        if (lineEdit is { }) {
            lineEdit.RemoveThemeStyleboxOverride("normal");
            lineEdit.RemoveThemeStyleboxOverride("read_only");
        }
    }

    public void SetValid(bool isValid) {
        if (isValid) {
            ClearLineEditInvalidTheme(filePathInput);
        }
        else {
            SetLineEditInvalidTheme(filePathInput);
        }
    }
}
#endif
