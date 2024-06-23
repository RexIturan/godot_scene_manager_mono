#if TOOLS
using Godot;

[Tool]
public partial class DirectoryItem : Container {
    private Button removeButton;
    private LineEdit lineEdit;

    public string Text {
        get => lineEdit?.Text ?? "";
        set {
            if (lineEdit is {}) {
                lineEdit.Text = value;
            }
        }
    }
    
    [Signal]
    public delegate void DeleteRequestedEventHandler(string value);
    
    ///// Godot Functions /////
    
    public override void _EnterTree() {
        base._EnterTree();
        
        removeButton ??= GetNode<Button>("%Delete");
        lineEdit ??= GetNode<LineEdit>("%LineEdit");
    }

    public override void _Ready() {
        base._Ready();
        removeButton.Pressed += OnDelete;
        lineEdit.Text = Text;
    }

    private void OnDelete() {
        EmitSignal(SignalName.DeleteRequested, Text);
    }
    
    ///// Static Functions /////

    public static DirectoryItem CreateInstance() {
        return GodotHelper.LoadAndInstaniate<DirectoryItem>(
            "res://addons/scene_manager_mono/editor/directory/directory_item.tscn"
        );
    }
}
#endif
