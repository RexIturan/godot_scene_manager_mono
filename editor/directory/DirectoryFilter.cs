#if TOOLS
using System.Collections.Generic;
using Godot;
using SceneManagerMono.Editor.Util;
using Container = Godot.Container;

namespace SceneManagerMono.Editor.Directory;

[Tool]
public partial class DirectoryFilter : Container {
    private Label label;
    private FileInput fileInput;
    private Button addButton;
    private DirectoryList directoryList;

    private string _labelText;
    [Export] public string LabelText {
        get => _labelText;
        set {
            _labelText = value;
            if (label is {}) {
                label.Text = _labelText;
            }
        }
    }

    public List<string> Values { get; set; } = new();

    [Signal]
    public delegate void ChangedEventHandler();
    
    ///// Godot Functions /////
    
    public override void _EnterTree() {
        base._EnterTree();

        label ??= GetNode<Label>("%Label");
        fileInput ??= GetNode<FileInput>("%File Input");
        addButton ??= GetNode<Button>("%Add");
        directoryList ??= GetNode<DirectoryList>("%Directory List");
    }

    public override void _Ready() {
        base._Ready();

        addButton.Pressed += OnAdd;
        directoryList.Removed += OnItemRemoved;
        label.Text = LabelText;
    }

    public override void _ExitTree() {
        base._ExitTree();
        
        directoryList.Removed -= OnItemRemoved;
    }

    public void UpdateValues() {
        directoryList.UpdateValues(Values);
    }
    
    ///// Private Functions /////
    
    private void OnAdd() {
        GD.Print($"OnAdd {fileInput.Text}");
        
        if (fileInput.Text != "" && !Values.Contains(fileInput.Text)) {
            Values.Add(fileInput.Text);
            directoryList.UpdateValues(Values);
            EmitSignal(SignalName.Changed);    
        }
        fileInput.Text = "";
    }
    
    private void OnItemRemoved(string removedValue) {
        Values.Remove(removedValue);
        directoryList.UpdateValues(Values);
        EmitSignal(SignalName.Changed);
    }
}
#endif
