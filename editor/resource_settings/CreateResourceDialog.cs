#if TOOLS
using System;
using Godot;
using SceneManagerMono.Editor.Util;

namespace SceneManagerMono.Editor.Resource_Settings;

[Tool]
public partial class CreateResourceDialog : ConfirmationDialog {
    private LineEdit LineEdit => GetNode<LineEdit>("%LineEdit");
    private FileInput FileInput => GetNode<FileInput>("%File Input");

    private string path;
    
    [Signal]
    public delegate void CreationRequestedEventHandler(string path);
    
    ///// Godot Function /////
    
    public override void _Ready() {
        base._Ready();

        UpdateOkButton();
        
        Hide();

        var fileInput = FileInput;
        fileInput.Changed += UpdateOkButton;
        LineEdit.TextChanged += OnNameChanged;
        Confirmed += OnConfirmation;
    }

    ///// Private Function /////
     
    private void OnConfirmation() {
        EmitSignal(SignalName.CreationRequested, path);
    }
    
    private void OnNameChanged(string newtext) {
        UpdateOkButton();
    }
    
    private void UpdateOkButton() {
        GetOkButton().Disabled = !IsPathValid();
    }

    private bool IsPathValid() {
        var seperator = FileInput.Text[^1] == '/' ? "" : "/";
        path = $"{FileInput.Text}{seperator}{LineEdit.Text}.tres";
        return FileInput.Text != "" && LineEdit.Text != "" && !ResourceLoader.Exists(path);
    }
}
#endif
